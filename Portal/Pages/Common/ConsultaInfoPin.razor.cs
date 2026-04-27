using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Common
{
	public partial class ConsultaInfoPin
	{
		#region Inyeccion Dependencias

		[Inject]
		private IMiLicenciaService MiLicenciaService { get; set; }

		#endregion Inyeccion Dependencias

		#region Variables

		[Parameter]
		public EventCallback<ConsultaDevolucionPorPinRequest> ConsultaDevolucionesChanged { get; set; }

		[Parameter]
		public string IdRunt { get; set; }

		[Parameter]
		public EventCallback<bool> OnFormCompleteChanged { get; set; }

        [Parameter]
        public EventCallback<bool> isFormComplete { get; set; }
        

        [Parameter]
        public bool IsPagoCuotas { get; set; }

        [Parameter]
        public bool NuevaConsulta { get; set; }
        

        private bool hasSelectedTipoDocumento = false;


        #endregion Variables

        private List<TipoDocumentoPtesaDTO> ListaDocumentos { get; set; } = [];

		private ConsultaDevolucionesModel ConsultaDevolucionesModel = new();
		private ValidationMessageStore MessageStore;
		private EditContext EditContext;
		private bool DocumentoValido = false;
		public bool caracteresValidos = false;
        private ConsultaDevolucionPorPinRequest DevolucionPorPinRequest { get; set; } = new();

		// Inicio código generado por GitHub Copilot
        [Parameter] public string? PinPrefill { get; set; }
        [Parameter] public int? TipoIdPrefill { get; set; }
        [Parameter] public string? NumeroIdPrefill { get; set; }

        private bool AutoSubmitEjecutado;
        // Fin código generado por GitHub Copilot

		protected override async Task OnInitializedAsync()
		{
			EditContext = new EditContext(ConsultaDevolucionesModel);

			await ObtenerTiposDocumento();

			MessageStore = new ValidationMessageStore(EditContext);
			await OnFormCompleteChanged.InvokeAsync(DocumentoValido);
		}

		private async Task NotifyValidationStateChanged()
		{
			EditContext.Validate(); // Esto valida el contexto de edición y devuelve true si es válido.
			StateHasChanged();

			//await Task.FromResult(true);
		}

        private async Task OnValidSubmit()
        {
            // Solo se ejecuta si el formulario es válido
            DevolucionPorPinRequest.Pin = ConsultaDevolucionesModel.Pin;
            DevolucionPorPinRequest.TipoIdentificacion = (int)ConsultaDevolucionesModel.TipoDocumento;
            DevolucionPorPinRequest.NumeroIdentificacion = ConsultaDevolucionesModel.Documento;
            DevolucionPorPinRequest.IdRunt = IdRunt;

            await ConsultaDevolucionesChanged.InvokeAsync(DevolucionPorPinRequest);
        }
		private async Task ObtenerTiposDocumento()
		{
			ListaDocumentos = await MiLicenciaService.ObtenerTipoDocumentos();
            ListaDocumentos = FilterDocumentsAviable.FilterDocumentsTypes(ListaDocumentos);
            ListaDocumentos = ListaDocumentos?.Where(x => x.VisualizarCea == 1).ToList();
		}

        protected override async Task OnParametersSetAsync()
        {
            if (NuevaConsulta)
            {
                await ObtenerTiposDocumento();
                LimpiarFormulario();
                await OnFormCompleteChanged.InvokeAsync(false);
            }

            // Inicio código generado por GitHub Copilot
            // Prefill desde querystring:
            // - Si un valor viene vacío, no se toca ese campo.
            // - El auto-submit solo corre cuando vienen los 3.
            var hayPinPrefill = !string.IsNullOrWhiteSpace(PinPrefill);
            var hayNumeroPrefill = !string.IsNullOrWhiteSpace(NumeroIdPrefill);
            var hayTipoPrefill = TipoIdPrefill is not null;

            if (hayPinPrefill)
            {
                ConsultaDevolucionesModel.Pin = PinPrefill;
            }

            if (hayNumeroPrefill)
            {
                ConsultaDevolucionesModel.Documento = NumeroIdPrefill;
            }

            if (hayTipoPrefill)
            {
                ConsultaDevolucionesModel.TipoDocumento = TipoIdPrefill;
            }

            if (hayPinPrefill && hayNumeroPrefill && hayTipoPrefill && !AutoSubmitEjecutado)
            {
                AutoSubmitEjecutado = true;
                await NotifyValidationStateChanged();
                if (EditContext.Validate())
                {
                    await OnValidSubmit();
                }
            }
            // Fin código generado por GitHub Copilot
        }

        private void LimpiarFormulario()
        {
            ConsultaDevolucionesModel = new ConsultaDevolucionesModel();

            hasSelectedTipoDocumento = false;
            DocumentoValido = false;
            caracteresValidos = false;
            EditContext?.NotifyValidationStateChanged();
        }
    }
}

