using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System;
using portalAdministrativoSISEC.Entidades.Common;
using System.Collections.Generic;
using System.Linq;
using portalAdministrativoSISEC.Data.Pines;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia;
using Blazored.Toast.Services;
using System.Globalization;
using portalAdministrativoSISEC.Util.Extension;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using Microsoft.PowerBI.Api.Models;

namespace portalAdministrativoSISEC.Pages.Pines.PinesAsociados
{
    public partial class PinesAsociados
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IToastService _toastService { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        [Parameter]
        public string VerPinesAsociados { get; set; }

        [Parameter]
        public string pinValue { get; set; }

        private string? message;

        public bool isLoading = true;

        public int itemPerPage = 10;

        private readonly PaginationState Pagination = new() { ItemsPerPage = 5 };

        private List<RowList> DataQueryable;
        private GridItemsProvider<RowList>? Data;

        private List<ResponsePinesAsociados> Listado = new();

        private List<ColumnList> columnas;

        private Paginas PruebaPaginas = new Paginas();

        [Parameter]
        public bool Opened { get; set; }

        [Parameter]
        public EventCallback CloseButtonClicked { get; set; }

        #endregion Variables

        #region Metodos

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            isLoading = false;
        }

        public async Task Iniciar(string pin, string view = "")
        {
            if (!string.IsNullOrEmpty(pin))
            {
                pinValue = pin;
                isLoading = true;

                await RealizarConsulta(pin, view);

                StateHasChanged();
                isLoading = false;
            }
        }

        private async Task CargarDatagridView(List<ResponsePinesAsociados> Listado)
        {
            List<RowList> rows = [];

            if (Listado != null)
            {
                Listado.ForEach(x => rows.Add(new RowList { Row = SetInfoDataGrid(x) }));
            }
            else
            {
                // Agregar solo los encabezados si la lista es nula o está vacía
                List<ColumnList> headers = PagoPinConst.InicializarColumnasAsociados();

                rows.Add(new RowList { Row = headers });
            }

            DataQueryable = rows;

            Data = async req =>
            {
                return GridItemsProviderResult.From(
                items: rows,
                totalItemCount: Listado.Count);
            };

            await Task.FromResult(true);
        }

        private void OnClose(EventArgs e) => message += "onclose, ";

        private void OnCancel(EventArgs e) => message += "oncancel, ";

        private async Task paginaChanged(Paginas paginas)
        {
            var paginado = paginas;
            PruebaPaginas = paginas;
            await RegistroPagina(PruebaPaginas.PaginaActual, PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
            StateHasChanged();
        }

        private async Task RegistroPagina(int pagina, int totalRegistrosPagina, int totalRegistros)
        {
            PruebaPaginas.RegistroInicialPagina = (pagina - 1) * totalRegistrosPagina + 1;
            PruebaPaginas.RegistroFinalPagina = Math.Min(pagina * totalRegistrosPagina, totalRegistros);
            PruebaPaginas.RegistrosPorPagina = totalRegistrosPagina;
            PruebaPaginas.PaginaActual = pagina;
            await SetPaginadorInicial(Listado, PruebaPaginas, true);
        }

        private async Task SetPaginadorInicial(List<ResponsePinesAsociados> listado, Paginas paginas, bool cambioPagina)
        {
            if (listado.Any())
            {
                if (!cambioPagina)
                {
                    // Solo para paso inicial de llenado de paginador
                    double cantidadPaginasDecimal = (double)listado.Count / 5;
                    int cantidadPaginas = (int)Math.Ceiling(cantidadPaginasDecimal);
                    PruebaPaginas.PaginaActual = 1;
                    PruebaPaginas.TotalPaginas = cantidadPaginas;
                    PruebaPaginas.PaginaInicial = 1;
                    PruebaPaginas.PaginaFinal = cantidadPaginas;
                    PruebaPaginas.RegistrosPorPagina = 5;
                    Pagination.ItemsPerPage = paginas.RegistrosPorPagina;
                    PruebaPaginas.TotalRegistros = paginas.TotalRegistros;
                    PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
                    PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
                }
                else
                {
                    // Se dispara solo en casos de cambio de pagina desde quickgrid
                    double cantidadPaginasDecimal = (double)listado.Count / paginas.RegistrosPorPagina;
                    int cantidadPaginas = (int)Math.Ceiling(cantidadPaginasDecimal);
                    PruebaPaginas.PaginaActual = paginas.PaginaActual;
                    PruebaPaginas.TotalPaginas = cantidadPaginas;
                    PruebaPaginas.PaginaInicial = 1;
                    PruebaPaginas.PaginaFinal = cantidadPaginas;
                    PruebaPaginas.RegistrosPorPagina = paginas.RegistrosPorPagina;
                    PruebaPaginas.TotalRegistros = listado.Count;
                    PruebaPaginas.RegistroInicialPagina = (PruebaPaginas.PaginaActual - 1) * PruebaPaginas.RegistrosPorPagina + 1;
                    PruebaPaginas.RegistroFinalPagina = Math.Min(PruebaPaginas.PaginaActual * PruebaPaginas.RegistrosPorPagina, PruebaPaginas.TotalRegistros);
                    Pagination.ItemsPerPage = paginas.RegistrosPorPagina;
                    await Pagination.SetCurrentPageIndexAsync(paginas.PaginaActual);
                }
            }
        }

        private void CloseDialog()
        {
            Opened = false;
            CloseButtonClicked.InvokeAsync();
        }

        private async Task RealizarConsulta(string pin, string view = "")
        {
            isLoading = true;

            var result = await MiLicenciaService.ConsultarPinesAsociados(pin, view);
            if (result != null)
            {
                isLoading = false;
                if (result.Codigo == 0 && result.Entidad != null)//(result.Entidad != null)
                {
                    columnas = (result?.Entidad?.FirstOrDefault()?.AgenteDispersion ?? "") == "COLPATRIA"
                        ? PagoPinConst.InicializarColumnasAsociadosPinDirecto()
                        : PagoPinConst.InicializarColumnasAsociados();

                    Listado = result.Entidad;
                    OfuscarListado(false);
                    await CargarDatagridView(Listado);
                }
                else
                {
                    Listado = new();
                    await CargarDatagridView(Listado);

                    _toastService.ShowInfo($"Actualmente no hay pines asociados. Por favor, realiza el pago de una cuota en la sección 'Pago de Cuotas' para visualizar la información - \n{result.Respuesta}", "Información");
                }
            }
            else
            {
                isLoading = false;
                Listado = new();
                await CargarDatagridView(Listado);
                _toastService.ShowError("Ha ocurrido un error en la consulta.");
            }
            PruebaPaginas.TotalRegistros = Listado.Count;
            await SetPaginadorInicial(Listado, PruebaPaginas, false);
        }

        public void OfuscarListado(bool descarga)
        {
            if (!descarga)
            {
                if (Listado.Any())
                {
                    for (int i = 0; i < Listado.Count; i++)
                    {
                        if (Listado[i].CanalVenta == (int)EnumOrigenCotizacion.MiLicencia || Listado[i].CanalVenta == (int)EnumOrigenCotizacion.PAO)
                        {
                            //Listado[i].Pin = OfuscarString(Listado[i].Pin,5);
                            Listado[i].NumeroIdentificacion = OfuscarString(Listado[i].NumeroIdentificacion, 4);
                        }
                    }
                }
            }
            else
            {
            }
        }

        public static string OfuscarString(string input, int visibleChars)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.Length <= 5)
                return input;

            int ofuscatedLength = input.Length - visibleChars;

            string ofuscado = new string('*', ofuscatedLength) +
                              input.Substring(ofuscatedLength);

            return ofuscado;
        }

        private List<ColumnList> SetInfoDataGrid(ResponsePinesAsociados pinesAsociados)
        {
            string ValorTransaccion = Convert.ToDecimal(pinesAsociados.ValorTransaccion).FormatAsCurrency();
            string ValorActor = Convert.ToDecimal(pinesAsociados.ValorActor).FormatAsCurrency();
            string ValorAns = Convert.ToDecimal(pinesAsociados.ValorAns).FormatAsCurrency();
            string ValorAliado = Convert.ToDecimal(pinesAsociados.ValorAliado).FormatAsCurrency();
            string ValosSicov = Convert.ToDecimal(pinesAsociados.ValosSicov).FormatAsCurrency();

            List<ColumnList> row =
            [
                new() { NameColumn = "Pin", Value = (!string.IsNullOrEmpty(pinesAsociados.Pin) ? pinesAsociados.Pin : "No encontrado")},
                new() { NameColumn = "Tipo Documento", Value = (!string.IsNullOrEmpty(pinesAsociados.TipoIdentificacion) ? pinesAsociados.TipoIdentificacion : "No encontrado") },
                new() { NameColumn = "Número Documento", Value = (!string.IsNullOrEmpty(pinesAsociados.NumeroIdentificacion) ? pinesAsociados.NumeroIdentificacion : "No encontrado") },
                new() { NameColumn = "Valor Pin", Value = (!string.IsNullOrEmpty(pinesAsociados.ValorTransaccion.ToString())? ValorTransaccion : "No encontrado") },
                new() { NameColumn = "Valor Actor", Value = (!string.IsNullOrEmpty(pinesAsociados.ValorActor.ToString()) ? ValorActor : "No encontrado") },
                new() { NameColumn = "Valor Aliado", Value = (!string.IsNullOrEmpty(pinesAsociados.ValorAliado.ToString()) ? ValorAliado : "No encontrado") },
                new() { NameColumn = "Agente Dispersión", Value = (!string.IsNullOrEmpty(pinesAsociados.AgenteDispersion) ? pinesAsociados.AgenteDispersion : "No encontrado") },

            ];

            if (pinesAsociados.AgenteDispersion == "COLPATRIA")
                row.Add(new() { NameColumn = "Fecha pago", Value = (!string.IsNullOrEmpty(pinesAsociados.FechaRegistro.ToString()) ? pinesAsociados.FechaRegistro.ToString() : "No encontrado") });
            else
            {
                row.AddRange(
                [
                    new() { NameColumn = "Valor ANSV", Value = (!string.IsNullOrEmpty(pinesAsociados.ValorAns.ToString()) ? ValorAns : "No encontrado") },
                    new() { NameColumn = "Valor Sicov", Value = (!string.IsNullOrEmpty(pinesAsociados.ValosSicov.ToString()) ? ValosSicov : "No encontrado") },
                    new() { NameColumn = "Fecha Operación", Value = (!string.IsNullOrEmpty(pinesAsociados.FechaRegistro.ToString()) ? pinesAsociados.FechaRegistro.ToString() : "No encontrado") },
                    new() { NameColumn = "Fecha Dispersión", Value = (!string.IsNullOrEmpty(pinesAsociados.FechaDispersion.ToString()) ? pinesAsociados.FechaDispersion.ToString() : "No encontrado") },
                    new() { NameColumn = "Banco Dispersión", Value = (!string.IsNullOrEmpty(pinesAsociados?.Banco) ? pinesAsociados?.Banco : "No encontrado") },
                    new() { NameColumn = "Cuenta Dispersión", Value = (!string.IsNullOrEmpty(pinesAsociados?.CtaDispersion) ? pinesAsociados?.CtaDispersion : "No encontrado") },
                    new() { NameColumn = "Valor Dispersión", Value = (!string.IsNullOrEmpty(pinesAsociados.ValorTransaccion.ToString()) ? ValorTransaccion : "No encontrado") },
                    new() { NameColumn = "Razón Social", Value = (!string.IsNullOrEmpty(pinesAsociados.RazonSocial) ? pinesAsociados.RazonSocial : "No encontrado") },
                    new() { NameColumn = "Tipo Pin", Value = (!string.IsNullOrEmpty(pinesAsociados.TipoPin) ? pinesAsociados.TipoPin : "No encontrado") }
                ]);
            }

            return row;
        }

        #endregion Metodos
    }
}