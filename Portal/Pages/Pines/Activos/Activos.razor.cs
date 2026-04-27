using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.JSInterop;
using OfficeOpenXml;
using ClosedXML.Excel;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.Common;
using portalAdministrativoSISEC.Pages.Pines.Common;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace portalAdministrativoSISEC.Pages.Pines.Activos
{
    public partial class Activos
    {
        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
        private IToastService _toastService { get; set; }
        [Inject] IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public string estado { get; set; }

        QuickGrid<ResponseInfoPinEstado> grid;
        GridItemsProvider<ResponseInfoPinEstado> pinesProvider;

        private ApplicationShared applicationShared = new ApplicationShared();
        private ModalPinesAsociados? modalPinesRef;

        PaginationState pagination = new PaginationState { ItemsPerPage = 10 };

        List<TipoDocumentoPtesaDTO> ListaDocumentos = new();
        public List<Centro> ListaCentros { get; set; } = new();
        private GetDataResponseCentro getCentroResponse = new GetDataResponseCentro();
        private List<string> ListaEmpresas = new();

        // Inicio código generado por GitHub Copilot
        private readonly Models.ActivosFiltrosModel FiltrosModel = new()
        {
            CanalVenta = 0,
            EstadoPin = (int)EnumTipoConsultaPines.Activos
        };

        private EditContext FiltrosEditContext;
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        private bool SinResultados { get; set; } = true;
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        private bool HaHechoPrimeraConsulta { get; set; }
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        private bool DebeRefrescarGrid;
        // Fin código generado por GitHub Copilot

        // Inicio refactorización/optimización por GitHub Copilot
        private string? filtroCentroIdRunt = null;
        // Fin refactorización/optimización por GitHub Copilot
        DateTime filtroFechaInicial = DateTime.Today.AddDays(-14);
        DateTime filtroFechaFinal = DateTime.Today;

        // Inicio código generado por GitHub Copilot
        private DateTime GetFechaFinalParaConsulta()
        {
            // `InputDate` solo captura fecha (hora 00:00:00). Para evitar excluir registros de "hoy",
            // cuando el usuario selecciona la fecha actual se usa la hora actual del servidor.
            // Para otras fechas, se usa el final del día para incluir todo el rango.
            var hoy = DateTime.Today;
            if (filtroFechaFinal.Date == hoy)
            {
                // Si la fecha final es hoy, se amplía el rango sumando 1 día.
                return filtroFechaFinal.Date.AddDays(2).AddTicks(-1);
            }

            return filtroFechaFinal.Date.AddDays(1).AddTicks(-1);
        }
        // Fin código generado por GitHub Copilot

        // Método generado por GitHub Copilot
        private async Task ExportarDatosAsync()
        {
            // Método generado por GitHub Copilot
            // Inicio refactorización por GitHub Copilot
            try
            {
                var consulta = new ConsultaInfoPinEstado
                {
                    Canal = FiltrosModel.CanalVenta,
                    IdTipoDocumento = int.TryParse(FiltrosModel.TipoDocumento, out var idTipoDoc) ? idTipoDoc : (int?)null,
                    Documento = string.IsNullOrWhiteSpace(FiltrosModel.Documento) ? null : FiltrosModel.Documento,
                    Estado = FiltrosModel.EstadoPin,
                    FechaInicial = filtroFechaInicial,
                    FechaFinal = GetFechaFinalParaConsulta(),
                    IdAgenteDispersion = FiltrosModel.AliadoRecaudo ?? 0,
                    IdCentro = null,
                    IdRunt = string.IsNullOrWhiteSpace(filtroCentroIdRunt) ? applicationShared.IdRunt : filtroCentroIdRunt,
                    NumPagina = 1,
                    NumRegistros = int.MaxValue,
                    Opcion = 1,
                    Pin = FiltrosModel.Pin
                };

                ResponseDTO<List<ResponseInfoPinEstado>> result = null;
                switch (estado)
                {
                    case nameof(EnumTipoConsultaPines.Activos):
                    case nameof(EnumTipoConsultaPines.Usados):
                        result = await MiLicenciaService.ConsultaInfoPinesPorEstado(consulta);
                        break;
                    case nameof(EnumTipoConsultaPines.Devoluciones):
                        ResponseDTO<List<ResponseConsultaDevolucion>> result2 = await MiLicenciaService.ConsultaDevolucionesPines(consulta);
                        if (result2 != null && result2.Entidad != null)
                        {
                            var mappedList = result2.Entidad.Select(d => new ResponseInfoPinEstado
                            {
                                Pin = d.Pin,
                                // Inicio código generado por GitHub Copilot
                                // FechaRegistro se usa como "Fecha de compra" en la exportación cuando esté disponible.
                                FechaRegistro = d.FechaRegistro,
                                // Fin código generado por GitHub Copilot
                                ValorTransaccion = float.Parse(d.ValorDevolver),
                                Estado = "Devuelto",
                                NUTVenta = d.NUTVenta,
                                // Inicio código generado por GitHub Copilot
                                RazonSocial = d.RazonSocial,
                                NombreCompleto = d.NombreCompleto,
                                // Fin código generado por GitHub Copilot
                                AgenteDispersion = d.AgenteDispersion,
                                // Inicio código generado por GitHub Copilot
                                // El endpoint de devoluciones puede no retornar CanalVenta. Se usa el filtro actual como fallback.
                                CanalVenta = d.CanalVenta ?? FiltrosModel.CanalVenta,
                                // Fin código generado por GitHub Copilot
                                NumeroIdentificacion = d.NumeroIdentificacion,
                                TipoIdentificacion = d.IdTipoIdentificacion,
                                // Inicio código generado por GitHub Copilot
                                FechaDevolucion = d.FechaDevolucion,
                                NovedadDevolucion = d.NovedadDevolucion,
                                TipoDevolucion = d.TipoDevolucion,
                                ConvenioEmpresa=d.ConvenioEmpresa,
                                Empresa=d.Empresa
                                // Fin código generado por GitHub Copilot
                            }).ToList();
                            result = new ResponseDTO<List<ResponseInfoPinEstado>>
                            {
                                Entidad = mappedList,
                                Codigo = result2.Codigo
                            };
                        }
                        break;
                }

                if (result == null || result.Entidad == null || !result.Entidad.Any())
                {
                    _toastService.ShowWarning("No hay datos para exportar.");
                    return;
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Pines");

                // Encabezados base (todas las columnas del grid principal)
                var col = 1;
                worksheet.Cell(1, col++).Value = "Canal de venta";
                worksheet.Cell(1, col++).Value = "Número de PIN";
                worksheet.Cell(1, col++).Value = "Modalidad";

                // Inicio código generado por GitHub Copilot
                // Columnas requeridas por el PBI para el archivo de devoluciones
                var incluirColumnasDevoluciones = estado == nameof(EnumTipoConsultaPines.Devoluciones);
                if (incluirColumnasDevoluciones)
                {
                    worksheet.Cell(1, col++).Value = "Fecha de compra";
                    worksheet.Cell(1, col++).Value = "Estado de devolución";
                    worksheet.Cell(1, col++).Value = "Fecha";
                    worksheet.Cell(1, col++).Value = "Tipo";
                    worksheet.Cell(1, col++).Value = "Novedad";
                }
                // Fin código generado por GitHub Copilot

                // Columnas adicionales: desglose del pago (solo NO devoluciones)
                var incluirDesglosePago = estado != nameof(EnumTipoConsultaPines.Devoluciones);

                if (incluirDesglosePago)
                {
                    worksheet.Cell(1, col++).Value = "Valor ANSV";
                    worksheet.Cell(1, col++).Value = "Valor SICOV";
                    worksheet.Cell(1, col++).Value = "Valor Aliado";
                }

                // Valor del centro y Valor total (en este orden)
                var incluirValorCentroGrid = estado != nameof(EnumTipoConsultaPines.Devoluciones);
                if (incluirValorCentroGrid)
                {
                    worksheet.Cell(1, col++).Value = "Valor del centro";
                }

                worksheet.Cell(1, col++).Value = estado == nameof(EnumTipoConsultaPines.Devoluciones) ? "Valor total a devolver" : "Valor total";
                worksheet.Cell(1, col++).Value = "Tipo Documento";
                worksheet.Cell(1, col++).Value = "Número de Documento";
                worksheet.Cell(1, col++).Value = "Nombre completo";

                worksheet.Cell(1, col++).Value = estado switch
                {
                    nameof(EnumTipoConsultaPines.Activos) => "Fecha de compra",
                    nameof(EnumTipoConsultaPines.Usados) => "Fecha de uso",
                    nameof(EnumTipoConsultaPines.Devoluciones) => "Fecha de devolución",
                    _ => "Fecha"
                };

                worksheet.Cell(1, col++).Value = "Código de transacción";
                worksheet.Cell(1, col++).Value = "Centro de compra";
                worksheet.Cell(1, col++).Value = "Aliado de recaudo";
                worksheet.Cell(1, col++).Value = "Convenio Empresa";
                worksheet.Cell(1, col++).Value = "Empresa";

                static string Modalidad(ResponseInfoPinEstado item)
                    => Convert.ToInt32(item.Cuotas) > 1 ? "A Cuotas" : "Único";

                static string CanalVentaNombre(ResponseInfoPinEstado item)
                    => System.Enum.IsDefined(typeof(EnumOrigenCotizacion), item.CanalVenta)
                        ? ((EnumOrigenCotizacion)item.CanalVenta).ToString()
                        : "Desconocido";

                static string? FechaFormateada(ResponseInfoPinEstado item, string estadoActual)
                {
                    DateTime? f = estadoActual switch
                    {
                        nameof(EnumTipoConsultaPines.Devoluciones) => item.FechaDevolucion,
                        _ => item.FechaRegistro
                    };

                    return f.HasValue
                        ? f.Value.ToString("dd/MM/yyyy - h:mm:ss tt", CultureInfo.InvariantCulture).ToLowerInvariant()
                        : (estadoActual == nameof(EnumTipoConsultaPines.Devoluciones) ? "-" : "Sin fecha");
                }

                int row = 2;
                foreach (var item in result.Entidad)
                {
                    var c = 1;
                    worksheet.Cell(row, c++).Value = CanalVentaNombre(item);
                    worksheet.Cell(row, c++).Value = item.Pin;
                    worksheet.Cell(row, c++).Value = Modalidad(item);

                    // Inicio código generado por GitHub Copilot
                    if (incluirColumnasDevoluciones)
                    {
                        // En Devoluciones el backend no siempre entrega la fecha de compra; se mantiene el campo y se llena si está disponible.
                        worksheet.Cell(row, c++).Value = item.FechaRegistro?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "-";
                        worksheet.Cell(row, c++).Value = "Devuelto";
                        worksheet.Cell(row, c++).Value = item.FechaDevolucion?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "-";
                        worksheet.Cell(row, c++).Value = item.TipoDevolucion ?? "-";
                        worksheet.Cell(row, c++).Value = item.NovedadDevolucion ?? "-";
                    }
                    // Fin código generado por GitHub Copilot

                    if (incluirDesglosePago)
                    {
                        worksheet.Cell(row, c++).Value = item.ValorAns;
                        worksheet.Cell(row, c++).Value = item.ValosSicov;
                        worksheet.Cell(row, c++).Value = item.ValorAliado;
                    }

                    if (incluirValorCentroGrid)
                    {
                        worksheet.Cell(row, c++).Value = item.ValorActor;
                    }

                    worksheet.Cell(row, c++).Value = item.ValorTransaccion;

                    worksheet.Cell(row, c++).Value = item.TipoIdentificacion;
                    worksheet.Cell(row, c++).Value = item.NumeroIdentificacion;
                    worksheet.Cell(row, c++).Value = item.NombreCompleto;
                    worksheet.Cell(row, c++).Value = FechaFormateada(item, estado);
                    worksheet.Cell(row, c++).Value = item.NUTVenta;
                    worksheet.Cell(row, c++).Value = item.RazonSocial;
                    worksheet.Cell(row, c++).Value = item.AgenteDispersion;
                    worksheet.Cell(row, c++).Value = item.ConvenioEmpresa;
                    worksheet.Cell(row, c++).Value = item.Empresa;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var ms = new MemoryStream();
                workbook.SaveAs(ms);
                var excelBytes = ms.ToArray();
                var fileName = $"Pines_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                await JS.InvokeVoidAsync("BlazorDownloadFile", fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelBytes);

                // Mostrar el modal de éxito usando la nueva función JS
                await JS.InvokeVoidAsync("mostrarModalExportacion");
            }
            catch (Exception ex)
            {
                // Inicio código generado por GitHub Copilot
                _toastService.ShowError($"Ocurrió un error al exportar los datos: {ex.Message}");
                // Fin código generado por GitHub Copilot
            }
            // Fin refactorización por GitHub Copilot
        }


        protected override void OnParametersSet()
        {
            // Si el parámetro existe y es un número válido
            if (!string.IsNullOrEmpty(estado) && System.Enum.TryParse<EnumTipoConsultaPines>(estado, ignoreCase: true, out var estadoEnum))
            {
                FiltrosModel.EstadoPin = (int)estadoEnum;
            }
            else
            {
                // Valor por defecto si no es válido
                FiltrosModel.EstadoPin = (int)EnumTipoConsultaPines.Activos;
            }

            DebeRefrescarGrid = true;
        }
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (DebeRefrescarGrid && grid is not null)
            {
                DebeRefrescarGrid = false;
                await FilterChangedAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        // Fin código generado por GitHub Copilot

        protected override async Task OnInitializedAsync()
        {
            // Inicio refactorización/optimización por GitHub Copilot
            try
            {
                // Inicio código generado por GitHub Copilot
                FiltrosEditContext = new EditContext(FiltrosModel);
                // Fin código generado por GitHub Copilot

                pagination.TotalItemCountChanged += Pagination_TotalItemCountChanged;

                var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
                getCentroResponse = centroShared.Value.Respuesta;

                var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
                applicationShared = result.Value;

                ListaDocumentos = await MiLicenciaService.ObtenerTipoDocumentos();
                ProtectedBrowserStorageResult<ApplicationSevice> menuServiceShared = await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService");

                ConsultaCentroPorComercio consultaCentro = new ConsultaCentroPorComercio() { IdComercio = getCentroResponse.IdComercio, ConsultaCentro = new ConsultaCentroPorId() { Plataforma = menuServiceShared.Value.Plataforma, Id = 0 } };

                ListaCentros = await MiLicenciaService.ObtenerTodosCentrosxComercio(consultaCentro);

                var empresasResponse = await MiLicenciaService.ObtenerEmpresasPines(applicationShared.IdRunt ?? "");
                ListaEmpresas = empresasResponse?.Entidad is { Count: > 0 }
                    ? empresasResponse.Entidad
                    : ["Todos"];

                // Define the GridItemsProvider. Its job is to convert QuickGrid's GridItemsProviderRequest into a query against
                // an arbitrary data soure. In this example, we need to translate query parameters into the particular URL format
                // supported by the external JSON API. It's only possible to perform whatever sorting/filtering/etc is supported
                // by the external API.

                pinesProvider = async req =>
                {
                    try
                    {
                        var consulta = new ConsultaInfoPinEstado
                        {
                            Canal = FiltrosModel.CanalVenta,
                            IdTipoDocumento = int.TryParse(FiltrosModel.TipoDocumento, out var idTipoDoc) ? idTipoDoc : (int?)null,
                            Documento = string.IsNullOrWhiteSpace(FiltrosModel.Documento) ? null : FiltrosModel.Documento,
                            Estado = FiltrosModel.EstadoPin,
                            FechaInicial = filtroFechaInicial,
                            FechaFinal = GetFechaFinalParaConsulta(),
                            IdAgenteDispersion = FiltrosModel.AliadoRecaudo ?? 0,
                            IdCentro = null,
                            // Inicio código generado por GitHub Copilot
                            IdRunt = string.IsNullOrWhiteSpace(filtroCentroIdRunt) ? applicationShared.IdRunt : filtroCentroIdRunt,
                            // Fin código generado por GitHub Copilot
                            NumPagina = (req.StartIndex / req.Count) + 1, // Calcula la página actual
                            NumRegistros = req.Count,
                            Opcion = 1,
                            Pin = FiltrosModel.Pin,
                            Empresa = string.IsNullOrWhiteSpace(FiltrosModel.Empresa) ? null : FiltrosModel.Empresa
                        };

                        // Inicio código generado por GitHub Copilot
                        string json = System.Text.Json.JsonSerializer.Serialize(consulta);
                        // Fin código generado por GitHub Copilot

                        ResponseDTO<List<ResponseInfoPinEstado>> result = null;
                        // Inicio código generado por GitHub Copilot

                        switch (estado)
                        {
                            case nameof(EnumTipoConsultaPines.Activos):
                            case nameof(EnumTipoConsultaPines.Usados):

                                result = await MiLicenciaService.ConsultaInfoPinesPorEstado(consulta);
                                break;
                            case nameof(EnumTipoConsultaPines.Devoluciones):
                                ResponseDTO<List<ResponseConsultaDevolucion>> result2 = await MiLicenciaService.ConsultaDevolucionesPines(consulta);
                                if (result2 != null && result2.Entidad != null)
                                {
                                    var mappedList = result2.Entidad.Select(d => new ResponseInfoPinEstado
                                    {
                                        Pin = d.Pin,
                                        FechaRegistro = d.FechaRegistro,
                                        ValorTransaccion = float.Parse(d.ValorDevolver),
                                        Estado = "Devuelto",
                                        NUTVenta = d.NUTVenta,
                                        RazonSocial = d.RazonSocial,
                                        AgenteDispersion = d.AgenteDispersion,
                                        Banco = d.Banco,
                                        CanalVenta = d.CanalVenta ?? 0,
                                        Cuotas = d.Cuotas,
                                        NombreCompleto = d.NombreCompleto,
                                        NumeroIdentificacion = d.NumeroIdentificacion,
                                        TipoIdentificacion = d.IdTipoIdentificacion,
                                        FechaDevolucion = d.FechaDevolucion,
                                        NovedadDevolucion = d.NovedadDevolucion,
                                        Correo = d.Correo,
                                        TotalRegistros = d.TotalRegistros,
                                        TipoDevolucion = d.TipoDevolucion,
                                        CuentaBanco = d.CuentaBanco,
                                        ConvenioEmpresa= d.ConvenioEmpresa,
                                        Empresa= d.Empresa
                                        
                                    }).ToList();
                                    result = new ResponseDTO<List<ResponseInfoPinEstado>>
                                    {
                                        Entidad = mappedList,
                                        Codigo = result2.Codigo
                                    };
                                }
                                break;
                        }
                        // Fin código generado por GitHub Copilot

                        if (result?.Entidad == null || result.Entidad.Count == 0)
                        {
                            SinResultados = true;
                            return GridItemsProviderResult.From(new List<ResponseInfoPinEstado>(), 0);
                        }

                        SinResultados = false;

                        return GridItemsProviderResult.From(
                            items: result.Entidad,
                            totalItemCount: result.Entidad[0].TotalRegistros
                        );
                    }
                    catch (Exception ex)
                    {
                        SinResultados = true;
                        _toastService.ShowError($"Ocurrió un error al consultar los pines: {ex.Message}");
                        return GridItemsProviderResult.From(new List<ResponseInfoPinEstado>(), 0);
                    }
                };
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Ocurrió un error al inicializar la consulta de pines: {ex.Message}");
            }
            // Fin refactorización/optimización por GitHub Copilot
        }

        // Esta es la única solución que he encontrado para hacer que mi paginador personalizado se actualice cuando cambian los filtros.
        // Sin esto, toca darle click a Filtrar para que se actualice el conteo
        private void Pagination_TotalItemCountChanged(object sender, int? e)
        {
            StateHasChanged();
        }

        async Task FilterChangedAsync()
        {
            HaHechoPrimeraConsulta = true;
            await pagination.SetCurrentPageIndexAsync(0);
            if (grid is null)
            {
                DebeRefrescarGrid = true;
                return;
            }

            await grid.RefreshDataAsync();
        }

        // Inicio código generado por GitHub Copilot
        private async Task HandleValidSubmitAsync(EditContext editContext)
        {
            await FilterChangedAsync();
        }
        // Fin código generado por GitHub Copilot

        private async Task GoToPageAsync(int pageIndex)
        {
            await pagination.SetCurrentPageIndexAsync(pageIndex);
        }

        private string PageButtonClass(int pageIndex)
            => pagination.CurrentPageIndex == pageIndex ? "btn-active" : null;

        private string AriaCurrentValue(int pageIndex)
            => pagination.CurrentPageIndex == pageIndex ? "page" : null;

        int? GetTipoIdentificacionInt(string tipoIdentificacion)
        {
            // Normaliza y elimina tildes de ambos textos antes de comparar
            string Normalize(string text) =>
                string.IsNullOrEmpty(text)
                    ? string.Empty
                    : System.Text.RegularExpressions.Regex.Replace(
                        text.Normalize(System.Text.NormalizationForm.FormD),
                        @"[\u0300-\u036f]", "");

            var doc = ListaDocumentos.FirstOrDefault(d =>
                string.Equals(Normalize(d.Nombre), Normalize(tipoIdentificacion), StringComparison.OrdinalIgnoreCase));

            return doc != null && int.TryParse(doc.IdTipoSisec, out var idInt) ? idInt : (int?)null;
        }

        private Task GoFirstAsync() => GoToPageAsync(0);
        private Task GoPreviousAsync() => GoToPageAsync(pagination.CurrentPageIndex - 1);
        private Task GoNextAsync() => GoToPageAsync(pagination.CurrentPageIndex + 1);
        private Task GoLastAsync() => GoToPageAsync(pagination.LastPageIndex.GetValueOrDefault(0));

        private async Task ShowModal(string id)
        {
            await JS.InvokeVoidAsync("showModalById", id);
        }

        // Inicio código generado por GitHub Copilot
        private bool CanGoBack => pagination.CurrentPageIndex > 0;
        private bool CanGoForwards => pagination.CurrentPageIndex < pagination.LastPageIndex;
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        private async Task LimpiarFiltrosAsync()
        {
            // Restablecer filtros a valores por defecto
            filtroFechaInicial = DateTime.Today.AddDays(-14);
            filtroFechaFinal = DateTime.Today;
            FiltrosModel.Pin = null;
            FiltrosModel.CanalVenta = 0;
            FiltrosModel.TipoDocumento = null;
            FiltrosModel.Documento = null;
            FiltrosModel.AliadoRecaudo = null;
            filtroCentroIdRunt = null;

            await FilterChangedAsync();
        }
        // Fin código generado por GitHub Copilot
    }
}


