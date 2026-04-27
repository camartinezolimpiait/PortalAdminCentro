using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.Pago.Wompi;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data.Pines;
using System.Linq;
using Microsoft.AspNetCore.Components.QuickGrid;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Globalization;
using Blazored.Toast.Services;
using portalAdministrativoSISEC.Util.Extension;

namespace portalAdministrativoSISEC.Pages.Pines.Usados
{
    public partial class Usados
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IToastService _toastService { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        public bool isLoading = true;

        private readonly PaginationState Pagination = new() { ItemsPerPage = 10 };

        //private IQueryable<List<RowList>> Data;

        private GridItemsProvider<RowList>? Data;

        private List<ColumnList> columnas;

        private ConsultaInfoPinEstado DatosFiltros = new();

        private List<ResponseInfoPinEstado> Listado = new();

        private List<ResponseInfoPinEstado> ListadoDescarga = new();

        private GetDataResponseCentro getCentroResponse = new GetDataResponseCentro();

        private string idRuntAuxiliar;

        private Paginas PruebaPaginas = new Paginas();

        [Parameter]
        public EventCallback<FileContentResult> OnDownloadCompleted { get; set; }

        #endregion Variables

        #region Methods

        public byte[] DownloadExcel(List<ResponseInfoPinEstadoUsadosDTO> Listado)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PinesUsados");

                // Escribir encabezados de columnas
                int columnIndex = 1;
                foreach (var property in typeof(ResponseInfoPinEstadoUsadosDTO).GetProperties())
                {
                    worksheet.Cells[1, columnIndex].Value = property.Name;
                    columnIndex++;
                }

                // Escribir datos de la lista en el archivo Excel
                int rowIndex = 2;
                foreach (var item in Listado)
                {
                    columnIndex = 1;
                    foreach (var property in typeof(ResponseInfoPinEstadoUsadosDTO).GetProperties())
                    {
                        var value = property.GetValue(item);
                        worksheet.Cells[rowIndex, columnIndex].Value = value != null ? value.ToString() : string.Empty;
                        columnIndex++;
                    }
                    rowIndex++;
                }

                // Convertir el paquete a un array de bytes
                byte[] excelBytes = package.GetAsByteArray();
                return excelBytes;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            getCentroResponse = centroShared.Value.Respuesta;
            idRuntAuxiliar = getCentroResponse.CodigoRUNT.ToString();
            await ConsultaInicial();
            isLoading = false;
        }

        private async Task ArmadoPaginador()
        {
            int total = 5;
            if (Listado.Count > 0) { total = Listado[0].TotalRegistros; }
            PruebaPaginas.PaginaActual = DatosFiltros.NumPagina.Value;
            PruebaPaginas.TotalPaginas = DatosFiltros.NumRegistros.Value;
            PruebaPaginas.PaginaInicial = 1;
            PruebaPaginas.PaginaFinal = 10;
            PruebaPaginas.RegistrosPorPagina = DatosFiltros.NumRegistros.Value;
            PruebaPaginas.TotalRegistros = total;
            PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
            PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
        }

        private async Task CargarDatagridView(List<ResponseInfoPinEstado> Listado)
        {
            List<RowList> rows = [];

            if (Listado != null)
            {
                Listado.ForEach(x =>
                {
                    string ValorTransaccion = Convert.ToDecimal(x.ValorTransaccion).FormatAsCurrency();
                    string ValorActor = Convert.ToDecimal(x.ValorActor).FormatAsCurrency();
                    string ValorAns = Convert.ToDecimal(x.ValorAns).FormatAsCurrency();
                    string ValorAliado = Convert.ToDecimal(x.ValorAliado).FormatAsCurrency();
                    string ValosSicov = Convert.ToDecimal(x.ValosSicov).FormatAsCurrency();
                    string ValorDispersado = Convert.ToDecimal(x.ValorDispersado).FormatAsCurrency();

                    int.TryParse(x.Cuotas, out int cuotas);

                    List<ColumnList> row =
                    [
                        new() { NameColumn = "Canal de Venta", Value = (!string.IsNullOrEmpty(x.CanalVenta.ToString()) ? PagoPinConst.DescripcionCanalVenta[(EnumOrigenCotizacion)x.CanalVenta]: "No encontrado")},
                        new() { NameColumn = "Pin", Value = (!string.IsNullOrEmpty(x.Pin) ? x.Pin : "No encontrado")},
                        new() { NameColumn = "Código de transacción", Value = (!string.IsNullOrEmpty(x.NUTVenta) ? x.NUTVenta : "No aplica") },
                        new() { NameColumn = "Tipo Documento", Value = (!string.IsNullOrEmpty(x.TipoIdentificacion) ? x.TipoIdentificacion : "No encontrado") },
                        new() { NameColumn = "Número Documento", Value = (!string.IsNullOrEmpty(x.NumeroIdentificacion) ? x.NumeroIdentificacion : "No encontrado") },
                        new() { NameColumn = "Valor Pin", Value = (!string.IsNullOrEmpty(x.ValorTransaccion.ToString())? ValorTransaccion : "No encontrado") },
                        new() { NameColumn = "Valor Actor", Value = (!string.IsNullOrEmpty(x.ValorActor.ToString()) ? ValorActor : "No encontrado") },
                        new() { NameColumn = "Valor ANSV", Value = (!string.IsNullOrEmpty(x.ValorAns.ToString()) ? ValorAns : "No encontrado") },
                        new() { NameColumn = "Valor Aliado", Value = (!string.IsNullOrEmpty(x.ValorAliado.ToString()) ? ValorAliado : "No encontrado") },
                        new() { NameColumn = "Valor Sicov", Value = (!string.IsNullOrEmpty(x.ValosSicov.ToString()) ? ValosSicov : "No encontrado") },
                        new() { NameColumn = "Fecha Operación", Value = (!string.IsNullOrEmpty(x.FechaRegistro.ToString()) ? x.FechaRegistro?.ToString("dd/MM/yyyy") : "No encontrado") },
                        new() { NameColumn = "IdDispersión", Value = (x.IdDispersion > 0 ? x.IdDispersion.ToString() : "No encontrado")},
                        new() { NameColumn = "Fecha Dispersión", Value = (!string.IsNullOrEmpty(x.FechaDispersion) ? TransformarFecha(x.FechaDispersion) : "No encontrado") },
                        new() { NameColumn = "Banco Dispersión", Value = (!string.IsNullOrEmpty(x.Banco) ? x.Banco : "No encontrado") },
                        new() { NameColumn = "Cuenta Dispersión", Value = (!string.IsNullOrEmpty(x.CtaDispersion) ? x.CtaDispersion : "No encontrado") },
                        new() { NameColumn = "Valor Dispersión", Value = (!string.IsNullOrEmpty(x.ValorDispersado.ToString()) ? ValorDispersado : "No encontrado") },
                        new() { NameColumn = "Agente Dispersión", Value = (!string.IsNullOrEmpty(x.AgenteDispersion) ? x.AgenteDispersion : "No encontrado") },
                        new() { NameColumn = "Razón Social", Value = (!string.IsNullOrEmpty(x.RazonSocial) ? x.RazonSocial : "No encontrado") },
                        new() { NameColumn = "Tipo Pin", Value = (!string.IsNullOrEmpty(x.TipoPin) ? x.TipoPin : "No encontrado") },
                        new() { NameColumn = "Pines Asociados", Value = (x.PagosRealizados > 0 || cuotas > 1) ? x.Pin : "0" },
                        new() { NameColumn = "Convenio Empresa", Value = x.ConvenioEmpresa ?? "No aplica" },
                        new() { NameColumn = "Empresa", Value = x.Empresa ?? "No aplica" },
                    ];

                    rows.Add(new RowList
                    {
                        Row = row
                    });
                });
            }
            else
            {
                // Agregar solo los encabezados si la lista es nula o está vacía
                List<ColumnList> headers = PagoPinConst.InicializarColumnasUsados();

                rows.Add(new RowList { Row = headers });
            }

            //Data = new[] { rows }.AsQueryable();

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
            DatosFiltros = value;
            await RealizarConsulta(value);
        }

        private async Task DescargarDatos(ConsultaDescarga consulta)
        {
            if (consulta.IsDownload)
            {
                await RealizarConsultaDescarga(consulta.Consulta);

                if (ListadoDescarga.Any())
                {
                    //await RealizarConsulta(DatosFiltros);
                    var listadoDescarga = await ArmadoListadoDescarga(ListadoDescarga);
                    byte[] archivoBytes = DownloadExcel(listadoDescarga);
                    await DownloadFileFromStream(archivoBytes, "ConsultaPinesUsados");
                }
            }
        }

        private async Task RealizarConsultaDescarga(ConsultaInfoPinEstado value)
        {
            isLoading = true;
            if (!string.IsNullOrEmpty(value.IdRunt))
            {
                value.Opcion = 2;
                value.NumPagina = 1;
                value.NumRegistros = 1;
                if (!string.IsNullOrEmpty(value.IdRunt)) { idRuntAuxiliar = value.IdRunt; }
                value.IdRunt = idRuntAuxiliar;
                value.Estado = (int)EnumTipoConsultaPines.Usados;
                if (value.NumPagina == null) { value.NumPagina = 1; }
                if (value.NumRegistros == null) { value.NumRegistros = 5; }

                var result = await MiLicenciaService.ConsultaInfoPinesPorEstado(value);
                if (result != null)
                {
                    isLoading = false;
                    if (result.Codigo == 0 && result.Entidad != null)//(result.Entidad != null)
                    {
                        ListadoDescarga = result.Entidad;
                        DatosFiltros.Opcion = 1;
                        DatosFiltros.NumPagina = 1;
                        DatosFiltros.NumRegistros = 5;
                    }
                    else
                    {
                        ListadoDescarga = new();
                        DatosFiltros.Opcion = 1;
                        DatosFiltros.NumPagina = 1;
                        DatosFiltros.NumRegistros = 5;
                        _toastService.ShowInfo(result.Respuesta, "Información");
                    }
                }
                else
                {
                    isLoading = false;
                    DatosFiltros.Opcion = 1;
                    DatosFiltros.NumPagina = 1;
                    DatosFiltros.NumRegistros = 5;
                    ListadoDescarga = new();
                    _toastService.ShowError("Ha ocurrido un error en la consulta.");
                }
            }
            else
            {
                isLoading = false;
                _toastService.ShowWarning("Debe seleccionar un centro para realizar la descarga.", "Advertencia");
            }
        }

        private async Task DownloadFileFromStream(byte[] archivo, string nombrearchivo)
        {
            try
            {
                var fileStream = GetFileStream(archivo);
                var fileName = nombrearchivo + ".xlsx";

                using var streamRef = new DotNetStreamReference(stream: fileStream);

                await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private Stream GetFileStream(byte[] archivoBytes)
        {
            var fileStream = new MemoryStream(archivoBytes);

            return fileStream;
        }

        private async Task RealizarConsulta(ConsultaInfoPinEstado value)
        {
            // Consulta inicial
            if (!string.IsNullOrEmpty(value.IdRunt))
            {
                if (!string.IsNullOrEmpty(value.IdRunt)) { idRuntAuxiliar = value.IdRunt; }
                value.IdRunt = idRuntAuxiliar;
                value.Estado = (int)EnumTipoConsultaPines.Usados;
                if (value.NumPagina == null) { value.NumPagina = 1; }
                if (value.NumRegistros == null) { value.NumRegistros = 5; }

                var result = await MiLicenciaService.ConsultaInfoPinesPorEstado(value);
                if (result != null)
                {
                    isLoading = false;
                    if (result.Codigo == 0 && result.Entidad != null)//(result.Entidad != null)
                    {
                        Listado = result.Entidad;
                        await CargarDatagridView(Listado);
                    }
                    else
                    {
                        Listado = new();
                        await CargarDatagridView(Listado);

                        _toastService.ShowInfo(result.Respuesta, "Información");
                    }
                }
                else
                {
                    isLoading = false;
                    Listado = new();
                    await CargarDatagridView(Listado);
                    _toastService.ShowError("Ha ocurrido un error en la consulta.");
                }
                await SetPaginadorInicial(Listado, PruebaPaginas, false);
            }
            else
            {
                if (!string.IsNullOrEmpty(value.IdRunt))
                {
                    idRuntAuxiliar = value.IdRunt;
                    value.IdRunt = idRuntAuxiliar;
                }

                isLoading = false;
                Listado = new();
                await CargarDatagridView(Listado);
                _toastService.ShowWarning("Debe seleccionar el centro para realizar la consulta.", "Información");
            }
        }

        private async Task ConsultaInicial()
        {
            try
            {
                string centro = idRuntAuxiliar;
                DateTime Finicial = DateTime.Now.AddMonths(-3);
                DateTime Ffinal = DateTime.Now;

                ConsultaInfoPinEstado consulta = new ConsultaInfoPinEstado()
                {
                    IdRunt = centro,
                    FechaInicial = Finicial,
                    FechaFinal = Ffinal,
                    Estado = (int)EnumTipoConsultaPines.Usados,
                    NumPagina = 1,
                    NumRegistros = 5,
                    IdTipoDocumento = 0,
                    Documento = null,
                    Pin = null,
                    IdAgenteDispersion = 0,
                    Canal = 0,
                    Opcion = 1
                };
                DatosFiltros = consulta;

                columnas = PagoPinConst.InicializarColumnasUsados();

                await RealizarConsulta(consulta);
                await SetPaginadorInicial(Listado, PruebaPaginas, false);
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
            }
        }

        private async Task paginaChanged(Paginas paginas)
        {
            var paginado = paginas;
            PruebaPaginas = paginas;
            await RegistroPagina(PruebaPaginas.PaginaActual, PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
        }

        private async Task RegistroPagina(int pagina, int totalRegistrosPagina, int totalRegistros)
        {
            DatosFiltros.Opcion = 1;
            DatosFiltros.NumPagina = pagina;
            DatosFiltros.NumRegistros = totalRegistrosPagina;
            await RealizarConsulta(DatosFiltros);
            PruebaPaginas.RegistroInicialPagina = (pagina - 1) * totalRegistrosPagina + 1;
            PruebaPaginas.RegistroFinalPagina = Math.Min(pagina * totalRegistrosPagina, totalRegistros);
            PruebaPaginas.RegistrosPorPagina = totalRegistrosPagina;
            PruebaPaginas.PaginaActual = pagina;
            await SetPaginadorInicial(Listado, PruebaPaginas, true);
            StateHasChanged();
        }

        private async Task SetPaginadorInicial(List<ResponseInfoPinEstado> listado, Paginas paginas, bool cambioPagina)
        {
            if (listado.Any())
            {
                if (!cambioPagina)
                {
                    // Solo para paso inicial de llenado de paginador
                    double cantidadPaginasDecimal = (double)listado.FirstOrDefault().TotalRegistros / 5;
                    int cantidadPaginas = (int)Math.Ceiling(cantidadPaginasDecimal);
                    PruebaPaginas.PaginaActual = 1;
                    PruebaPaginas.TotalPaginas = cantidadPaginas;
                    PruebaPaginas.PaginaInicial = 1;
                    PruebaPaginas.PaginaFinal = cantidadPaginas;
                    PruebaPaginas.RegistrosPorPagina = 5;
                    Pagination.ItemsPerPage = paginas.RegistrosPorPagina;
                    PruebaPaginas.TotalRegistros = listado.FirstOrDefault().TotalRegistros;
                    PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
                    PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
                }
                else
                {
                    // Se dispara solo en casos de cambio de pagina desde quickgrid
                    double cantidadPaginasDecimal = (double)listado.FirstOrDefault().TotalRegistros / paginas.RegistrosPorPagina;
                    int cantidadPaginas = (int)Math.Ceiling(cantidadPaginasDecimal);
                    PruebaPaginas.PaginaActual = paginas.PaginaActual;
                    PruebaPaginas.TotalPaginas = cantidadPaginas;
                    PruebaPaginas.PaginaInicial = 1;
                    PruebaPaginas.PaginaFinal = cantidadPaginas;
                    PruebaPaginas.RegistrosPorPagina = paginas.RegistrosPorPagina;
                    PruebaPaginas.TotalRegistros = listado.FirstOrDefault().TotalRegistros;
                    PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
                    PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
                    Pagination.ItemsPerPage = paginas.RegistrosPorPagina;
                    await Pagination.SetCurrentPageIndexAsync(paginas.PaginaActual);
                }
            }
        }

        private string TransformarFecha(string fechaString)
        {
            string fechaFormateada = "";
            if (DateTime.TryParse(fechaString, out DateTime fecha))
            {
                // La conversión fue exitosa, puedes usar 'fecha'
                fechaFormateada = fecha.ToString("dd/MM/yyyy");
            }
            else
            {
                // La conversión falló
                fechaFormateada = "Formato de fecha inválido";
            }
            return fechaFormateada;
        }

        private async Task<List<ResponseInfoPinEstadoUsadosDTO>> ArmadoListadoDescarga(List<ResponseInfoPinEstado> Listado)
        {
            List<ResponseInfoPinEstadoUsadosDTO> lista = new();

            if (Listado != null)
            {
                Listado.ForEach(x =>
                {
                    ResponseInfoPinEstadoUsadosDTO item = new();
                    item.CanalVenta = (!string.IsNullOrEmpty(x.CanalVenta.ToString()) ? PagoPinConst.DescripcionCanalVenta[(EnumOrigenCotizacion)x.CanalVenta] : "No encontrado");
                    item.Pin = (!string.IsNullOrEmpty(x.Pin) ? x.Pin : "No encontrado");
                    item.NUTVenta = (!string.IsNullOrEmpty(x.NUTVenta) ? x.NUTVenta : "No aplica");
                    item.TipoIdentificacion = (!string.IsNullOrEmpty(x.TipoIdentificacion) ? x.TipoIdentificacion : "No encontrado");
                    item.NumeroIdentificacion = (!string.IsNullOrEmpty(x.NumeroIdentificacion) ? x.NumeroIdentificacion : "No encontrado");
                    item.ValorTransaccion = (!string.IsNullOrEmpty(x.ValorTransaccion.ToString()) ? x.ValorTransaccion : 0);
                    item.ValorActor = (!string.IsNullOrEmpty(x.ValorActor.ToString()) ? x.ValorActor : 0);
                    item.ValorAns = (!string.IsNullOrEmpty(x.ValorAns.ToString()) ? x.ValorAns : 0);
                    item.ValorAliado = (!string.IsNullOrEmpty(x.ValorAliado.ToString()) ? x.ValorAliado : 0);
                    item.ValosSicov = (!string.IsNullOrEmpty(x.ValosSicov.ToString()) ? x.ValosSicov : 0);
                    item.FechaOperacion = (!string.IsNullOrEmpty(x.FechaRegistro.ToString()) ? x.FechaRegistro?.ToString("dd/MM/yyyy") : "No encontrado");
                    item.FechaDispersion = (!string.IsNullOrEmpty(x.FechaDispersion) ? TransformarFecha(x.FechaDispersion) : "No encontrado");
                    item.Banco = (!string.IsNullOrEmpty(x.Banco) ? x.Banco : "No encontrado");
                    item.CtaDispersion = (!string.IsNullOrEmpty(x.CtaDispersion) ? x.CtaDispersion : "No encontrado");
                    item.ValorDispersado = (!string.IsNullOrEmpty(x.ValorDispersado.ToString()) ? x.ValorDispersado : 0);
                    item.AgenteDispersion = (!string.IsNullOrEmpty(x.AgenteDispersion) ? x.AgenteDispersion : "No encontrado");
                    item.RazonSocial = (!string.IsNullOrEmpty(x.RazonSocial) ? x.RazonSocial : "No encontrado");
                    item.TipoPin = (!string.IsNullOrEmpty(x.TipoPin) ? x.TipoPin : "No encontrado");
                    item.ConvenioEmpresa = x.ConvenioEmpresa;
                    item.Empresa = x.Empresa;

                    lista.Add(item);
                });
            }
            await Task.FromResult(true);
            return lista;
        }

        #endregion Methods
    }
}

