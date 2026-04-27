using portalAdministrativoSISEC.Application.Contracts.MiLicencia.PortalAdministrativo;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Aplication;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo
{
    public class PortalAdministrativoService : IPortalAdministrativoService
    {
        #region Variables

        protected HttpClient Cliente { get; set; }
        protected readonly IOptions<AppSettings> _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private string UrlServicio => _appSettings.Value.ApiPortalAdministrativo.Url;

        #endregion Variables

        #region Constructor

        // Inicio refactorización/optimización por GitHub Copilot
        public PortalAdministrativoService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings;
            _httpClientFactory = httpClientFactory;

            // Usar el cliente HTTP nombrado "PortalAdministrativo" configurado en StartupExtensions.AgregarHttpClient
            Cliente = _httpClientFactory.CreateClient("PortalAdministrativo");

            // Asegurar BaseAddress por si la configuración cambia en el futuro
            if (Cliente.BaseAddress == null)
            {
                Cliente.BaseAddress = new Uri(UrlServicio);
            }
        }
        // Fin refactorización/optimización por GitHub Copilot

        #endregion Constructor

        #region API Consumption
        /// <summary>
        /// Consumes a GET service from the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="createToken"></param>
        /// <returns></returns>
        public async Task<T> ConsumirServicioGET<T>(string url, bool createToken = true)
        {
            string token = "";
            if (createToken)
            {
                TokenModel currentToken = await GetToken();
                token = currentToken.TokenBearer;
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await Cliente.SendAsync(request);

            var respuestaMetodo = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(respuestaMetodo);
        }
        /// <summary>
        /// Consumes a POST service from the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="metodo"></param>
        /// <param name="json"></param>
        /// <param name="createToken"></param>
        /// <returns></returns>
		public async Task<T> ConsumirServicioPOST<T>(string metodo, object json, bool createToken = true)
        {
            string token = "";

            if (createToken)
            {
                TokenModel currentToken = await GetToken();
                token = currentToken.TokenBearer;
            }

            string jsonString = JsonConvert.SerializeObject(json);

            using var request = new HttpRequestMessage(HttpMethod.Post, metodo);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await Cliente.SendAsync(request);
            var respuestaMetodo = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(respuestaMetodo);
        }

        #endregion API Consumption

        #region Utils
        /// <summary>
        /// Registers a log for the cotizador service.
        /// </summary>
        /// <param name="detalleLog"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> RegistrarLogCotizador(DetalleLog detalleLog)
        {
            bool isValid = true;
            if (detalleLog == null)
            {
                throw new ArgumentNullException(nameof(detalleLog), "El detalle del log no puede ser nulo.");
            }
            try
            {
                Log log = new()
                {
                    Origen = "Portal Administrativo",
                    NombreMaquina = Environment.MachineName,
                    Metodo = detalleLog.Metodo,
                    Request = detalleLog.Request,
                    Response = detalleLog.Response,
                    Mensaje = detalleLog.Mensaje,
                    MensajeCompleto = detalleLog.MensajeCompleto
                };
                return await ConsumirServicioPOST<bool>(MetodosApiPortalAdmin.REGISTRAR_LOG_COTIZADOR, log);
            }
            catch
            {
                isValid = false; // Aseguramos que siempre se retorne false
            }
            return isValid; // Retorna true si se registró correctamente, false en caso contrario
        }
        /// <summary>
        /// Registers an exception log in the system.
        /// </summary>
        /// <param name="exceptionLog"></param>
        /// <returns></returns>
		public async Task RegisterExceptionLog(ExceptionLog exceptionLog)
        {
            try
            {
                await ConsumirServicioPOST<BasicResponse>(MetodosApiPortalAdmin.REGISTER_EXCEPTION_LOG, exceptionLog);
            }
            catch (Exception ex)
            {

                var errorLog = new ExceptionLog
                {
                    MessageLog = ex.Message,
                    ExceptionType = ex.GetType().ToString(),
                    TimestampLog = DateTime.Now,
                    StackTraceLog = ex.ToString(),
                    TransactionIdLog = exceptionLog.TransactionIdLog, // Mantener la misma transacción si existe
                    SorceLog = "RegisterExceptionLog",
                    LayerSourceLog = "LogService",
                    ApplicationSourceLog = "Portal Administrativo", // Ajusta según tu aplicación
                    MachineNameLog = Environment.MachineName,
                    IPAddressLog = GetLocalIPAddress(), // Necesitarás implementar este método
                    UserLog = exceptionLog.UserLog ?? "system", // Mantener el mismo usuario o usar "system"
                    CorrelationKey = exceptionLog.CorrelationKey // Mantener la misma correlación
                };

                await RegistrarLogCotizador(new DetalleLog()
                {
                    Metodo = "RegisterExceptionLog",
                    Request = JsonConvert.SerializeObject(exceptionLog),
                    Response = ex.ToString(),
                    Mensaje = ex.Message,
                    MensajeCompleto = ex.InnerException?.Message
                });


            }
        }
        /// <summary>
        /// Registers an info log in the system.
        /// </summary>
        /// <param name="infoLog"></param>
        /// <returns></returns>
		public async Task RegisterInfoLog(InfoLog infoLog)
        {
            try
            {
                await ConsumirServicioPOST<BasicResponse>(MetodosApiPortalAdmin.REGISTER_INFO_LOG, infoLog);
            }
            catch (Exception ex)
            {

                var errorLog = new ExceptionLog
                {
                    MessageLog = $"Error al registrar InfoLog: {ex.Message}",
                    ExceptionType = ex.GetType().ToString(),
                    TimestampLog = DateTime.Now,
                    StackTraceLog = ex.ToString(),
                    SorceLog = "LogService",
                    LayerSourceLog = "InfoLogHandler",
                    ApplicationSourceLog = "MiLicencia", // Ajusta según tu aplicación
                    MachineNameLog = Environment.MachineName,
                    IPAddressLog = GetLocalIPAddress(), // Necesitarás implementar este método
                    UserLog = "system",
                    CorrelationKey = infoLog.CorrelationKey, // Mantener la misma correlación
                    TransactionIdLog = Guid.NewGuid().ToString() // Generar un nuevo ID de transacción
                };
                await RegisterExceptionLog(errorLog);
            }

        }
        /// <summary>
        /// Obtains a token for the API.
        /// </summary>
        /// <returns></returns>
        private async Task<TokenModel> GetToken()
        {
            object json = new
            {
                _appSettings.Value.ApiPortalAdministrativo.UserName,
                _appSettings.Value.ApiPortalAdministrativo.UserPassword
            };

            try
            {
                var httpClient = _httpClientFactory.CreateClient("PortalAdministrativo");

                string jsonString = JsonConvert.SerializeObject(json);
                var contenidoRequest = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Realizar la solicitud directamente sin usar ConsumirServicioPOST
                var response = await httpClient.PostAsync("Token/Login2", contenidoRequest);

                response.EnsureSuccessStatusCode();

                var respuestaMetodo = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenModel>(respuestaMetodo) ?? new TokenModel();
            }
            catch
            {
                // Retornar un token vacío en caso de error
                return new TokenModel();
            }
        }

        /// <summary>
        /// Obtains the local IP address of the machine.
        /// </summary>
        /// <returns></returns>
        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
        #endregion Private Methods
    }
}

