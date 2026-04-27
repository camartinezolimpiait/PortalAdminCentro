using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.CuotasCeas
{
	public partial class CuotasCeas
	{
		#region Inyeccion Dependencias

		[Inject]
		private IMiLicenciaService MiLicenciaService { get; set; }

		[Inject]
		private IJSRuntime JS { get; set; }

		#endregion Inyeccion Dependencias

		#region Variables

		[Parameter]
		public PagoPin PagoPin { get; set; }

		[Parameter]
		public EventCallback<bool> OnFormValidChanged { get; set; }

		[Parameter]
		public EventCallback<PagoPin> PagoPinChanged { get; set; }

        [Parameter]
        public EventCallback OnRetroceder { get; set; }

        internal CostoCuota CuotaSeleccionada { get; set; }
        private EditContext editContext;
		private PagoCuotasModel PagoCuotasModel = new();
        private bool isLoading = true;

        #endregion Variables

        /// <summary>
        /// Inicializa el formulario
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
		{
            editContext = new EditContext(PagoCuotasModel);

            if (PagoPin?.ConfiguracionCuotas?.CuotaSeleccionada?.NumeroCuotas > 0)
			{
				CostoCuota cuota = PagoPin?.CostoCuotas.FirstOrDefault(x => x.NumeroCuotas == PagoPin?.ConfiguracionCuotas?.CuotaSeleccionada?.NumeroCuotas);

				if (cuota?.PrimeraCuota != PagoPin?.ConfiguracionCuotas?.CuotaSeleccionada?.PrimeraCuota)
				{
					CuotaSeleccionada = cuota;
                    PagoCuotasModel.NumeroCuotas = cuota?.NumeroCuotas;

                }
				else
				{
					CuotaSeleccionada = PagoPin?.ConfiguracionCuotas?.CuotaSeleccionada;
                    PagoCuotasModel.NumeroCuotas = PagoPin?.ConfiguracionCuotas?.CuotaSeleccionada?.NumeroCuotas;
                }
			}
			isLoading = false;

        }

		/// <summary>
		/// Se dispara el evento al seleciconar un dio button de cuotas
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private async Task RadioSelection()
		{
            bool isValid = await ValidacionCuota();
			if (isValid)
			{
				await OnFormValidChanged.InvokeAsync(true);
            }
        }

		/// <summary>
		/// Validaciones para verificar que la cuota se haya seleccionado correctamente
		/// </summary>
		/// <returns></returns>
		private async Task<bool> ValidacionCuota()
		{
            CuotaSeleccionada = PagoPin?.CostoCuotas?.FirstOrDefault(x => x.NumeroCuotas == PagoCuotasModel.NumeroCuotas);

            if (CuotaSeleccionada.NumeroCuotas == 0 || CuotaSeleccionada.PrimeraCuota == 0 || CuotaSeleccionada.ValorTotal == 0)
			{
				await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "Se debe seleccionar opción de cuota para pagar");
				await PagoPinChanged.InvokeAsync(PagoPin);
				return await Task.FromResult(false);
			}

			if (CuotaSeleccionada.NumeroCuotas > 0)
			{
				//PENDIENTE ASIGNACION DE CUOTAS AL OBJETO PAGOPIN PARA LA CREACION DEL PIN
				PagoPin.ConfiguracionCuotas.CuotaSeleccionada = CuotaSeleccionada;

				PagoPin.ValorDiscriminadoCotizacion.ValorTotal = (double)CuotaSeleccionada.ValorTotal;
				PagoPin.ValorDiscriminadoCotizacion.Banco = PagoPin.ConfiguracionCuotas.ValorAliado;
				PagoPin.Cuotas = PagoPin.ConfiguracionCuotas.CuotaSeleccionada.NumeroCuotas;

				await PagoPinChanged.InvokeAsync(PagoPin);
				return await Task.FromResult(true);
			}

			return await Task.FromResult(false);
		}


        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }
    }
}

