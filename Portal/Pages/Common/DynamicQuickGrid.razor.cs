using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.PowerBI.Api.Models;
using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Pages.Pines.PinesAsociados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Common
{
    public partial class DynamicQuickGrid
    {
        #region Variables

        [Parameter]
        public GridItemsProvider<RowList> Data { get; set; }

        [Parameter]
        public List<RowList> DataQueryable { get; set; }

        [Parameter]
        public List<ColumnList> Columns { get; set; }

        [Parameter]
        public PaginationState Pagination { get; set; }

        [Parameter]
        public EventCallback<string> BotonDescargaActived { get; set; }

        [Parameter]
        public EventCallback<Paginas> PaginaChanged { get; set; }

        [Parameter]
        public string Theme { get; set; } = "milicencia";

        [Parameter]
        public EventCallback<string> BotonVerPinesAsociadosActived { get; set; }

        [Parameter]
        public EventCallback<RowList> EventCallbackButton { get; set; }

        // Objeto para paginador de consultas.
        [Parameter]
        public Paginas Paginas { get; set; } = new();

        [Parameter]
        public bool ShowPager { get; set; } = true;

        [Parameter]
        public string View { get; set; } = "";

        public int valorMaximo { get; set; } = 0;
        public int PaginaInicial { get; set; } = 0;
        public int PaginasRecorrido { get; set; } = 0;
        private string atras { get; set; } = "<";
        private string siguiente { get; set; } = ">";
        public string PinValue { get; set; } = "";
        private PinesAsociados pinesAsociados;
        private bool dialogShow = false;

        [Parameter]
        public bool IsPagoCuotas { get; set; }

        public string _lastColor = "bg-color-white";

        #endregion Variables

        protected override async Task OnInitializedAsync()
        {
            if (Paginas != null)
            {
                ValidarCantidades();
            }
        }

        private async Task DescargarDetalle(RowList value)
        {
            string valor = value.Row.FirstOrDefault().Value.ToString();
            if (!string.IsNullOrEmpty(valor))
            {
                if (long.Parse(valor) > 0)
                    await BotonDescargaActived.InvokeAsync(valor);
            }
        }

        private async Task EventCallbackButtonClick(RowList rowList)
        {
            await EventCallbackButton.InvokeAsync(rowList);
        }

        private async Task VerPinesAsociados(RowList value)
        {
            dialogShow = true;
            if (value.Row.Count > 0)
            {
                int columnIndex = -1;
                for (int i = 0; i < value.Row.Count; i++)
                {
                    if (value.Row[i].NameColumn == "Pines Asociados" || value.Row[i].NameColumn == "Cuotas Pin")
                    {
                        columnIndex = i;
                        break;
                    }
                }
                if (columnIndex != -1)
                {
                    string pin = value.Row[columnIndex].Value;
                    await pinesAsociados.Iniciar(pin, View);
                }
            }
        }

        private bool ValidarPinesAsociados(RowList value)
        {
            if (value.Row.Count > 0)
            {
                string columnCuotas = "0";

                columnCuotas = value.Row.FirstOrDefault(x => x.NameColumn == (View == "Usados" ? "Pines Asociados" : "Cuotas"))?.Value ?? "0";

                if (columnCuotas != "0")
                    return true;
            }
            return false;
        }

        public async Task OnSiguienteClicked(int valor)
        {
            Paginas.PaginaActual = valor + 1;
            ValidarCantidades();
            await PaginaChanged.InvokeAsync(Paginas);
        }

        public async Task OnSeleccionClicked(int valor)
        {
            Paginas.PaginaActual = valor;
            ValidarCantidades();
            await PaginaChanged.InvokeAsync(Paginas);
        }

        public async Task OnAtrasClicked(int valor)
        {
            Paginas.PaginaActual = valor - 1;
            ValidarCantidades();
            await PaginaChanged.InvokeAsync(Paginas);
        }

        public async Task OnSiguienteFinalClicked(int valor)
        {
            Paginas.PaginaActual = Paginas.PaginaFinal;
            ValidarCantidades();
            await PaginaChanged.InvokeAsync(Paginas);
        }

        public async Task OnAtrasInicialClicked(int valor)
        {
            Paginas.PaginaActual = Paginas.PaginaInicial;
            ValidarCantidades();
            await PaginaChanged.InvokeAsync(Paginas);
        }

        private async Task CantidadRegistrosChanged(ChangeEventArgs e)
        {
            var value = e.Value.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                Paginas.RegistrosPorPagina = int.Parse(value);
                Paginas.PaginaActual = 1;
                ValidarCantidades();
                await PaginaChanged.InvokeAsync(Paginas);
            }
        }

        public void ValidarCantidades()
        {
            var cantidadPaginasDecimal = (double)Paginas.TotalRegistros / Paginas.RegistrosPorPagina;
            Paginas.TotalPaginas = (int)Math.Ceiling(cantidadPaginasDecimal);

            if (Paginas.TotalPaginas > Paginas.PaginaActual)
            {
                int cantidadMuestra = Paginas.TotalPaginas - Paginas.PaginaActual;
                if (cantidadMuestra > 4)
                {
                    PaginasRecorrido = 0;
                    valorMaximo = 4;
                    PaginaInicial = Paginas.PaginaActual;
                    PaginasRecorrido = Paginas.PaginaActual + valorMaximo + 1;
                }
                else
                {
                    PaginasRecorrido = 0;
                    valorMaximo = cantidadMuestra;
                    PaginaInicial = Paginas.PaginaActual;
                    PaginasRecorrido = Paginas.PaginaActual + valorMaximo;
                }
            }
            else
            {
                valorMaximo = 0;
                if (Paginas.PaginaActual == Paginas.TotalPaginas)
                {
                    if (Paginas.TotalPaginas < 5 && Paginas.TotalPaginas > 1)
                    {
                        switch (Paginas.TotalPaginas)
                        {
                            case 2:
                                PaginaInicial = 2;
                                PaginasRecorrido = Paginas.TotalPaginas + 1;
                                break;

                            case 3:
                                PaginaInicial = 2;
                                PaginasRecorrido = Paginas.TotalPaginas + 1;
                                break;

                            case 4:
                                PaginaInicial = 2;
                                PaginasRecorrido = Paginas.TotalPaginas + 1;
                                break;
                        }
                    }
                    else
                    {
                        if (Paginas.TotalPaginas > 4)
                        {
                            PaginaInicial = Paginas.TotalPaginas - 4;
                            PaginasRecorrido = Paginas.TotalPaginas;
                        }
                        else
                        {
                            PaginaInicial = 1;
                            PaginasRecorrido = 0;
                        }
                        if (Paginas.TotalPaginas == 0)
                        {
                            PaginaInicial = 1;
                            PaginasRecorrido = 0;
                        }
                        PaginasRecorrido = Paginas.TotalPaginas;
                    }
                }
            }
        }

        private void ShowDialog()
        {
            dialogShow = true;
        }
    }
}