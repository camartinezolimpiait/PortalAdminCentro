// Inicio código generado por GitHub Copilot
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Common
{
    public partial class ModalDetalleFacturacion : ComponentBase
    {
        #region Inyección de Dependencias

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        #endregion Inyección de Dependencias

        #region Parámetros del Componente

        /// <summary>
        /// IdPtesaPIN (viene del PINDetail del grid)
        /// </summary>
        [Parameter]
        public int IdPtesaPIN { get; set; }

        /// <summary>
        /// ID único del modal para DaisyUI
        /// </summary>
        [Parameter]
        public string ModalId { get; set; } = "modal-detalle-facturacion";

        /// <summary>
        /// Callback que se ejecuta cuando se cierra el modal
        /// </summary>
        [Parameter]
        public EventCallback OnCerrar { get; set; }

        #endregion Parámetros del Componente

        #region Variables Privadas

        /// <summary>
        /// Indica si el modal está cargando datos
        /// </summary>
        private bool IsLoading { get; set; }

        /// <summary>
        /// Datos del historial de la facturación obtenidos del servicio
        /// </summary>
        private HistoricData DetalleFacturacion { get; set; }

        // Inicio refactorización/optimización por GitHub Copilot
        /// <summary>
        /// Indica si el modal está actualmente abierto.
        /// </summary>
        private bool _isOpen { get; set; } = false;
        // Fin refactorización/optimización por GitHub Copilot

        #endregion Variables Privadas

        #region Métodos Públicos

        /// <summary>
        /// Abre el modal y carga los datos del historial
        /// </summary>
        /// <param name="idPtesaPIN">ID de la petición del PIN</param>
        public async Task AbrirModalAsync(int idPtesaPIN)
        {
            // Método generado por GitHub Copilot
            IdPtesaPIN = idPtesaPIN;

            // Inicio refactorización/optimización por GitHub Copilot
            _isOpen = true;
            // Fin refactorización/optimización por GitHub Copilot
            await CargarDetalleAsync();
            await JsRuntime.InvokeVoidAsync("eval", $"document.getElementById('{ModalId}').showModal()");
        }

        /// <summary>
        /// Cierra el modal y limpia los datos
        /// </summary>
        public async Task CerrarModal()
        {
            // Método generado por GitHub Copilot
            // Inicio refactorización/optimización por GitHub Copilot
            // Se marca el modal como cerrado ANTES de limpiar DetalleFacturacion,
            _isOpen = false;
            // Fin refactorización/optimización por GitHub Copilot
            await JsRuntime.InvokeVoidAsync("eval", $"document.getElementById('{ModalId}').close()");
            DetalleFacturacion = null;
            await OnCerrar.InvokeAsync();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        /// <summary>
        /// Carga el historial desde el servicio GetElectronicBillingRequestDetail
        /// </summary>
        private async Task CargarDetalleAsync()
        {
            // Método generado por GitHub Copilot
            IsLoading = true;
            DetalleFacturacion = null;
            StateHasChanged();

            try
            {
                // Llamar ÚNICAMENTE al servicio GetElectronicBillingRequestDetail con IdPtesaPIN
                var response = await MiLicenciaService.GetElectronicBillingRequestDetail(IdPtesaPIN);

                if (response?.SolicitudExitosa == true && response.Datos?.Datos != null)
                {
                    // Asignar los datos del HistoricData
                    DetalleFacturacion = response.Datos.Datos;
                }
                else
                {
                    // Mostrar mensaje de error
                    var mensaje = response?.Mensaje ?? "No se pudo cargar el historial de la solicitud.";
                    await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, mensaje);
                }
            }
            catch (Exception ex)
            {
                await MiLicenciaService.ShowNotificacion(
                    NotificationStatus.Error,
                    $"Error al cargar el historial: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Obtiene la clase CSS del badge según el estado
        /// </summary>
        private string GetEstadoBadgeClass(string estado)
        {
            // Método generado por GitHub Copilot
            return estado?.ToLower() switch
            {
                "en error de configuración" => "badge-warning",
                "en error de conexión" => "badge-warning",
                "registrado" => "badge-info",
                "listo para procesar" => "badge-info",
                "encolados" => "badge-info",
                "procesando" => "badge-info",
                "facturada" => "badge-success",
                "anulado" => "badge-neutral",
                _ => "badge"
            };
        }

        /// <summary>
        /// Mapea el estado del backend al nombre de la pestaña correspondiente del frontend
        /// Homologa múltiples estados del API a las 4 pestañas principales
        /// </summary>
        /// <param name="estadoBackend">Estado que viene de la respuesta del API</param>
        /// <returns>Nombre de la pestaña a mostrar en la columna Estado</returns>
        private string MapearEstadoAPestana(string estadoBackend)
        {
            // Inicio refactorización/optimización por GitHub Copilot
            var estadoEnum = ElectronicBillingStatus.MapearDesdeEstadoBackend(estadoBackend);
            return ElectronicBillingStatus.GetEstadoDisplayName(estadoEnum);
            // Fin refactorización/optimización por GitHub Copilot
        }

        #endregion Métodos Privados
    }
}
// Fin código generado por GitHub Copilot

