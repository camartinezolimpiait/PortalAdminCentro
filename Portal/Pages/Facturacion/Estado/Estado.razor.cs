using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Estado
{
    public partial class Estado
    {
        #region Inyección Dependencias

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Inyección Dependencias

        #region Properties

        [Parameter]
        public string IdRunt { get; set; }

        [Parameter]
        public bool Proveedor { get; set; }

        [Parameter]
        public bool IsConfiguration { get; set; } = false;

        #endregion Properties

        #region Fields

        private RespuestaErrorFacturacion ErrorFacturacion;
        private bool IsLoading = true;

        #endregion Fields

        #region Protected Methods

        // Inicio refactorización/optimización por GitHub Copilot
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await GetInfoStatus();
            }
            catch (Exception ex)
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"Ocurrió un error al consultar el estado: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Fin refactorización/optimización por GitHub Copilot

        #endregion Protected Methods

        #region Private Methods

        // Inicio refactorización/optimización por GitHub Copilot
        private async Task GetInfoStatus()
        {
            IsLoading = true;
            StateHasChanged();

            try
            {
                var responseEstado = await MiLicenciaService.ConsultarEstadoSistema(new() { IdRunt = IdRunt });
                if (responseEstado.SolicitudExitosa)
                {
                    ErrorFacturacion = responseEstado.Datos;
                }
                else
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"No se pudo obtener el estado del sistema. | {responseEstado.Mensaje}");
                }
            }
            catch (Exception ex)
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"Error al obtener el estado: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        // Fin refactorización/optimización por GitHub Copilot

        private async Task UpdateStatus() => await GetInfoStatus();

        #endregion Private Methods
    }
}
