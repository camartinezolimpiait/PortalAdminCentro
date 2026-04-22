using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Entidades.Recaptcha;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace portalAdministrativoSISEC.Pages.Devolucion
{
    public partial class Devolucion
    {
        #region Inyeccion Dependencias

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private IConfiguration _configuration { get; set; }

        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        private ApplicationSevice MenuService = new();
        private GetDataResponseCentro GetCentroResponse = new();
        private readonly Centro CentroSeleccionado = new();

        private int pasosCompraPin;
        private bool isLoading = false;
        private bool IsLoadingData = false;

        private bool DevolucionRealizada = false;
        private bool ShowValidation = false;
        private bool AllowReturn = false;
        private bool isFormComplete = false;

        private bool prueba = false;
        public List<TipoDocumentoPtesaDTO> ListaDocumentos { get; set; } = [];

        private RespuestaRecaptchaDinamica<ConsultaInfoPinDto> ConsultaInfoPin = new();

        private ConsultaDevolucionPorPinRequest DevolucionPorPinRequest { get; set; } = new();

        #endregion Variables

        #region Metodos

        /// <summary>
        /// Inicializa el componente, recupera la sesión y la información del centro desde el almacenamiento protegido.
        /// </summary>
        /// <returns>Una tarea que representa la operación de inicialización.</returns>
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService");
            MenuService = protectedSessionStore.Value;

            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            GetCentroResponse = centroShared.Value.Respuesta;

            //await ValidarPermisos();
            isLoading = false;

            CentroSeleccionado.IdCentro = GetCentroResponse.IdCentro;
            CentroSeleccionado.Nombre = GetCentroResponse.Nombre;
            CentroSeleccionado.IdComercio = GetCentroResponse.IdComercio;
            CentroSeleccionado.IdDepartamento = GetCentroResponse.IdDepartamento;
            CentroSeleccionado.IdMunicipio = GetCentroResponse.IdMunicipio;
            CentroSeleccionado.IdZona = GetCentroResponse.IdZona;
            CentroSeleccionado.Direccion = GetCentroResponse.Direccion;
            CentroSeleccionado.Email = GetCentroResponse.Email;
            CentroSeleccionado.Fijo = GetCentroResponse.Fijo;
            CentroSeleccionado.Movil = GetCentroResponse.Movil;
            CentroSeleccionado.Latitud = GetCentroResponse.Latitud;
            CentroSeleccionado.Longitud = GetCentroResponse.Longitud;
            CentroSeleccionado.CodigoRUNT = GetCentroResponse.CodigoRUNT;
        }

        /// <summary>
        /// Se ejecuta cuando los parámetros del componente cambian.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }
        // Inicio refactorización/optimización por GitHub Copilot
        /// <summary>
        /// Realiza una consulta asincrónica para validar y recuperar información sobre una solicitud de reembolso basada en PIN, 
        /// actualizando las propiedades de estado relevantes según el resultado..
        /// </summary>
        /// <remarks>Este método deshabilita las acciones de validación y devolución durante la carga de datos y muestra 
        /// notificaciones de error si no se genera el Captcha de Google o si no se encuentra o no se aprueba el PIN. 
        /// Tras una validación exitosa, actualiza la información del reembolso y habilita las acciones de devolución 
        /// si este se originó desde el portal administrativo..</remarks>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea se completa cuando la consulta de reembolso y las actualizaciones de estado relacionadas 
        /// han finalizado.</returns>
        private async Task ConsultaDevolucionPin()
        {
            ShowValidation = false;
            AllowReturn = false;
            IsLoadingData = true;

            try
            {
                var clientsId = MenuService.Plataforma == nameof(EnumTipoCliente.CEA)
                    ? ((int)EnumTipoCliente.CEA).ToString()
                    : ((int)EnumTipoCliente.CRC).ToString();

                var googleCaptcha = await GenerateGoogleCaptcha();

                if (string.IsNullOrEmpty(googleCaptcha))
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, "No se genero el Captcha de google correctamente recarge su explorador e intentelo de nuevo.");
                    return;
                }

                var consultaRequest = new ConsultaEstadoDevolucion
                {
                    IdCliente = clientsId,
                    TipoIdentificacion = DevolucionPorPinRequest.TipoIdentificacion,
                    NumeroIdentificacion = DevolucionPorPinRequest.NumeroIdentificacion,
                    PinComprado = DevolucionPorPinRequest.Pin,
                    IdOrigenCotizacion = (int)EnumOrigenCotizacion.PortalAdministrativo,
                    IdRunt = GetCentroResponse.CodigoRUNT.ToString()
                };

                ConsultaInfoPin = await MiLicenciaService.DevolucionByPin<ConsultaInfoPinDto>(DevolucionPorPinRequest, false);
                var respuestaAcciones = await MiLicenciaService.ConsultaEstadoDevolucion(consultaRequest, googleCaptcha);

                if (!(respuestaAcciones?.Aprobado ?? false))
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, respuestaAcciones?.Texto ?? "No se encontró información del PIN para procesar la devolución.");
                    ShowValidation = !(ConsultaInfoPin?.Data?.Entidad?.IdOrigenCotizacion == (int)EnumOrigenCotizacion.PortalAdministrativo);
                    return;
                }
                
                await ValidarConsultaInfoPin();
                AllowReturn = ConsultaInfoPin?.Data?.Entidad?.IdOrigenCotizacion == (int)EnumOrigenCotizacion.PortalAdministrativo;
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        // Fin refactorización/optimización por GitHub Copilot

        // Inicio refactorización/optimización por GitHub Copilot
        /// <summary>
        /// Ejecuta la devolución del PIN consultado.
        /// </summary>
        /// <remarks>
        /// Valida que la información del PIN exista y que el origen de la cotización sea el portal administrativo.
        /// Ejecuta la petición de anulación a través de <see cref="IMiLicenciaService"/> y muestra notificaciones
        /// según el resultado. Mantiene la bandera <see cref="DevolucionRealizada"/> en true solo si la operación fue exitosa.
        /// </remarks>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        private async Task RealizarDevolucion()
        {
            IsLoadingData = true;

            try
            {
                // Validar estado previo
                if (ConsultaInfoPin?.Data?.Entidad == null)
                {
                    ShowValidation = true;
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, "No se encontró información del PIN para procesar la devolución.");
                    return;
                }

                if (ConsultaInfoPin.Data.Entidad.IdOrigenCotizacion != (int)EnumOrigenCotizacion.PortalAdministrativo)
                {
                    ShowValidation = true;
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, "No es posible realizar la devolución: el PIN no pertenece al portal administrativo.");
                    return;
                }

                // Ejecutar la anulación
                var anulacion = await MiLicenciaService.DevolucionByPin<ResponseAnulacion>(DevolucionPorPinRequest, true);

                var mensaje = anulacion?.Data?.MensajeRespuestaField;
                var codigo = anulacion?.Data?.CodigoRespuestaField;

                if (anulacion == null || anulacion.Data == null || codigo != 0 || !string.Equals(mensaje, "OK", StringComparison.OrdinalIgnoreCase))
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, mensaje ?? "Error al procesar la devolución.");
                    return;
                }

                DevolucionRealizada = true;
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Success, "Devolución realizada correctamente.");
            }
            catch (Exception ex)
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"Error al realizar la devolución: {ex.Message}");
            }
            finally
            {
                IsLoadingData = false;
            }
        }
        // Fin refactorización/optimización por GitHub Copilot

        /// <summary>
        /// Valida la respuesta de la consulta del PIN y muestra notificaciones en caso de error o estados no válidos.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica de validación.</returns>
        private async Task ValidarConsultaInfoPin()
        {
            if (ConsultaInfoPin.Data.Codigo == -1 || ConsultaInfoPin.Data.Entidad == null)
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, ConsultaInfoPin.Data.Respuesta);

            if (ConsultaInfoPin.Data?.Entidad?.EstadoPin != (int)EnumEstadoPin.Activo && ConsultaInfoPin.Data.Entidad != null)
            {
                string estadoPin = PagoPinConst.DescripcionEstadoPin[(EnumEstadoPin)(ConsultaInfoPin.Data?.Entidad?.EstadoPin ?? (int)EnumEstadoPin.ReferenciaVencida)];
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"El PIN ingresado esta en estado {estadoPin} Solo puede realizar la devolución de pines recaudados que aún no han sido usados.");
            }
        }

        // Método documentado por GitHub Copilot
        /// <summary>
        /// Construye y devuelve una descripción legible del trámite y la categoría asociada al PIN consultado.
        /// </summary>
        /// <remarks>
        /// - Si la información de la consulta no está disponible, devuelve "NO SET".
        /// - Si la categoría contiene el carácter 'I', se utiliza una descripción especial según el tipo de trámite
        ///   (PrimeraVez o Recategorizar). En caso contrario, se usa la descripción por defecto del trámite.
        /// - Depende de <see cref="ConsultaInfoPin"/> y de <see cref="PagoPinConst"/> para obtener las descripciones.
        /// </remarks>
        /// <returns>Cadena con la descripción del trámite y la categoría, o "NO SET" si no hay datos válidos.</returns>
        private string SetTramiteCategoria()
        {
            if (ConsultaInfoPin.Ok)
            {
                return ConsultaInfoPin.Data.Entidad.Categoria.Contains('I')
                    ? (ConsultaInfoPin.Data.Entidad.TipoTramite switch
                    {
                        (int)EnumTramite.PrimeraVez => $"{PagoPinConst.DescripcionTramite[EnumTramite.PrimeraVez]} " +
                            $"{ConsultaInfoPin.Data.Entidad.Categoria}",

                        (int)EnumTramite.Recategorizar => $"{PagoPinConst.DescripcionTramite[EnumTramite.Recategorizar]} " +
                            $"{ConsultaInfoPin.Data.Entidad.Categoria}",

                        _ => "NO SET"
                    })
                    : $"{PagoPinConst.DescripcionTramite[(EnumTramite)ConsultaInfoPin.Data.Entidad.TipoTramite]} " +
                    $"{ConsultaInfoPin.Data.Entidad.Categoria}";
            }

            return "NO SET";
        }

        /// <summary>
        /// Navega a la página de compra de PINs.
        /// </summary>
        private void VolverCompraPin()
        {
            NavManager.NavigateTo("/compradepin", false);
        }

        /// <summary>
        /// Actualiza el request de consulta de devoluciones cuando cambia el formulario.
        /// </summary>
        /// <param name="devolucionPorPinRequest">Objeto con los datos del formulario de consulta.</param>
        /// <returns>Una tarea completada inmediatamente.</returns>
        private async Task ConsultaDevolucionesChanged(ConsultaDevolucionPorPinRequest devolucionPorPinRequest)
        {
            DevolucionPorPinRequest = devolucionPorPinRequest;
            await Task.FromResult(true);
        }

        /// <summary>
        /// Maneja el cambio de estado del formulario (completo/incompleto) y solicita renderizado.
        /// </summary>
        /// <param name="isComplete">Indicador de si el formulario está completo.</param>
        private void OnFormCompleteChanged(bool isComplete)
        {
            isFormComplete = isComplete;
            StateHasChanged();
        }

        /// <summary>
        /// Formatea un valor decimal como moneda con separadores de miles precedido por el símbolo '$'.
        /// </summary>
        /// <param name="valor">Valor decimal a formatear.</param>
        /// <returns>Cadena formateada en moneda o cadena vacía si el valor es menor o igual a cero.</returns>
        public static string FormatearMoneda(decimal valor)
        {
            if (valor > 0)
                return string.Format("$ {0:N0}", valor);
            return "";
        }

        // Inicio refactorización/optimización por GitHub Copilot
        /// <summary>
        /// Genera un token válido de Google reCAPTCHA para proteger el flujo de devolución de PINes.
        /// </summary>
        /// <remarks>
        /// Verifica la existencia de la clave del sitio en la configuración, solicita la ejecución del script de reCAPTCHA
        /// mediante JavaScript y notifica cualquier error detectado durante el proceso, devolviendo <see cref="string.Empty"/>
        /// cuando el token no se puede obtener.
        /// </remarks>
        /// <returns>
        /// El token reCAPTCHA generado; si ocurre algún problema durante la generación, se devuelve <see cref="string.Empty"/>.
        /// </returns>
        public async Task<string> GenerateGoogleCaptcha()
        {
            const string captchaErrorMessage = "No se genero el Captcha de google correctamente recarge su explorador e intentelo de nuevo.";

            try
            {
                var siteKey = _configuration["AppSettings:SecretKey"];

                if (string.IsNullOrWhiteSpace(siteKey))
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, "No se encontró la clave del sitio para reCAPTCHA.");
                    return string.Empty;
                }

                var recaptchaToken = await _jsRuntime.InvokeAsync<string>("runCaptcha");

                if (string.IsNullOrWhiteSpace(recaptchaToken))
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, captchaErrorMessage);
                    return string.Empty;
                }

                return recaptchaToken;
            }
            catch (JSException jsEx)
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"Error de seguridad: {jsEx.Message}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"Error: {ex.Message}");
                return string.Empty;
            }
        }
        // Fin refactorización/optimización por GitHub Copilot

        #endregion Metodos
    }
}