using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using OfficeOpenXml;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Pines.Dispersiones
{
    public partial class Dispersiones
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService _MiLicenciaService { get; set; }

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

        [Parameter]
        public EventCallback<FileContentResult> OnDownloadCompleted { get; set; }

        public bool isLoading = true;

        private readonly PaginationState Pagination = new() { ItemsPerPage = 5 };

        //private IQueryable<List<RowList>> Data;
        private GridItemsProvider<RowList>? Data;

        private List<ColumnList> columnas;

        private ConsultaInfoPinEstado DatosFiltros = new();

        private List<ResponseConsultaDispersion> Listado = new();
        private List<ResponseConsultaDispersion> ListadoDescarga = new();
        private GetDataResponseCentro getCentroResponse = new GetDataResponseCentro();
        private string idRuntAuxiliar;

        private Paginas PruebaPaginas = new Paginas();
        //private Paginas PruebaPaginas = new Paginas();

        #endregion Variables

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            getCentroResponse = centroShared.Value.Respuesta;
            await ConsultaInicial();
            isLoading = false;
        }

        private async Task CargarDatagridView(List<ResponseConsultaDispersion> Listado)
        {
            List<RowList> rows = [];

            if (Listado.Any())
            {
                Listado.ForEach(x =>
                {
                    string valorDispersion = "";
                    if (!string.IsNullOrEmpty(x.ValorTotalDispersion))
                    {
                        valorDispersion = Convert.ToDecimal(x.ValorTotalDispersion).FormatAsCurrency();
                    }
                    List<ColumnList> row = new List<ColumnList>
                    {
                            new ColumnList { NameColumn = "IdDispersion", Value = (x.IdDispersion>0 ? x.IdDispersion.ToString():"0")},
                            new ColumnList { NameColumn = "Negocio", Value = (!string.IsNullOrEmpty(x.Negocio)?x.Negocio :"No encontrado") },
                            new ColumnList { NameColumn = "Banco", Value = (!string.IsNullOrEmpty(x.Banco)?x.Banco :"No encontrado") },
                            new ColumnList { NameColumn = "Cuenta" ,Value = (!string.IsNullOrEmpty(x.Cuenta)?x.Cuenta :"No encontrado")},
                            new ColumnList { NameColumn = "Total Dispersión", Value = (!string.IsNullOrEmpty(x.ValorTotalDispersion)?valorDispersion :"0") },
                            new ColumnList { NameColumn = "Agente Dispersión", Value = (!string.IsNullOrEmpty(x.AgenteDispersion)?x.AgenteDispersion :"No encontrado") },
                            new ColumnList { NameColumn = "Detalle de Pago", Value =  (x.IdDispersion>0 ? x.IdDispersion.ToString():"0") },
                        };

                    rows.Add(new RowList { Row = row });
                });
            }
            else
            {
                // Agregar solo los encabezados si la lista es nula o está vacía
                List<ColumnList> headers = PagoPinConst.InicializarColumnasDispersiones();

                rows.Add(new RowList { Row = headers });
            }

			//Data = new[] { rows }.AsQueryable();

			Data = async req =>
			{
				return GridItemsProviderResult.From(
				items: rows,
				totalItemCount: Listado.Count);
			};

			await Task.FromResult(true);
        }

        private async Task DatosFiltrosChanged(ConsultaInfoPinEstado value)
        {
            // Recibe el valor del componente hijo
            DatosFiltros = value;
            if (!string.IsNullOrEmpty(value.IdRunt) && value.FechaInicial != null && value.FechaFinal != null)
            {
                await RealizarConsulta(value.IdRunt, value.FechaInicial, value.FechaFinal,false, esBusquedaManual: true);
            }
            else
            {
                _toastService.ShowWarning("El centro, la fecha inicial y la fecha final son datos necesarios. Por favor llene los datos y vuelva a intentarlo.", "Información");
            }
        }

        private async Task DescargarDatos(ConsultaDescarga consulta)
        {
            if (consulta.IsDownload)
            {
                if (!string.IsNullOrEmpty(consulta.Consulta.IdRunt))
                {
                    await RealizarConsulta(consulta.Consulta.IdRunt, consulta.Consulta.FechaInicial, consulta.Consulta.FechaFinal,consulta.IsDownload);
                    if (ListadoDescarga.Count > 0)
                    {
                        DownloadExcelFile<ResponseConsultaDispersion> DownloadExcelFile = new(JS);

                        byte[] archivoBytes = await DownloadExcelFile.DownloadExcel(Listado, "ConsultaDispersiones");
                        await DownloadExcelFile.DownloadFileFromStream(archivoBytes, "ConsultaDispersiones");
                    }
                }
                else
                {
                    isLoading = false;
                    _toastService.ShowWarning("Debe seleccionar un centro para realizar la descarga.", "Información");
                }
        }
        }

        private async Task DescargarDatosDetallePin(bool isDownload, List<ResponseDetallePin> listadoDetalle)
        {
            if (isDownload && listadoDetalle.Any())
            {
                DownloadExcelFile<ResponseDetallePin> DownloadExcelFile = new(JS);

                byte[] archivoBytes = await DownloadExcelFile.DownloadExcel(listadoDetalle, "Detalle Pago");
                await DownloadExcelFile.DownloadFileFromStream(archivoBytes, $"DetalleDePagoPin{listadoDetalle[0].Pin}");
            }
        }

        private async Task RealizarConsulta(string idRunt, DateTime? fechaInicial, DateTime? fechaFin,bool isDownload, bool esBusquedaManual = false)
        {
            if (!isDownload)
            {
                ConsultaDispersion consulta = new ConsultaDispersion() { IdRunt = idRunt, FechaInicial = fechaInicial, FechaFin = fechaFin };

                var result = await _MiLicenciaService.ConsultarDispersion(consulta);
                if (result != null)
                {
                    if (result.Codigo == 0 && result.Entidad != null)
                    {
                        idRuntAuxiliar = idRunt;

                        Listado = result.Entidad;
                        CargarDatagridView(Listado);
                    }
                    else
                    {
                        Listado = new();
                        CargarDatagridView(Listado);

                        if (esBusquedaManual)
                        {
                            _toastService.ShowInfo(!string.IsNullOrEmpty(result.Respuesta) ? result.Respuesta : "Ha ocurrido un error en la consulta inicial.", "Información");
                        }
                            
                    }
                }
                else
                {
                    Listado = new();

                    if (esBusquedaManual)
                    {
                        _toastService.ShowError("Ha ocurrido un error en la consulta.");
                    }
                }
            }
            else
            {
                ConsultaDispersion consulta = new ConsultaDispersion() { IdRunt = idRunt, FechaInicial = fechaInicial, FechaFin = fechaFin };

                var result = await _MiLicenciaService.ConsultarDispersion(consulta);
                if (result != null)
                {
                    if (result.Codigo == 0 && result.Entidad != null)
                    {
                        idRuntAuxiliar = idRunt;

                        if (result.Entidad.Count > 0)
                        {
                            ListadoDescarga = result.Entidad;
                        }
                        else
                        {
                            _toastService.ShowInfo(!string.IsNullOrEmpty(result.Respuesta) ? result.Respuesta : "No se encontraron datos para la descarga.", "Información");
                        }
                        
                        
                    }
                    else
                    {
                        ListadoDescarga = new();
                        _toastService.ShowInfo(!string.IsNullOrEmpty(result.Respuesta) ? result.Respuesta : "Ha ocurrido un error en la consulta inicial.", "Información");
                    }
                }
                else
                {
                    ListadoDescarga = new();
                    _toastService.ShowError("Ha ocurrido un error en la consulta.");
                }
            }
            // Consulta inicial
            
        }

        private async Task ConsultaInicial()
        {
            try
            {
                // consulta inicial asociada al centro actual que esta logueado y en los ultimos 2 meses.
                string centro = getCentroResponse.CodigoRUNT.ToString();
                //string inicial = "20231201";
                //string final = "20231230";
                DateTime Finicial = DateTime.Now.AddMonths(-2);
                DateTime Ffinal= DateTime.Now;
                //DateTime Finicial = DateTime.ParseExact(inicial, "yyyyMMdd", CultureInfo.InvariantCulture);
                //DateTime Ffinal = DateTime.ParseExact(final, "yyyyMMdd", CultureInfo.InvariantCulture);
                //DateTime Finicial = DateTime.Now.AddMonths(-2);
                //DateTime Ffinal= DateTime.Now;
                columnas = PagoPinConst.InicializarColumnasDispersiones();
                await RealizarConsulta(centro, Finicial, Ffinal,false, esBusquedaManual:false);
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
            }
        }

        private Stream GetFileStream(byte[] archivoBytes)
        {
            var fileStream = new MemoryStream(archivoBytes);

            return fileStream;
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
            }
        }

        private async Task BotonDescargaActived(string value)
        {
            await DescargaDetalle(value);
        }

        private async Task DescargaDetalle(string idDispersion)
        {
            ConsultaDetallePin consulta = new ConsultaDetallePin { IdRunt = idRuntAuxiliar, IdDispersion = idDispersion };
            var result = await _MiLicenciaService.ConsultarDetallePin(consulta);
            if (result != null)
            {
                if (result.Codigo == 0 && result.Entidad.Any())
                {
                    await DescargarDatosDetallePin(true, result.Entidad);
                }
                else
                {
                    _toastService.ShowInfo(result.Respuesta);
                }
            }
            else
            {
                _toastService.ShowError("Ha ocurrido un error en la consulta.");
            }
        }

        //private async Task ArmadoPaginador()
        //{
        //    PruebaPaginas.PaginaActual = 1;
        //    PruebaPaginas.TotalPaginas = 10;
        //    PruebaPaginas.PaginaInicial = 1;
        //    PruebaPaginas.PaginaFinal = 10;
        //    PruebaPaginas.RegistrosPorPagina = 30;
        //    PruebaPaginas.TotalRegistros = 300;
        //    PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
        //    PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
        //}
        //private async Task SetPaginadorInicial(List<ResponseInfoPinEstado> listado,Paginas paginas,bool cambioPagina)
        //{
        //    if (listado.Any())
        //    {
        //        if (!cambioPagina)
        //        {
        //            // Solo para paso inicial de llenado de paginador
        //            int cantidadPaginas = listado.FirstOrDefault().TotalRegistros / 30;
        //            PruebaPaginas.PaginaActual = 1;
        //            PruebaPaginas.TotalPaginas = cantidadPaginas;
        //            PruebaPaginas.PaginaInicial = 1;
        //            PruebaPaginas.PaginaFinal = cantidadPaginas;
        //            PruebaPaginas.RegistrosPorPagina = 30;
        //            PruebaPaginas.TotalRegistros = listado.FirstOrDefault().TotalRegistros;
        //            PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
        //            PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
        //        }
        //        else
        //        {
        //            // Se dispara solo en casos de cambio de pagina desde quickgrid
        //            int cantidadPaginas = listado.FirstOrDefault().TotalRegistros / paginas.RegistrosPorPagina;
        //            PruebaPaginas.PaginaActual = paginas.PaginaActual;
        //            PruebaPaginas.TotalPaginas = cantidadPaginas;
        //            PruebaPaginas.PaginaInicial = 1;
        //            PruebaPaginas.PaginaFinal = cantidadPaginas;
        //            PruebaPaginas.RegistrosPorPagina = paginas.RegistrosPorPagina;
        //            PruebaPaginas.TotalRegistros = listado.FirstOrDefault().TotalRegistros;
        //            PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
        //            PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);

        //        }
        //    }
        //}

        //private async Task paginaChanged(Paginas paginas)
        //{
        //    var paginado = paginas;
        //    PruebaPaginas = paginas;
        //    RegistroPagina(PruebaPaginas.PaginaActual, PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
        //    // Realizar la consulta nuevamente con los parametros
        //    //
        //    //SetPaginadorInicial(Listado, PruebaPaginas, true);
        //SetPaginadorInicial(Listado, PruebaPaginas, true);
        //    //
        //}
        //private async Task RegistroPagina(int pagina, int totalRegistrosPagina, int totalRegistros)
        //{
        //    PruebaPaginas.RegistroInicialPagina = (pagina - 1) * totalRegistrosPagina + 1;
        //    PruebaPaginas.RegistroFinalPagina = Math.Min(pagina * totalRegistrosPagina, totalRegistros);
        //}

        #endregion Methods
    }
}