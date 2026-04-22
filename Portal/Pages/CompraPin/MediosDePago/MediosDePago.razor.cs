using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.MediosDePago
{
    public partial class MediosDePago
    {
        #region 
        private bool isLoading = true;
        [Parameter]
        public PagoPin pagoPin { get; set; }
        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }
        [Parameter]
        public EventCallback OnRetroceder { get; set; }
        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }

        private EditContext editContext;
        private MedioDePagoModel MediosDePagoModel = new();


        protected override void OnInitialized()
        {
            editContext = new EditContext(MediosDePagoModel);
            if (pagoPin.TipoRecaudoCtrl != null || pagoPin.TipoRecaudoCtrl > 0)
            {
                MediosDePagoModel.TipoRecaudoCtrl = pagoPin.TipoRecaudoCtrl;
            }
            isLoading = false;
        }

        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }

        private async Task HandleValidSubmit()
        {
            pagoPin.TipoRecaudoCtrl = MediosDePagoModel.TipoRecaudoCtrl;
            await PagoPinChanged.InvokeAsync(pagoPin);
            await OnFormValidChanged.InvokeAsync(true);
        }

        private async Task ChangeMethod()
        {
            pagoPin.TipoRecaudoCtrl = MediosDePagoModel.TipoRecaudoCtrl;
            pagoPin.ConfiguracionCuotas.CuotaSeleccionada = null;
            pagoPin.Cuotas = 0;
        }


        #endregion
    }
}
