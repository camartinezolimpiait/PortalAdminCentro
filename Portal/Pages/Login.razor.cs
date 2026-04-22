using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Entidades;
using portalAdministrativoSISEC.Entidades.Recaptcha;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Services.Agendamiento.Perfil;
using portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Util.LogAuditoria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Util.Helpers;

namespace portalAdministrativoSISEC.Pages
{
    public partial class Login
    {
        #region Constructor

        [Inject]
        public ISuperTransporteService _superTransporteService { get; set; }
        [Inject]
        public IApiService _apiService { get; set; }

        [Inject]
        public IPerfilAgendaService _perfilAgendaService { get; set; }

        [Inject]
        public IPortalAdministrativoService _portalAdministrativoService { get; set; }

        [Inject]
        private IConfiguration _configuration { get; set; }

        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        [Inject]
        private HttpClient Http { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }

        [Inject]
        private ProtectedSessionStorage protectedSessionStore { get; set; }

        #endregion Constructor

        #region Variables

        private ApplicationSevice menuservice = new ApplicationSevice();
        private ApplicationShared applicationShared = new ApplicationShared();
        private UserData dataUser = new UserData();
        private List<ClienteDTO> clienteDtoGral = new List<ClienteDTO>();
        private string message = null;
        private string typeAlert = null;
        private string tempPassword = null;
        private bool showModal = false;
        private bool showLoader = false;
        [Parameter] public bool recoverPassword { get; set; }
        private string emailRecoverPass { get; set; }
        private string email { get; set; }
        private string google { get; set; }
        public bool isRecaptchaActive { get => (_configuration["AppSettings:IsRecaptchaActive"] ?? "").ToLower() == "true"; }

        #endregion Variables

        #region metodos

        public async Task RegisterInfoLogAsync(
        string endpoint,
        string httpMethod,
        string requestPayload,
        string responsePayload = null,
        string statusCode = null,
        LogLevel level = LogLevel.Info)
        {

            // Input validation
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint), "El endpoint de la API no puede ser nulo o vacío.");

            if (string.IsNullOrWhiteSpace(httpMethod))
                throw new ArgumentNullException(nameof(httpMethod), "El método HTTP no puede ser nulo ni estar vacío");

            try
            {
                var infoLog = new InfoLog
                {
                    LevelLog = level.ToString().ToUpperInvariant(),
                    ApiEndpointLog = endpoint.Trim(),
                    HttpMethodLog = httpMethod.Trim().ToUpperInvariant(),
                    RequestPayloadLog = requestPayload?.Trim(),
                    ResponsePayloadLog = responsePayload?.Trim(),
                    StatusCodeLog = statusCode?.Trim(),
                    TimestampLog = DateTime.UtcNow,
                    CorrelationKey = Guid.NewGuid().ToString()
                };

                await _portalAdministrativoService.RegisterInfoLog(infoLog)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log the error to a fallback mechanism (e.g., console, file, or another logger)
                // Note: In a real implementation, use a proper fallback logging mechanism
                string value = $"Error al registrar el log de información: {ex.Message}";
                Console.Error.WriteLine(value);

                // Optionally rethrow or handle based on requirements
                throw new InvalidOperationException("Failed to register info log.", ex);
            }

        }

        public async Task LogLoginAttemptAsync(string method, string httpMethod, string username, string platform)
        {
            await RegisterInfoLogAsync(
                endpoint: method,
                httpMethod: httpMethod,
                requestPayload: JsonConvert.SerializeObject(new { Username = username, Platform = platform }),
                responsePayload: JsonConvert.SerializeObject(new { Status = "Success" }),
                statusCode: "200",
                level: LogLevel.Info
            );
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                // Verificar si el captcha está activo
                if (firstRender)
                {
                    clienteDtoGral = await GetClients();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);

                // Implementacion de registro de exceptions
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
            }
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            try
            {
                google = await _jsRuntime.InvokeAsync<string>("runCaptcha");
                if (firstRender)
                {
                    StateHasChanged(); // Esto puede omitirse si no se necesita forzar una actualización.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                // Implementacion de registro de exceptions
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
            }
        }

        private async Task<HttpResponseMessage> SendAuthenticationRequest()
        {
            ApiResponse<object> response = await _apiService.CallApiAsync<object>(
                baseAddress: _configuration["AppSettings:uriSisecAuth"],
                apiEndpoint: "AspNetUser/AutenticarUsuario",
                method: HttpMethod.Post,
                content: dataUser,
                apiKey: "SisecAut"
            );

            return response.HttpResponse;
        }


        protected async Task AuthenticateUser()
        {
            HttpResponseMessage response = await SendAuthenticationRequest();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                DataUserLog dataUserLog = JsonConvert.DeserializeObject<DataUserLog>(content);
                if (dataUserLog.Respuesta)
                {
                    // Registramos el registro de login para testear funcionalidad automaticamente
                    await LogLoginAttemptAsync("AspNetUser/AutenticarUsuario", "POST", dataUser.userName, dataUser.plataforma);

                    ClienteDTO clientes = menuservice.listaClientes.FirstOrDefault(x => x.Nombre == dataUser.plataforma) ?? new();

                    menuservice.UserID = dataUserLog.Id;
                    menuservice.Autenticado = true;
                    menuservice.ShowCaptcha = isRecaptchaActive;
                    menuservice.Plataforma = dataUser.plataforma;
                    menuservice.ClienteId = clientes.Id;
                    menuservice.MustChangePassword = dataUserLog.ChangePassword;
                    menuservice.UserName = dataUser.userName;
                    await protectedSessionStore.SetAsync(menuservice.NameLocalStorage, menuservice);

                    applicationShared.IdPerfil = dataUserLog.IdPerfil;
                    applicationShared.Plataforma = dataUser.plataforma;
                    applicationShared.IdCentro = dataUserLog.InformacionUsuario.IdCentro;
                    applicationShared.UserName = dataUser.userName;

                    // Consulta idCentro en SISEC
                    GetCentroResponse respuestaCentro = await _perfilAgendaService.GetInformationCentroId(new GetCentroRequest
                    {
                        ID = applicationShared.IdCentro,
                        Plataforma = applicationShared.Plataforma
                    });

                    if (respuestaCentro == null || respuestaCentro.Respuesta == null)
                    {
                        message = "No se pudo obtener la información del centro asociado a este usuario. Por favor, comuníquese con el administrador del sistema.";
                        typeAlert = "danger";
                        showLoader = false;
                        return;
                    }
                    else
                    {
                        if (respuestaCentro.Respuesta.CodigoRUNT == null)
                        {
                            message = "El centro asociado a este usuario no tiene código RUNT asignado. Por favor, comuníquese con el administrador del sistema.";
                            typeAlert = "danger";
                            showLoader = false;
                            return;
                        }

                        applicationShared.IdRunt = respuestaCentro.Respuesta.CodigoRUNT.ToString();
                        await protectedSessionStore.SetAsync("centroStorage", respuestaCentro);

                        if (!applicationShared.Plataforma.Equals("Armas"))
                        {
                            await ConsultSuperTransporteInformation(respuestaCentro);
                        }

                        await protectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
                        navigation.NavigateTo("/configuracion/PerfilMilicencia", true);
                    }
                }
                else
                {
                    message = dataUserLog.MensajeError;
                    typeAlert = "danger";
                }
            }
            else
            {
                message = response.ReasonPhrase;
                typeAlert = "danger";
            }
            showLoader = false;
        }

        protected async Task ConsultSuperTransporteInformation(GetCentroResponse respuestaCentro)
        {
            try
            {
                var pluralPlatform = GetPluralPlatform(dataUser.plataforma.ToUpper());
                _superTransporteService.SetPlataforma(pluralPlatform);
                string plataformaStrappi = pluralPlatform;
                //Consulta idRunt en Strappi para validar si Centro existe
                var centroStrappy = await _superTransporteService.GetCentroByRunt((long)respuestaCentro?.Respuesta?.CodigoRUNT);
                if (centroStrappy != null)
                {
                    if (centroStrappy.data.Any())
                    {
                        applicationShared.IdCentroStrappi = centroStrappy.data.FirstOrDefault().id;

                        //Consulta Vigilado en Strappi por idCentro

                        if (centroStrappy.data[0].attributes.vigilado.data == null)
                        {
                            int idVigilado = await MonitoredVerification(respuestaCentro.Respuesta.IdComercio, dataUser.plataforma.ToUpper());
                            // Asignar vigilado a Centro
                            var resultVigilado = _superTransporteService.PutCentro(new
                            {
                                vigilado = new
                                {
                                    connect = new List<int>()
                                    {
                                        idVigilado,
                                    }.ToArray()
                                }
                            }, applicationShared.IdCentroStrappi);
                        }
                    }
                    else
                    {
                        int idVigilado = await MonitoredVerification(respuestaCentro.Respuesta.IdComercio, dataUser.plataforma.ToUpper());

                        var resultadoCreacion = await _superTransporteService.PostCentro(new
                        {
                            IDRUNT = respuestaCentro.Respuesta.CodigoRUNT.ToString(),
                            nombre = respuestaCentro.Respuesta.Nombre,
                            de_prueba = false
                        });

                        if (resultadoCreacion.data != null)
                            applicationShared.IdCentroStrappi = resultadoCreacion.data.id;
                        // Asigna el centro al vigilado
                        var resultVigilado = _superTransporteService.PutCentro(new
                        {
                            vigilado = new
                            {
                                connect = new List<int>()
                                    {
                                        idVigilado,
                                    }.ToArray()
                            }
                        }, applicationShared.IdCentroStrappi);
                    }
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";

                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";

                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
            }
        }

        protected async Task<int> MonitoredVerification(int idComercio, string plataforma)
        {
            int idVigilado = 0;
            DataVigilado resultado = new();
            try
            {
                //Consulta en SISEC  nit y datos del comercio por el idCentro
                GetComercioResponse respuestaComercio = await _perfilAgendaService.GetInformationComercio(new GetCentroRequest
                {
                    ID = idComercio,
                    Plataforma = plataforma
                });
                //Consulta en Strappi los datos del vigilado por NIT
                VigiladoFilterDto vigiladoFilterDto = new();
                vigiladoFilterDto = await _superTransporteService.GetVigilado(respuestaComercio?.Respuesta?.Nit);
                if (vigiladoFilterDto.data.Count > 0)
                {
                    resultado = new DataVigilado()
                    {
                        id = vigiladoFilterDto.data[0].id,
                        attributes = new AttributesVigilado()
                        {
                            NIT = vigiladoFilterDto.data[0].attributes.NIT,
                            razon_social = vigiladoFilterDto.data[0].attributes.razon_social
                        }
                    };

                    idVigilado = resultado.id;
                }
                else
                {
                    // Crea vigilado
                    int resultadoPost = await _superTransporteService.PostVigilado(new
                    {
                        NIT = respuestaComercio.Respuesta.Nit,
                        razon_social = respuestaComercio.Respuesta.RazonSocial,
                        de_prueba = false
                    });

                    if (resultadoPost > 0)
                    {
                        idVigilado = resultadoPost;
                    }
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));

                return 0;
            }
            return idVigilado;
        }

        protected async Task ChangePassword()
        {
            try
            {
                int longitud = 10;
                Guid miGuid = Guid.NewGuid();
                tempPassword = miGuid.ToString().Replace("-", string.Empty).Substring(0, longitud);

                ResetPassword resetPassword = new ResetPassword
                {
                    userName = dataUser.userName,
                    passWord = tempPassword,
                    plataforma = dataUser.plataforma,
                };

                ApiResponse<ResetPasswordResponse> response = await _apiService.CallApiAsync<ResetPasswordResponse>(
                    baseAddress: _configuration["AppSettings:uriSisecAuth"],
                    apiEndpoint: "AspNetUser/ReseteoContrasena/",
                    method: HttpMethod.Post,
                    content: resetPassword,
                    apiKey: "SisecAut"
                );

                if (response.IsSuccess)
                {
                    ResetPasswordResponse resetPasswordResponse = response.Content;

                    if (resetPasswordResponse.Respuesta)
                    {
                        await LogLoginAttemptAsync("AspNetUser/ReseteoContrasena/", "POST", dataUser.userName, dataUser.plataforma);
                        await GetEmailRecoverPass();
                    }
                    else
                    {
                        message = resetPasswordResponse.RespuestaTexto;
                        typeAlert = "danger";
                    }
                }
                else
                {
                    message = response.HttpResponse.ReasonPhrase;
                    typeAlert = "danger";
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType?.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(
                    ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null)
                );
            }
        }

        protected async Task GetEmailRecoverPass()
        {
            try
            {
                string apiName = "AspNetUser/ObtenerAspNetUserEmail/" + dataUser.userName;

                ApiResponse<EmailRecoverPassResponse> response = await _apiService.CallApiAsync<EmailRecoverPassResponse>(
                    baseAddress: _configuration["AppSettings:uriSisecAuth"],
                    apiEndpoint: apiName,
                    method: HttpMethod.Get,
                    content: null,
                    apiKey: "SisecAut"
                );

                if (response.IsSuccess)
                {
                    EmailRecoverPassResponse emailResponse = response.Content;

                    email = emailResponse.email;
                    emailRecoverPass = email.Substring(0, 3) + "***********" + email.Substring(email.IndexOf("@") - 2, email.Length - email.IndexOf("@") + 2);
                    showModal = true;
                    await SendEmailRecoverPass(emailResponse.userName);
                }
                else
                {
                    message = response.HttpResponse.ReasonPhrase;
                    typeAlert = "danger";
                }
            }
            catch (Exception ex)
            {
                message = "Ha ocurrido un error intentelo mas tarde.";
                typeAlert = "danger";

                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType?.Name ?? "Default";

                await _portalAdministrativoService.RegisterExceptionLog(
                    ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null)
                );
            }
        }

        // Método generado por GitHub Copilot
        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Envía un correo electrónico para recuperación de contraseña al usuario especificado.
        /// Valida todos los posibles errores por objetos nulos.
        /// </summary>
        /// <param name="nameClient">Nombre del cliente destinatario.</param>
        protected async Task SendEmailRecoverPass(string nameClient)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nameClient))
                {
                    message = "No se pudo enviar el correo de recuperación de contraseña.";
                    typeAlert = "danger";
                    return;
                }
                var destinatarios = new List<Destinatarios>
                {
                    new Destinatarios { NameDestinatario = email }
                };
                var dataEmail = new Dictionary<string, string>
                {
                    { "Password", tempPassword },
                    { "Cliente", nameClient }
                };
                var emailSender = new EmailSender
                {
                    CodigoPlantilla = ((int)PlantillasDeCorreo.RecuperarContrasena).ToString(),
                    Destinatarios = destinatarios,
                    LlaveValor = dataEmail,
                };
                var baseAddress = _configuration["AppSettings:uriSisecAuth"];
                if (string.IsNullOrWhiteSpace(baseAddress))
                {
                    message = "No se encuentra la URL base para el envío de correo.";
                    typeAlert = "danger";
                    return;
                }
                var response = await _apiService.CallApiAsync<object>(
                    baseAddress: baseAddress,
                    apiEndpoint: "Plantilla/EnviarCorreo",
                    method: HttpMethod.Post,
                    content: emailSender,
                    apiKey: "SisecAut"
                );

                // Validar la respuesta de la API y manejar posibles errores
                if (response?.HttpResponse?.IsSuccessStatusCode == true)
                {
                    message = "Correo de recuperación enviado exitosamente.";
                    typeAlert = "success";
                }
                else
                {
                    message = "No se pudo enviar el correo de recuperación de contraseña.";
                    typeAlert = "danger";
                }
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType?.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(
                    ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null)
                );
                message = "No se pudo enviar el correo de recuperación de contraseña.";
                typeAlert = "danger";
            }
        }
        // Fin código generado por GitHub Copilot


        protected async Task<List<ClienteDTO>> GetClients()
        {
            List<ClienteDTO> clienteDTO = new List<ClienteDTO>();

            try
            {
                ApiResponse<List<ClienteDTO>>? response = await _apiService.CallApiAsync<List<ClienteDTO>>(
                    baseAddress: @_configuration["AppSettings:uriSisecParametization"],
                    apiEndpoint: "Cliente/ObtenerClienteTodosAsync",
                    method: HttpMethod.Get,
                    content: null,
                    "SisecAdmin"
                    );

                if (response != null && response.HttpResponse != null && response.HttpResponse.IsSuccessStatusCode)
                {
                    clienteDTO = response.Content;
                }
                else
                {
                    message = "Falló la conexión con la base de datos";
                    typeAlert = "danger";
                    showLoader = false;
                }
                menuservice.listaClientes = clienteDTO;
            }
            catch (Exception ex)
            {
                message = "Ha ocurrido un error intentelo de nuevo.";
                typeAlert = "danger";
                showLoader = false;
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
            }

            return clienteDTO;
        }

        private static string GetPluralPlatform(string plataforma)
        {
            return plataforma switch
            {
                "CRC" => "crcs",
                "CEA" => "ceas",
                "CDA" => "cda",
                _ => "armas"
            };
        }

        private async Task OnSubmitLogin()
        {
            try
            {
                message = "";
                typeAlert = "";
                showLoader = true;

                ReCaptcha reCaptcha = await ValidateCaptcha(google);

                if (reCaptcha.Success.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    message = reCaptcha.ErrorCodes != null ? string.Join(", ", reCaptcha.ErrorCodes) : string.Empty;
                    typeAlert = "danger";
                    showLoader = false;
                    return;
                }
                await AuthenticateUser();
            }
            catch (Exception ex)
            {
                showLoader = false;
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                showLoader = false;
            }
        }

        private async Task OnSubmitRecoverPass()
        {
            try
            {
                message = "";
                typeAlert = "";
                showLoader = true;

                if (string.IsNullOrEmpty(dataUser.userName) || string.IsNullOrEmpty(dataUser.plataforma))
                {
                    message = "Por favor completa todos los campos";
                    typeAlert = "danger";
                    showLoader = false;
                    return;
                }
                await ChangePassword();
                showLoader = false;
            }
            catch (Exception ex)
            {
                showLoader = false;
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
            }
        }

        private async Task<ReCaptcha> ValidateCaptcha(string gResponse)
        {
            ReCaptcha reCaptcha = new();

            // Si el captcha no está activo, retornar true directamente
            if (!isRecaptchaActive)
            {
                reCaptcha.Success = "true";
                reCaptcha.ErrorCodes = new string[] { $"Token de reCAPTCHA válido" };
                return reCaptcha;
            }
            // Si no hay respuesta del captcha, rechazar
            if (string.IsNullOrEmpty(gResponse))
            {
                reCaptcha.Success = "false";
                reCaptcha.ErrorCodes = new string[] { $"Token de reCAPTCHA inválido, por favor refresca la pagina, intenta de nuevo" };

                return reCaptcha;
            }
            // Usar using declaration (C# 8+) para gestionar la disposición automática
            using var client = new HttpClient();
            string secretKey = @_configuration["AppSettings:SecretKey"];
            string verifyUrl = @_configuration["AppSettings:CaptchaVerifyUrl"];

            try
            {
                var requestUrl = $"{verifyUrl}?secret={secretKey}&response={gResponse}";
                var gReply = await client.GetAsync(requestUrl);

                if (!gReply.IsSuccessStatusCode)
                {
                    reCaptcha.Success = "false";
                    reCaptcha.ErrorCodes = new string[] { "El servicio de verificación de reCAPTCHA no responde. Por favor, intenta de nuevo." };
                    return reCaptcha;
                }

                var recaptchaResponse = await gReply.Content.ReadFromJsonAsync<RecaptchaResponse>();
                var responseContent = await gReply.Content.ReadAsStringAsync();
                var captchaResponse = JsonConvert.DeserializeObject<ReCaptcha>(responseContent);

                if (!recaptchaResponse.Success)
                {
                    string source = responseContent.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ? "Refresca la página." : string.Empty;
                    reCaptcha.Success = "false";
                    reCaptcha.ErrorCodes = new string[] { $"Token de reCAPTCHA inválido. {source} Por favor intenta de nuevo." };
                    return reCaptcha;
                }
                // La propiedad Success probablemente es un bool, no una string
                reCaptcha.Success = captchaResponse.Success;
                reCaptcha.Score = captchaResponse.Score;
                reCaptcha.ErrorCodes = captchaResponse.ErrorCodes;

                return reCaptcha;
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                // Considera usar un logger en lugar de Console.WriteLine
                Console.WriteLine($"Error validando reCAPTCHA: {ex.Message}");
                reCaptcha.Success = "false";
                reCaptcha.Score = "0";
                reCaptcha.ErrorCodes = new string[] { $"{ex.Message}" };
                return reCaptcha;
            }
        }

        private async Task ChangeMethod(string uri)
        {
            message = "";
            typeAlert = "";
            showModal = false;

            navigation.NavigateTo(uri, false);
        }

        #endregion metodos
    }
}