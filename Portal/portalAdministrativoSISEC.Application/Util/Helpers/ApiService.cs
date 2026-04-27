using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using portalAdministrativoSISEC.Application.Data.Auth;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia.PortalAdministrativo;
using portalAdministrativoSISEC.Util.LogAuditoria;
using System.Diagnostics;

namespace portalAdministrativoSISEC.Util.Helpers
{
    public class ApiService : IApiService
    {
        private readonly ILogger<ApiService> _logger;
        private readonly IAesEncryptionHelper _aesEncryptionHelper;
        private readonly IConfiguration _configuration;
        private readonly IPortalAdministrativoService _portalAdminService;

        public ApiService(IAesEncryptionHelper aesEncryptionHelper, IConfiguration configuration, ILogger<ApiService> logger,
            IPortalAdministrativoService portalAdminService)
        {
            _aesEncryptionHelper = aesEncryptionHelper;
            _configuration = configuration;
            _logger = logger;
            _portalAdminService = portalAdminService;
        }

        public async Task<ApiResponse<T>> CallApiAsync<T>(string baseAddress, string apiEndpoint, HttpMethod method, object content = null, string keyName = "SisecAdmin")
        {
            try
            {
                string token = await GetBearerTokenAsync(baseAddress, keyName);
                if (string.IsNullOrEmpty(token))
                {
                    //throw new InvalidOperationException($"Error al procesar respuesta de autenticación");
                }

                using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpRequestMessage request = new HttpRequestMessage(method, apiEndpoint);

                if (content != null)
                {
                    string json = JsonConvert.SerializeObject(content);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await httpClient.SendAsync(request);

                string responseContent = await response.Content.ReadAsStringAsync();

                // Crear respuesta con toda la información disponible
                var apiResponse = new ApiResponse<T>
                {
                    HttpResponse = response,
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    ErrorMessage = response.IsSuccessStatusCode ? null : responseContent
                };

                // Solo intentar deserializar si la respuesta fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        apiResponse.Content = JsonConvert.DeserializeObject<T>(responseContent);
                    }
                    catch (JsonException ex)
                    {
                        await RegistrarExcepcionAsync(ex);
                        apiResponse.IsSuccess = false;
                        apiResponse.ErrorMessage = $"Error al deserializar: {ex.Message}";
                    }
                }

                return apiResponse;
            }
            catch (HttpRequestException ex)
            {
                await RegistrarExcepcionAsync(ex);
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error de conexión: {ex.Message}",
                    StatusCode = ex.StatusCode.HasValue ? (int)ex.StatusCode.Value : 0
                };
            }
            catch (Exception ex)
            {
                await RegistrarExcepcionAsync(ex);
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}",
                    StatusCode = 0
                };
            }
        }

        private async Task<string> GetBearerTokenAsync(string authBaseUrl, string keyName)
        {
            try
            {
                string encryptedApiKey = _configuration[$"{keyName}:{keyName}Id"];
                string decryptedJson = await _aesEncryptionHelper.Decrypt(encryptedApiKey);
                var credentials = JsonConvert.DeserializeObject<SisecCredentials>(decryptedJson);

                using var httpClient = new HttpClient { BaseAddress = new Uri(authBaseUrl) };

                var loginRequest = new
                {
                    accessId = credentials.AccessId,
                    accessKey = credentials.AccessKey
                };

                var requestContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Auth/Login", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    await RegistrarExcepcionAsync(new Exception($"Error en autenticación: {response.StatusCode}"));
                    return string.Empty;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                return loginResponse?.TokenBearer;
            }
            catch (Exception ex)
            {
                await RegistrarExcepcionAsync(ex);
                return string.Empty;
            }
        }

        private async Task RegistrarExcepcionAsync(Exception ex)
        {
            try
            {
                StackTrace stackTrace = new StackTrace();
                StackFrame callingFrame = stackTrace.GetFrame(1);
                System.Reflection.MethodBase callingMethod = callingFrame?.GetMethod();

                string methodName = callingMethod?.Name ?? "Default";
                string componentName = callingMethod?.DeclaringType?.Name ?? "Default";

                var log = ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null);
                await _portalAdminService.RegisterExceptionLog(log);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error en el metodo de: RegistrarExcepcionAsync");
            }
        }
    }
}


