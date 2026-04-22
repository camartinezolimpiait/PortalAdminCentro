using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.Tramite.TipoTramite
{
    public partial class TipoTramite
    {

        #region 
        private bool isLoading = true;
        public List<SeleccionTramites> ListaSeleccionTramite { get; set; } = new();
        [Parameter]
        public PagoPin pagoPin { get; set; }
        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }
        [Parameter]
        public EventCallback OnRetroceder { get; set; }
        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }
        private EditContext editContext;

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
        private ValidationMessageStore messageStore;
        private static readonly object DummyModel = new();

        private int? TipoTramiteSeleccionado
        {
            get
            {
                // Lectura: decide de dónde tomar el valor
                if (pagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboMoto
                    || pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto)
                {
                    return pagoPin.TipoTramite2;
                }
                else
                {
                    return pagoPin.TipoTramite;
                }
            }
            set
            {
                if (pagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboMoto
                    || pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto)
                {
                    if (pagoPin.TipoTramite2 != value)
                    {
                        // Reset categorías si cambió el valor
                        pagoPin.Categoria2 = string.Empty;

                        pagoPin.TipoTramite2 = value;
                        _ = PagoPinChanged.InvokeAsync(pagoPin); // no necesitas esperar en un setter
                    }
                }
                else
                {
                    if (pagoPin.TipoTramite != value)
                    {
                        // Reset categorías si cambió el valor
                        pagoPin.Categoria = string.Empty;
                        pagoPin.Categoria1 = string.Empty;

                        pagoPin.TipoTramite = value;
                        ValidarTipoTramite();
                        _ = PagoPinChanged.InvokeAsync(pagoPin);
                    }
                }
            }
        }


        #endregion

        protected override void OnInitialized()
        {
            editContext = new EditContext(DummyModel);
            messageStore = new(editContext);
            editContext.OnValidationRequested += HandleValidationRequested;

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

            if (pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
            {
                pagoPin.PasoCotizacion = PasosCompraPin.TipoTramiteComboCarro;
            }

            isLoading = false;
        }

        private async Task Retroceder()
        {
            if (pagoPin.PasoCotizacion == PasosCompraPin.DatosPersonales && pagoPin.OpcionTramite == DescripcionOpcionTramite.FirstOrDefault(x => x.Key == 2).Key)
            {
                pagoPin.PasoCotizacion = PasosCompraPin.CategoriaComboCarro;
            }
            else
            {
                pagoPin.PasoCotizacion = pagoPin.OpcionTramite == DescripcionOpcionTramite.FirstOrDefault(x => x.Key == 1).Key ?
                PasosCompraPin.CantidadTramites : pagoPin.PasoCotizacion - 1;
                pagoPin.Categoria1 = pagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboCarro ? "" : pagoPin.Categoria1;
            }

            await PagoPinChanged.InvokeAsync(pagoPin);
            if (pagoPin.PasoCotizacion == PasosCompraPin.CantidadTramites || pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
            {
                await OnRetroceder.InvokeAsync();
            }
        }

        private async Task OnValidSubmit()
        {
            await PagoPinChanged.InvokeAsync(pagoPin);
            if (pagoPin.PasoCotizacion != PasosCompraPin.CantidadTramites)
            {
                await OnFormValidChanged.InvokeAsync(true);
            }
        }

        private void ValidarTipoTramite()
        {
            switch (pagoPin.TipoTramite)
            {
                case (int)EnumTramite.PrimeraVez:
                    pagoPin.TramiteInstructor = false;
                    break;

                case (int)EnumTramite.Recategorizar:
                    pagoPin.TramiteInstructor = false;
                    break;

                case (int)EnumTramite.PrimeraVezInstructor:
                    pagoPin.TramiteInstructor = true;
                    pagoPin.TipoTramite = (int)EnumTramite.PrimeraVezInstructor;
                    break;

                case (int)EnumTramite.RecategorizarInstructor:
                    pagoPin.TipoTramite = (int)EnumTramite.RecategorizarInstructor;
                    pagoPin.TramiteInstructor = true;
                    break;
            }
        }

        private void HandleValidationRequested(object sender, ValidationRequestedEventArgs args)
        {
            messageStore?.Clear();
            if (TipoTramiteSeleccionado == 0 || TipoTramiteSeleccionado == null)
            {
                messageStore.Add(() => TipoTramiteSeleccionado, "Debe seleccionar al menos un tipo de trámite");
            }
        }

    }
}
