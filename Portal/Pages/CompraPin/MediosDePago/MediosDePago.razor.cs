using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.MediosDePago
{
    public partial class MediosDePago
    {
        #region Parámetros y Variables

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

        #endregion

        #region Métodos del Ciclo de Vida

        protected override void OnInitialized()
        {
            editContext = new EditContext(MediosDePagoModel);
            if (pagoPin.TipoRecaudoCtrl != null || pagoPin.TipoRecaudoCtrl > 0)
            {
                MediosDePagoModel.TipoRecaudoCtrl = pagoPin.TipoRecaudoCtrl;
            }
            isLoading = false;
        }

        #endregion

        #region Métodos de Navegación

        // Método generado por GitHub Copilot
        /// <summary>
        /// Maneja el evento de retroceso al paso anterior
        /// </summary>
        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }

        #endregion

        #region Métodos de Validación y Envío

        // Método generado por GitHub Copilot
        /// <summary>
        /// Maneja el envío del formulario cuando es válido
        /// Actualiza el PagoPin y notifica al componente padre
        /// </summary>
        private async Task HandleValidSubmit()
        {
            pagoPin.TipoRecaudoCtrl = MediosDePagoModel.TipoRecaudoCtrl;

            // Notificar cambios al componente padre
            await PagoPinChanged.InvokeAsync(pagoPin);
            await OnFormValidChanged.InvokeAsync(true);
        }

        #endregion

        #region Métodos de Cambio de Estado

        // Método generado por GitHub Copilot
        /// <summary>
        /// Maneja el cambio de método de pago
        /// Reinicia la configuración de cuotas cuando cambia el medio de pago
        /// </summary>
        private async Task ChangeMethod()
        {
            pagoPin.TipoRecaudoCtrl = MediosDePagoModel.TipoRecaudoCtrl;
            pagoPin.ConfiguracionCuotas.CuotaSeleccionada = null;
            pagoPin.Cuotas = 0;

            await Task.CompletedTask;
        }

        #endregion

        #region Eventos del Componente CouponCode

        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Maneja el evento cuando se aplica exitosamente un cupón de referido
        /// Actualiza el objeto PagoPin con la información del convenio y notifica al componente padre
        /// </summary>
        /// <param name="codigo">Código del cupón aplicado</param>
        private async Task OnCouponAplicado(string codigo)
        {
            // El componente CouponCode ya actualizó pagoPin.CodigoConvenio y pagoPin.EmpresaConvenio
            // Aquí podemos agregar lógica adicional si es necesaria

            // Notificar al componente padre sobre el cambio en PagoPin
            await PagoPinChanged.InvokeAsync(pagoPin);

            // Opcional: Aquí podrías agregar lógica para recalcular costos si el cupón otorga descuentos
            // Por ejemplo:
            // await RecalcularCostosConDescuento(codigo);

            // Forzar actualización de la UI
            StateHasChanged();
        }
        // Fin código generado por GitHub Copilot

        #endregion
    }
}

