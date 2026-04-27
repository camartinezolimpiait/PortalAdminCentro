using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.CompraPin.Models;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.Transaccion
{
    public partial class TransactionProgress : IDisposable
    {
        #region Fields

        private bool IsLoading = false;

        private string? mensajeCopiado;

        private bool IsUpdating = false;

        private string ButtonText = "Actualizar estado";

        private string ButtonClass = "flex items-center text-white text-sm font-medium bg-azul-600 hover:bg-azul-700 rounded-md px-3 py-1 transition-colors duration-150 cursor-pointer";

        private int RemainingSeconds = 0;

        private string CountdownDisplay = string.Empty;

        private string LastUpdateDisplay = "0 min";

        private System.Timers.Timer? CountdownTimer;

        private DateTime LastUpdateTime;

        private System.Timers.Timer TimeAgoTimer;

        private bool _disposed = false;

        #endregion Fields

        #region Properties

        [Parameter]
        public TransactionInfo transactionInfo { get; set; }

        [Parameter]
        public PagoPin PagoPin { get; set; }

        [Parameter]
        public EventCallback TransactionDone { get; set; }

        [Inject]
        private IMiLicenciaService _miLicenciaService { get; set; }

        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; }

        #endregion Properties

        #region Public Methods

        // Inicio refactorización/optimización por GitHub Copilot
        // Implementación del patrón Dispose para cumplir con S3881 y CA1816

        // Método generado por GitHub Copilot
        public void Dispose()
        {
            // Llamar a Dispose con true para liberar recursos administrados
            Dispose(true);
            // Suprimir la finalización si el recolector de basura lo llama
            GC.SuppressFinalize(this);
        }

        // Método generado por GitHub Copilot
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Liberar recursos administrados
                    CountdownTimer?.Dispose();
                    TimeAgoTimer?.Dispose();
                }
                // Liberar recursos no administrados aquí si los hubiera

                _disposed = true;
            }
        }

        // Fin refactorización/optimización por GitHub Copilot

        #endregion Public Methods

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            await IniciarProcesoAsync();
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task IniciarProcesoAsync()
        {
            // Cambiar el estado del botón a "actualizando"
            IsUpdating = true;
            ButtonText = "Actualizando...";
            ButtonClass = "flex items-center text-white text-sm font-medium bg-gris-400 rounded-md px-3 py-1 transition-colors duration-150 cursor-not-allowed";

            //  Llama tu método principal de consulta
            await ConsultarInformacionAsync();

            // Inicia el contador regresivo (120 segundos)
            RemainingSeconds = 120;
            StartCountdown();

            // Inicia el temporizador de 2 minutos
            _ = HabilitarBotonDespuesDeEsperaAsync();

            IsLoading = false;
        }

        private async Task CopiarNUT()
        {
            await JS.InvokeVoidAsync("copiarAlPortapapeles", PagoPin.Nut);
            mensajeCopiado = "¡Copiado al portapapeles!";

            StateHasChanged();
        }

        private async Task ConsultarInformacionAsync()
        {
            ResponseDTO<Entidad> response = await _miLicenciaService.ConsultaInfoPin<ResponseDTO<Entidad>>(new Entidades.Devolucion.ConsultaDevolucionPorPinRequest()
            {
                IdRunt = PagoPin.CentroSeleccionado.CodigoRUNT.ToString(),
                NumeroIdentificacion = PagoPin.Usuario.NumDocumento,
                Pin = PagoPin.PinGenerado,
                TipoIdentificacion = PagoPin.Usuario.TipoDocumento.Value
            });

            if (response?.Entidad == null)
            {
                return;
            }

            if (response.Entidad.EstadoPin == (int)EnumEstadoPin.Activo)
            {
                await IrAResumenCompra();
            }
        }

        private async Task HabilitarBotonDespuesDeEsperaAsync()
        {
            await Task.Delay(120000); // 2 minutos

            IsUpdating = false;
            ButtonText = "Actualizar estado";
            ButtonClass = "flex items-center text-white text-sm font-medium bg-azul-600 hover:bg-azul-700 rounded-md px-3 py-1 transition-colors duration-150 cursor-pointer";

            await InvokeAsync(StateHasChanged); // Forzar renderizado al finalizar el contador
        }

        private async Task ActualizarManualAsync()
        {
            if (IsUpdating)
                return;

            await IniciarProcesoAsync();
        }

        private void StartCountdown()
        {
            CountdownTimer?.Dispose();
            CountdownTimer = new System.Timers.Timer(1000);
            CountdownTimer.Elapsed += async (s, e) =>
            {
                if (RemainingSeconds > 0)
                {
                    RemainingSeconds--;
                    CountdownDisplay = $"{RemainingSeconds / 60:D2}:{RemainingSeconds % 60:D2}";
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    CountdownTimer?.Stop();
                    IsUpdating = false;
                    ButtonText = "Actualizar estado";
                    ButtonClass = "flex items-center text-white text-sm font-medium bg-azul-600 hover:bg-azul-700 rounded-md px-3 py-1 transition-colors duration-150 cursor-pointer";

                    // Guardar la hora de la última actualización
                    LastUpdateTime = DateTime.Now;
                    LastUpdateDisplay = "unos segundos";

                    // ?? Iniciar el actualizador automático del texto
                    StartTimeAgoUpdater();

                    await InvokeAsync(StateHasChanged);
                }
            };
            CountdownTimer.Start();
        }

        private async Task IrAResumenCompra()
        {
            await TransactionDone.InvokeAsync();
        }

        private void Salir()
        {
            Navigation.NavigateTo("/configuracion/PerfilMilicencia", forceLoad: true);
        }

        private void ComprarOtroPin()
        {
            Navigation.NavigateTo("/compradepin", forceLoad: true);
        }

        private void StartTimeAgoUpdater()
        {
            TimeAgoTimer?.Dispose();
            TimeAgoTimer = new System.Timers.Timer(30000); // cada 30 segundos
            TimeAgoTimer.Elapsed += async (s, e) =>
            {
                if (LastUpdateTime != default)
                {
                    LastUpdateDisplay = LastUpdateTime.GetTimeAgo();
                    await InvokeAsync(StateHasChanged);
                }
            };
            TimeAgoTimer.Start();
        }

        #endregion Private Methods
    }
}

