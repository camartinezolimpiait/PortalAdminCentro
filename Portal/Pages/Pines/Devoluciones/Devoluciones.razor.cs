using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Application.PortalAdministrativo;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Pines.Devoluciones
{
    public partial class Devoluciones
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private IOfuscamientoService OfuscamientoService { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        private bool isLoading = true;
        private readonly PaginationState Pagination = new() { ItemsPerPage = 5 };
        //private IQueryable<List<RowList>> Data;

        private GridItemsProvider<RowList>? Data;
        private List<ColumnList> Columnas;

        private readonly ConsultaInfoPinEstado DatosFiltros = new();
        private List<ResponseConsultaDevolucion> Listado = [];
        private List<ResponseConsultaDevolucion> ListadoDescarga = [];
        private GetDataResponseCentro GetCentroResponse = new();
        private Paginas Paginas = new();
        private Paginas PruebaPaginas = new Paginas();

        #endregion Variables

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            isLoading = false;

            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            GetCentroResponse = centroShared.Value.Respuesta;

            DatosFiltros.IdRunt = GetCentroResponse?.CodigoRUNT?.ToString(); //"101010"
            DatosFiltros.IdCentro = GetCentroResponse?.IdCentro ?? 0;
            DatosFiltros.NumRegistros = 10;
            DatosFiltros.NumPagina = 1;
            DatosFiltros.Estado = 1;
            DatosFiltros.Opcion = 1;
            DatosFiltros.FechaInicial = new(2023, 1, 1);
            DatosFiltros.FechaFinal = DateTime.Now;
            Columnas = PagoPinConst.InicializarColumnasDevoluciones();
            await GetConsultaDevoluciones();
            await SetPaginadorInicial(Listado, PruebaPaginas, false);
        }

        private async Task LoadQuickGridData(List<ResponseConsultaDevolucion> consultaDevolucionesList)
        {
            // Implementacion para crear el objeto dinamico del QuickGrid
            List<RowList> rows = [];

            if (consultaDevolucionesList.Count == 0)
            {
                rows.Add(new RowList() { Row = PagoPinConst.InicializarColumnasDevoluciones() });
            }
            consultaDevolucionesList.ForEach(x =>
            {
                _ = decimal.TryParse(x.ValorDevolver, out decimal valorDevolver);

                List<ColumnList> row =
                [
                    new() { NameColumn = "Canal de venta", Value = PagoPinConst.DescripcionCanalVenta[(EnumOrigenCotizacion)x.CanalVenta]},
                    new() { NameColumn = "Pin", Value = x.Pin},
                    new() { NameColumn = "Código de transacción", Value = (!string.IsNullOrEmpty(x.NUTVenta) ? x.NUTVenta : "No aplica") },
                    new() { NameColumn = "Número Documento", Value = x.NumeroIdentificacion },
                    new() { NameColumn = "Nombre Completo", Value = x.NombreCompleto },
                    new() { NameColumn = "Fecha Registro", Value =  x.FechaRegistro == new DateTime() ? "" : x.FechaRegistro?.ToString("dd-MM-yyyy") },
                    new() { NameColumn = "Fecha Devolución", Value = x.FechaDevolucion == new DateTime() ? "" : x.FechaDevolucion?.ToString("dd-MM-yyyy") },
                    new() { NameColumn = "Tipo Devolución", Value = x.TipoDevolucion },
                    new() { NameColumn = "Banco", Value = x.Banco },
                    new() { NameColumn = "Cuenta Banco", Value = x.CuentaBanco },
                    new() { NameColumn = "Correo", Value = x.Correo },
                    new() { NameColumn = "Valor a Devolver", Value = valorDevolver.FormatAsCurrency() },
                    new() { NameColumn = "Estado Devolución", Value = x.EstadoDevolucion },
                    new() { NameColumn = "Novedad", Value = x.NovedadDevolucion },
                    new() { NameColumn = "Agente Dispersión", Value = x.AgenteDispersion },
                    new() { NameColumn = "Pines Asociados", Value = "No encontrado" }, // comentado mientras se llega a entregaa certficacion
					//new() { NameColumn = "Cuotas", Value = (!string.IsNullOrEmpty(x.Cuotas) ? x.Cuotas : "0") },
                    new() { NameColumn = "Comprobante Devolución", Value = x.Pin  ??"" },
                    new() {NameColumn = "Convenio Empresa", Value = x.ConvenioEmpresa ?? ""},
                    new() { NameColumn = "Empresa", Value = x.Empresa ?? "" },
                ];

                rows.Add(new RowList() { Row = row });
            });

            // = new[] { rows }.AsQueryable();

            Data = async req =>
            {
                return GridItemsProviderResult.From(
                items: rows,
                totalItemCount: Listado.FirstOrDefault().TotalRegistros);
            };

            await Task.FromResult(true);
        }

        private async Task DatosFiltrosChanged(ConsultaInfoPinEstado value)
        {
            // Recibe el valor del componente hijo
            //DatosFiltros.IdRunt = "99999999";
            DatosFiltros.FechaInicial = value.FechaInicial;
            DatosFiltros.FechaFinal = value.FechaFinal;
            DatosFiltros.Documento = value.Documento;
            DatosFiltros.Pin = value.Pin;
            DatosFiltros.IdAgenteDispersion = value.IdAgenteDispersion;
            DatosFiltros.Canal = value.Canal;
            await GetConsultaDevoluciones();
            await SetPaginadorInicial(Listado, PruebaPaginas, false);
        }

        private async Task GetConsultaDevoluciones()
        {
            ResponseDTO<List<ResponseConsultaDevolucion>> result = await MiLicenciaService.ConsultaDevolucionesPines(DatosFiltros);

            if (result != null)
            {
                if (result.Codigo == 0 && result.Entidad != null && result.Entidad?.Count != 0)
                {
                    Listado = result.Entidad;

                    await OfuscarListado(false);
                    await LoadQuickGridData(Listado);
                }
                else
                {
                    await LoadQuickGridData(Listado);
                    await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, $"{result.Respuesta ?? "No se encontro información asociada a los filtros"}");
                }
            }
            else
            {
                await LoadQuickGridData(Listado);
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Ha ocurrido un error en la consulta.");
            }
        }

        private async Task DescargarDatos(ConsultaDescarga consulta)
        {
            if (consulta.IsDownload)
            {
                consulta.Consulta.Opcion = 2;
                consulta.Consulta.NumPagina = 1;
                consulta.Consulta.NumRegistros = 1;
                consulta.Consulta.IdRunt = DatosFiltros.IdRunt;

                ResponseDTO<List<ResponseConsultaDevolucion>> result = await MiLicenciaService.ConsultaDevolucionesPines(consulta.Consulta);

                if (result != null)
                {
                    try
                    {
                        if (result.Entidad != null)
                        {
                            if (result?.Entidad?.Count > 0)
                            {
                                ListadoDescarga = result.Entidad;
                                await OfuscarListado(false);
                                var listadoDescarga = await ArmadoListadoDescarga(ListadoDescarga);
                                DownloadExcelFile<ResponseConsultaDevolucionDTO> DownloadExcelFile = new(JS);

                                byte[] archivoBytes = await DownloadExcelFile.DownloadExcel(listadoDescarga, "ConsultaDevoluciones");
                                await DownloadExcelFile.DownloadFileFromStream(archivoBytes, "ConsultaDevoluciones");
                            }
                            else
                            {
                                await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "No se ha encontrado información para la descarga.");
                            }
                        }
                        else
                        {
                            await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "No se ha encontrado información para la descarga.");
                        }
                    }
                    catch (Exception ex)
                    {
                        await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, $"Ha ocurrido un error: {ex.Message}");
                    }
                }
                else
                {
                    await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "Ha ocurrido un error al realizar la consulta");
                }

                DatosFiltros.Opcion = 1;
                DatosFiltros.NumPagina = 1;
                DatosFiltros.NumRegistros = 10;
            }
        }

        public async Task OfuscarListado(bool descarga)
        {
            List<ResponseConsultaDevolucion> lista = descarga ? ListadoDescarga : Listado;

            foreach (var item in lista)
            {
                if (item.CanalVenta == (int)EnumOrigenCotizacion.MiLicencia || item.CanalVenta == (int)EnumOrigenCotizacion.PAO)
                {
                    // Ofuscar el email
                    item.Correo = await OfuscamientoService.Ofuscamiento(item.Correo);//OfuscarEmail(item.Correo);
                                                                                      // Ofuscar nombre
                    item.NombreCompleto = await OfuscamientoService.Ofuscamiento(item.NombreCompleto); //OfuscarNombreCompleto(item.NombreCompleto);
                }
            }
        }

        private async Task PaginaChanged(Paginas paginas)
        {
            PruebaPaginas = paginas;
            await RegistroPagina(PruebaPaginas.PaginaActual, PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
        }

        private async Task RegistroPagina(int pagina, int totalRegistrosPagina, int totalRegistros)
        {
            DatosFiltros.Opcion = 1;
            DatosFiltros.NumPagina = pagina;
            DatosFiltros.NumRegistros = totalRegistrosPagina;
            await GetConsultaDevoluciones();
            PruebaPaginas.RegistroInicialPagina = (pagina - 1) * totalRegistrosPagina + 1;
            PruebaPaginas.RegistroFinalPagina = Math.Min(pagina * totalRegistrosPagina, totalRegistros);
            PruebaPaginas.RegistrosPorPagina = totalRegistrosPagina;
            PruebaPaginas.PaginaActual = pagina;
            await SetPaginadorInicial(Listado, PruebaPaginas, true);
            StateHasChanged();
        }

        private async Task SetPaginadorInicial(List<ResponseConsultaDevolucion> listado, Paginas paginas, bool cambioPagina)
        {
            if (listado.Count != 0)
            {
                int currentRegistrosPagina = cambioPagina ? paginas.RegistrosPorPagina : 5;

                double cantidadPaginasDecimal = (double)(listado.FirstOrDefault()?.TotalRegistros ?? 0) / currentRegistrosPagina;
                int cantidadPaginas = (int)Math.Ceiling(cantidadPaginasDecimal);

                PruebaPaginas.PaginaActual = cambioPagina ? paginas.PaginaActual : 1;
                PruebaPaginas.TotalPaginas = cantidadPaginas;
                PruebaPaginas.PaginaInicial = 1;
                PruebaPaginas.PaginaFinal = cantidadPaginas;

                PruebaPaginas.RegistrosPorPagina = cambioPagina ? paginas.RegistrosPorPagina : 5;

                Pagination.ItemsPerPage = paginas.RegistrosPorPagina;

                PruebaPaginas.TotalRegistros = listado.FirstOrDefault().TotalRegistros;
                PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
                PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);

                if (cambioPagina)
                    await Pagination.SetCurrentPageIndexAsync(paginas.PaginaActual);
            }
        }

        private async Task<List<ResponseConsultaDevolucionDTO>> ArmadoListadoDescarga(List<ResponseConsultaDevolucion> Listado)
        {
            List<ResponseConsultaDevolucionDTO> lista = new();

            if (Listado != null)
            {
                Listado.ForEach(x =>
                {
                    ResponseConsultaDevolucionDTO item = new ResponseConsultaDevolucionDTO();

                    item.CanalVenta = (!string.IsNullOrEmpty(x.CanalVenta.ToString()) ? PagoPinConst.DescripcionCanalVenta[(EnumOrigenCotizacion)x.CanalVenta] : "No encontrado");
                    item.Pin = (!string.IsNullOrEmpty(x.Pin) ? x.Pin : "No encontrado");
                    item.NUTVenta = (!string.IsNullOrEmpty(x.NUTVenta) ? x.NUTVenta : "No aplica");
                    item.NumeroIdentificacion = (!string.IsNullOrEmpty(x.NumeroIdentificacion) ? x.NumeroIdentificacion : "No encontrado");
                    item.NombreCompleto = (!string.IsNullOrEmpty(x.NombreCompleto) ? x.NombreCompleto : "No encontrado");
                    item.FechaRegistro = (!string.IsNullOrEmpty(x.FechaRegistro.ToString()) ? x.FechaRegistro?.ToString("dd/MM/yyyy") : "");
                    item.FechaDevolucion = (!string.IsNullOrEmpty(x.FechaDevolucion.ToString()) ? x.FechaDevolucion?.ToString("dd/MM/yyyy") : "");
                    item.TipoDevolucion = (!string.IsNullOrEmpty(x.TipoDevolucion) ? x.TipoDevolucion : "");
                    item.Banco = (!string.IsNullOrEmpty(x.Banco) ? x.Banco : "");
                    item.CuentaBanco = (!string.IsNullOrEmpty(x.CuentaBanco) ? x.CuentaBanco : "No encontrado");
                    item.Correo = (!string.IsNullOrEmpty(x.Correo) ? x.Correo : "No encontrado");
                    item.ValorDevolver = (!string.IsNullOrEmpty(x.ValorDevolver) ? x.ValorDevolver : "No encontrado");
                    item.EstadoDevolucion = (!string.IsNullOrEmpty(x.EstadoDevolucion) ? x.EstadoDevolucion : "No encontrado");
                    item.NovedadDevolucion = (!string.IsNullOrEmpty(x.NovedadDevolucion) ? x.NovedadDevolucion : "No encontrado");
                    item.AgenteDispersion = (!string.IsNullOrEmpty(x.AgenteDispersion) ? x.AgenteDispersion : "No encontrado");
                    lista.Add(item);
                });
            }
            await Task.FromResult(true);
            return lista;
        }

        private async Task EventCallbackButtonClick(RowList rowList)
        {
            List<ColumnList> colums = rowList.Row;

            EnumOrigenCotizacion origenCotizacion = GetKeyFromValue(PagoPinConst.DescripcionCanalVenta, colums[0].Value);
            string tipoDevolucion = colums.FirstOrDefault(x => x.NameColumn == "Tipo Devolución")?.Value;
            string estadoDevolucion = colums.FirstOrDefault(x => x.NameColumn == "Estado Devolución")?.Value;

            if (origenCotizacion == EnumOrigenCotizacion.Centro)
            {
                if (tipoDevolucion == "Transferencia")
                {
                    if (estadoDevolucion == "Devolución Efectuada")
                    {
                        string pin = colums.FirstOrDefault(x => x.NameColumn == "Pin")?.Value;

                        FileBase64 fileResult = await MiLicenciaService.ConsultarComprobanteDevolucion(new ConsultaComprobanteDevolucionRequest()
                        {
                            Pin = pin,
                            NumeroIdentificacion = colums.FirstOrDefault(x => x.NameColumn == "Número Documento")?.Value,
                            IdOrigenCotizacion = (int)EnumOrigenCotizacion.Centro
                        });

                        if (string.IsNullOrEmpty(fileResult?.Base64))
                            await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "Devolución efectuada. Comprobante de pago en proceso de carga. Por favor, consulte nuevamente");
                        else
                        {
                            await JS.InvokeVoidAsync("saveAsFile", $"DEVOL_{pin}.pdf", fileResult?.Base64);
                        }
                    }
                    else if (estadoDevolucion == "En Trámite")
                        await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "La devolución se encuentra en trámite. Por favor, consulte nuevamente");
                    else
                        await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "El pin consultado no tiene un estado de devolución valido");
                }
                else if (tipoDevolucion == "Efectivo")
                {
                    if (estadoDevolucion == "Devolución Efectuada")
                        await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "Devolución realizada en efectivo. Comprobante de pago no disponible");
                    else if (estadoDevolucion == "En Trámite")
                        await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "La devolución se encuentra en trámite. Por favor, consulte nuevamente");
                }
                else
                    await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "El pin consultado no tiene un tipo de devolución valido");
            }
            else if (origenCotizacion == EnumOrigenCotizacion.MiLicencia)
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "Por razones de seguridad, no podemos mostrar el comprobante de pago debido a que contiene información personal del usuario");
            else
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, $"No se puede generar el comprobante para el origen de cotizacion del pin {PagoPinConst.DescripcionCanalVenta[origenCotizacion]}");
        }

        private static TKey GetKeyFromValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue value)
        {
            // Use EqualityComparer<TValue>.Default.Equals to compare values
            return dictionary.FirstOrDefault(pair => EqualityComparer<TValue>.Default.Equals(pair.Value, value)).Key;
        }

        #endregion Methods
    }
}


