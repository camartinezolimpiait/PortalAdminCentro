using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Emision
{
    public partial class Emision
    {
        #region Properties

        [Parameter]
        public string IdRunt { get; set; }

        [Parameter]
        public List<ConsultaGenericaTipos> Regimen { get; set; } = [];

        [Parameter]
        public List<ConsultaGenericaTipos> Departamentos { get; set; } = [];

        [Parameter]
        public List<ConsultaMunicipios> Municipios { get; set; } = [];

        [Parameter]
        public List<ConsultaGenericaTipos> TiposPersona { get; set; } = [];

        [Parameter]
        public string UserName { get; set; } = string.Empty;

        [Parameter]
        public ConfigurarEmisionModel ConfigurarEmisionModel { get; set; } = new();

        [Parameter]
        public EventCallback<object> UpdateModel { get; set; }

        #endregion Properties

        #region Inyeccion Dependencias

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Inyeccion Dependencias

        #region Fields

        private bool IsLoading = true;
        private bool IsFormValid = false;

        private GetDataResponseCentro GetCentroResponse = new();
        private EditContext EditContext;
        private ValidationMessageStore MessageStore;

        #endregion Fields

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            ProtectedBrowserStorageResult<GetCentroResponse> centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            GetCentroResponse = centroShared.Value.Respuesta;

            if (string.IsNullOrEmpty(ConfigurarEmisionModel.NIT) || string.IsNullOrEmpty(ConfigurarEmisionModel.Dv) || string.IsNullOrEmpty(ConfigurarEmisionModel.NombreComercial))
            {
                ConfigurarEmisionModel = new();
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "No se han encontrado los datos asociados al centro");
            }

            EditContext = new EditContext(ConfigurarEmisionModel);
            MessageStore = new ValidationMessageStore(EditContext);

            if (!string.IsNullOrEmpty(ConfigurarEmisionModel.NIT) || !string.IsNullOrEmpty(ConfigurarEmisionModel.Dv) || !string.IsNullOrEmpty(ConfigurarEmisionModel.NombreComercial))
            {
                GetCentroResponse.IdMunicipio = ConfigurarEmisionModel.Ciudad ?? 0;
                await NotifyValidationStateChanged();
            }

            IsLoading = false;
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task HandleInputChange(string fieldName, ChangeEventArgs e)
        {
            MessageStore.Clear();
            switch (fieldName)
            {
                case "departamento":
                    ConfigurarEmisionModel.Departamento = int.Parse(e.Value.ToString());
                    ConfigurarEmisionModel.Ciudad = 0;

                    if (ConfigurarEmisionModel.Ciudad == 0)
                        MessageStore.Add(() => ConfigurarEmisionModel.Ciudad, "Seleccione la ciudad donde se encuentra el centro.");
                    break;

                case "ciudad":
                    ConfigurarEmisionModel.Ciudad = int.Parse(e.Value.ToString());
                    break;

                case "TipoPersona":
                    ConfigurarEmisionModel.TipoPersona = int.Parse(e.Value.ToString());
                    break;

                case "tipoRegimen":
                    ConfigurarEmisionModel.RegimenContributivo = Regimen[int.Parse(e.Value.ToString())]?.Codigo;
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
                DatosEmision datosEmision = new()
                {
                    IdRunt = $"{IdRunt ?? "0"}"/* $"{78964321}"*/,
                    CodigoTipoPersona = $"{ConfigurarEmisionModel.TipoPersona ?? 0}",
                    RazonSocial = ConfigurarEmisionModel.RazonSocial,
                    Nit = ConfigurarEmisionModel.NIT,
                    Dv = ConfigurarEmisionModel.Dv,
                    CodigoRegimenContributivo = ConfigurarEmisionModel.RegimenContributivo,
                    NombreComercialEstablecimiento = ConfigurarEmisionModel.NombreComercial,
                    CorreoElectronico = ConfigurarEmisionModel.Correo,
                    Celular = ConfigurarEmisionModel.Celular,
                    DireccionEstablecimiento = ConfigurarEmisionModel.Direccion,
                    CodigoDepartamento = $"{ConfigurarEmisionModel.Departamento}",
                    CodigoCiudad = $"{ConfigurarEmisionModel.Ciudad}",
                    Nota = ConfigurarEmisionModel.Observaciones,
                    UsuarioPortal = UserName,
                };
                FacturacionResponse<object> responseEmision = await MiLicenciaService.AlmacenamientoDatosEmision(datosEmision);

                if (responseEmision.SolicitudExitosa)
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Success, responseEmision.Mensaje);
                    await UpdateModel.InvokeAsync(ConfigurarEmisionModel);
                }
                else
                    await responseEmision.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
            }
            else
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, "Hay validaciones en el formulario que no se estan cumpliendo");
        }

        #endregion Private Methods
    }
}

