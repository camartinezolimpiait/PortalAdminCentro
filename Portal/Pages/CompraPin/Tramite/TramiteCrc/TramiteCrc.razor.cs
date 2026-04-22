using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Enum.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.Tramite.TramiteCrc
{
    public partial class TramiteCrc
    {

        #region variables
        private bool isLoading = true;
        [Parameter]
        public PagoPin PagoPin { get; set; }

        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }

        [Parameter] 
        public EventCallback OnRetroceder { get; set; }

        private int OpcionCantidad { get; set; }

        public OpcionCantidadModel cantidadModel { get; set; }

        private EditContext editContext;

        private bool componentePrincipal { get; set; }
        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }

        public IEnumerable<OpcionTramitesEnum> Tramites { get; set; } =
            System.Enum.GetValues(typeof(OpcionTramitesEnum)).Cast<OpcionTramitesEnum>();


        public Dictionary<int, string> DescripcionOpcionTramite { get; set; } =
            new()
            {
            { 1, "Un solo trámite" },
            { 2, "Varios trámites (Carro y moto)" }
            };

        public Dictionary<EnumTramite, string> DescripcionTramite { get; set; } = new()
        {
            { EnumTramite.SinDefinir, "Trámite no definido" },
            { EnumTramite.PrimeraVez, "Primera vez o licencia adicional" },
            { EnumTramite.Renovar, "Renovar licencia" },
            { EnumTramite.Recategorizar, "Recategorizar licencia" },
            { EnumTramite.PrimeraVezInstructor, "Nueva licencia de instructor" },
            { EnumTramite.RecategorizarInstructor, "Recategorizar licencia de instructor" }
        };

        public List<SeleccionTramites> ListaSeleccionTramite { get; set; } = new();

        #endregion


        protected override void OnInitialized()
        {
            cantidadModel ??= new();
            editContext = new EditContext(cantidadModel);
            componentePrincipal = true;

            ListaSeleccionTramite = System.Enum.GetValues(typeof(EnumTramite))
                .Cast<EnumTramite>()
                .Select(tramite => new SeleccionTramites
                {
                    Id = (int)tramite,
                    Nombre = DescripcionTramite.ContainsKey(tramite) ? DescripcionTramite[tramite] : "",
                    Instructor = DescripcionTramite.ContainsKey(tramite) &&
                                 DescripcionTramite[tramite].Contains("instructor", StringComparison.OrdinalIgnoreCase),
                    Selected = false
                }).Where(x => x.Id != 0 && !x.Instructor)
                .ToList();

            if (PagoPin.OpcionTramite > 0)
            {
                cantidadModel.Cantidad = PagoPin.OpcionTramite.Value;
            }

            if (PagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto || PagoPin.PasoCotizacion == PasosCompraPin.DatosPersonales ||
                PagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro || PagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboMoto || 
                PagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteSimple || PagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboCarro)
            {
                componentePrincipal = false;
            }

            isLoading = false;
        }

        private async Task<bool> HandleValidSubmit()
        {
            PagoPin.OpcionTramite = cantidadModel.Cantidad.Value;
            PagoPin.PasoCotizacion = PagoPin.OpcionTramite == DescripcionOpcionTramite.FirstOrDefault(x => x.Key == 1).Key
                ? PasosCompraPin.TipoTramiteSimple
                : PasosCompraPin.TipoTramiteComboCarro;

            await PagoPinChanged.InvokeAsync(PagoPin);
            componentePrincipal = false;
            StateHasChanged();
            return true;
        }

        private async Task HandleAfterSubmit()
        {
            if (cantidadModel.Cantidad != PagoPin.OpcionTramite)
            {
                PagoPin.Categoria = "";
                PagoPin.Categoria1 = "";
                PagoPin.Categoria2 = "";
                PagoPin.TipoTramite = null;
                PagoPin.TipoTramite2 = null;
                PagoPin.ValorDiscriminadoCotizacion = new();
                PagoPin.TramiteInstructor = false;
                await PagoPinChanged.InvokeAsync(PagoPin);
            }
        }

        private async Task Retroceder()
        {
            if (componentePrincipal || PagoPin.PasoCotizacion == PasosCompraPin.DatosBasicos || PagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
            {
                await OnRetroceder.InvokeAsync();
            }
            else
            {
                if (PagoPin.PasoCotizacion == PasosCompraPin.CantidadTramites)
                {
                    componentePrincipal = true;
                }
                StateHasChanged();
            }
        }

        private void HandleFormValidChangedTramite()
        {
            OnFormValidChanged.InvokeAsync(true);
        }
    }
}

