using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Numeracion
{
    public partial class Numeracion
    {
        #region Properties

        [Parameter]
        public string IdRunt { get; set; }

        [Parameter]
        public string UserName { get; set; } = string.Empty;

        [Parameter]
        public ConfigurarNumeracionModel ConfigurarNumeracionModel { get; set; } = new();

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

        #endregion Fields



        #region Properties

        public List<Departamentos> Departamentos { get; set; } = [];
        public List<Municipios> Municipios { get; set; } = [];

        #endregion Properties

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(ConfigurarNumeracionModel.NumeroResolucion) || ConfigurarNumeracionModel.Desde == 0 || ConfigurarNumeracionModel.Hasta == 0)
            {
                ConfigurarNumeracionModel = new();
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Success, "No se han encontrado los datos asociados al centro");
            }

            EditContext = new EditContext(ConfigurarNumeracionModel);
            MessageStore = new ValidationMessageStore(EditContext);

            if (!string.IsNullOrEmpty(ConfigurarNumeracionModel.NumeroResolucion) || ConfigurarNumeracionModel.Desde != 0 || ConfigurarNumeracionModel.Hasta != 0)
            {
                if (ConfigurarNumeracionModel.EmpezarDesde > ConfigurarNumeracionModel.Hasta || ConfigurarNumeracionModel.EmpezarDesde < ConfigurarNumeracionModel.Desde)
                    ConfigurarNumeracionModel.EmpezarDesde = ConfigurarNumeracionModel.Desde;

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
                case "empezarDesde":
                    int consecutivo = int.Parse(e.Value.ToString());

                    if (consecutivo >= ConfigurarNumeracionModel.Desde && consecutivo <= ConfigurarNumeracionModel.Hasta)
                        ConfigurarNumeracionModel.EmpezarDesde = consecutivo;
                    else
                        MessageStore.Add(() => ConfigurarNumeracionModel.EmpezarDesde, "Empezar desde debe estas dentro del rango definido entre Desde y Hasta.");
                    break;

                case "desde":
                    ConfigurarNumeracionModel.EmpezarDesde = ConfigurarNumeracionModel.Desde;
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
                DatosNumeracion datosNumeracion = new()
                {
                    IdRunt = $"{IdRunt ?? "0"}",
                    NumeroResolucion = $"{ConfigurarNumeracionModel.NumeroResolucion}",
                    FechaInicio = new DateTime(ConfigurarNumeracionModel.FechaInicio.Year, ConfigurarNumeracionModel.FechaInicio.Month, ConfigurarNumeracionModel.FechaInicio.Day, 0, 0, 0, DateTimeKind.Local),
                    FechaFin = new DateTime(ConfigurarNumeracionModel.Fechafin.Year, ConfigurarNumeracionModel.Fechafin.Month, ConfigurarNumeracionModel.Fechafin.Day, 0, 0, 0, DateTimeKind.Local),
                    Prefijo = $"{ConfigurarNumeracionModel.Prefijo}",
                    NumeroDesde = $"{ConfigurarNumeracionModel.Desde}",
                    NumeroHasta = $"{ConfigurarNumeracionModel.Hasta}",
                    EmpezarDesde = ConfigurarNumeracionModel.ConsecutivoEspecifico,
                    NumeroEmpezarDesde = ConfigurarNumeracionModel.EmpezarDesde,
                    UsuarioPortal = $"{UserName}"
                };

                FacturacionResponse<object> responseEmision = await MiLicenciaService.AlmacenamientoDatosNumeracion(datosNumeracion);

                if (responseEmision.SolicitudExitosa)
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Success, responseEmision.Mensaje);
                    await UpdateModel.InvokeAsync(ConfigurarNumeracionModel);
                }
                else
                    await responseEmision.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
            }
            else
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, "Hay validaciones en el formulario que no se están cumpliendo");
        }

        #endregion Private Methods
    }
}