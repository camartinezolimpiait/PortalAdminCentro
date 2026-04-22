using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Credenciales
{
    public partial class Credenciales
    {
        #region Properties

        [Parameter]
        public string IdRunt { get; set; }

        [Parameter]
        public string UsuarioPortal { get; set; }

        [Parameter]
        public ConfigurarCredencialesModel ConfigurarCredencialesModel { get; set; } = new();

        [Parameter]
        public EventCallback<object> UpdateModel { get; set; }

        #endregion Properties

        #region Inyección Dependencias

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Inyección Dependencias

        #region Variables

        private EditContext EditContext;
        private ValidationMessageStore MessageStore;
        private bool IsLoading = true;
        private bool IsFormValid = false;

        #endregion Variables

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(ConfigurarCredencialesModel.Usuario) || string.IsNullOrEmpty(ConfigurarCredencialesModel.Clave))
            {
                ConfigurarCredencialesModel = new();
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "No se han encontrado los datos asociados al centro");
            }

            EditContext = new EditContext(ConfigurarCredencialesModel);
            MessageStore = new ValidationMessageStore(EditContext);

            IsLoading = false;

            if (!string.IsNullOrEmpty(ConfigurarCredencialesModel.Usuario) || !string.IsNullOrEmpty(ConfigurarCredencialesModel.Clave))
                await NotifyValidationStateChanged();
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task HandleInputChange(string fieldName, ChangeEventArgs e)
        {
            await NotifyValidationStateChanged();
        }

        private async Task NotifyValidationStateChanged()
        {
            var isValid = EditContext.Validate();

            StateHasChanged();
            IsFormValid = isValid;

            await Task.FromResult(true);
        }

        private async Task ValidarCredenciales()
        {
            await NotifyValidationStateChanged();

            if (IsFormValid)
            {
                CredencialesProveedor credenciales = new()
                {
                    IdRunt = $"{IdRunt ?? "0"}",
                    Usuario = ConfigurarCredencialesModel.Usuario,
                    Clave = ConfigurarCredencialesModel.Clave,
                    UsuarioPortal = UsuarioPortal
                };

                FacturacionResponse<EstadoCredenciales> response = await MiLicenciaService.ValidarEstadoCredencial(credenciales);

                if (response.SolicitudExitosa)
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Success, response.Mensaje);
                    await UpdateModel.InvokeAsync(ConfigurarCredencialesModel);
                }
                else
                    await response.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
            }
            else
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, "Hay validaciones en el formulario que no se están cumpliendo");
        }

        #endregion Private Methods
    }
}