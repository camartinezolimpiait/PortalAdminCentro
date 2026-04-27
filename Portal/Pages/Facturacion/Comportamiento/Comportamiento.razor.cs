using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Comportamiento
{
    public partial class Comportamiento
    {
        #region Properties

        [Parameter]
        public string IdRunt { get; set; }

        [Parameter]
        public string UserName { get; set; } = string.Empty;

        [Parameter]
        public List<ConsultaGenericaTipos> DisparadoresFacturacion { get; set; } = [];

        [Parameter]
        public ConfigurarComportamientoModel ConfigurarComportamientoModel { get; set; } = new();

        [Parameter]
        public EventCallback<object> UpdateModel { get; set; }

        #endregion Properties

        #region Inyección Dependencias

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Inyección Dependencias

        #region Fields

        private bool IsLoading = true;
        private bool IsFormValid = false;

        private EditContext EditContext;
        private ValidationMessageStore MessageStore;
        public List<EnumEventoFacturacion> ListaEventos { get; set; } = [EnumEventoFacturacion.RecaudoPIN, EnumEventoFacturacion.UsoPIN];
        public List<Departamentos> Departamentos { get; set; } = [];
        public List<Municipios> Municipios { get; set; } = [];

        #endregion Fields

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            if (!ConfigurarComportamientoModel.FacturacionActiva || string.IsNullOrEmpty(ConfigurarComportamientoModel.EventoFacturacion))
            {
                ConfigurarComportamientoModel = new();
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "No se han encontrado los datos asociados al centro");
            }

            EditContext = new EditContext(ConfigurarComportamientoModel);
            MessageStore = new ValidationMessageStore(EditContext);

            if (!string.IsNullOrEmpty(ConfigurarComportamientoModel.EventoFacturacion))
                await NotifyValidationStateChanged();

            IsLoading = false;
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task HandleInputChange(string fieldName, ChangeEventArgs e)
        {
            switch (fieldName)
            {
                case "eventoFacturacion":
                    ConfigurarComportamientoModel.EventoFacturacion = e.Value.ToString();
                    await NotifyValidationStateChanged();
                    break;
            }

            await NotifyValidationStateChanged();
        }

        private async Task NotifyValidationStateChanged()
        {
            var isValid = EditContext.Validate();

            StateHasChanged();
            IsFormValid = isValid;

            await Task.FromResult(true);
        }

        private async Task GuardarDatos()
        {
            await NotifyValidationStateChanged();

            if (IsFormValid)
            {
                DatosComportamiento datosComportamiento = new()
                {
                    IdRunt = $"{IdRunt ?? "0"}"/* $"{972674}"*/,
                    ActivarFacturacion = ConfigurarComportamientoModel.FacturacionActiva,
                    CodigoMomentoFacturacion = ConfigurarComportamientoModel.EventoFacturacion,
                    UsuarioPortal = $"{UserName}"
                };

                FacturacionResponse<object> responseEmision = await MiLicenciaService.AlmacenamientoComportamiento(datosComportamiento);

                if (responseEmision.SolicitudExitosa)
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Success, responseEmision.Mensaje);
                    await UpdateModel.InvokeAsync(ConfigurarComportamientoModel);
                }
                else
                    await responseEmision.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
            }
            else
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "Hay validaciones en el formulario que no se están cumpliendo");
        }

        #endregion Private Methods
    }
}

