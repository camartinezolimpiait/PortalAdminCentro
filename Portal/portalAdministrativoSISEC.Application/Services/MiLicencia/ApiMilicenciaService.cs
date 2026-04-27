using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Aplication;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using portalAdministrativoSISEC.Util.Const;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.MiLicencia
{
    public class ApiMilicenciaService : IApiMilicenciaService
    {
		#region Variables
		private string _filePath;

		protected HttpClient Cliente { get; set; }
        protected readonly IOptions<AppSettings> _appSettings;

		private readonly IAesEncryptionHelper _aesEncryptionHelper;

		private string UrlServicio => _appSettings.Value.ApiFrontMiLicencia.Url;

        #endregion Variables

        #region Constructor

        public ApiMilicenciaService(IOptions<AppSettings> appSettings, HttpClient cliente, IAesEncryptionHelper aesEncryptionHelper)
        {

            _appSettings = appSettings;
            cliente.BaseAddress = new Uri(UrlServicio);
            Cliente = cliente;
            Cliente.Timeout = TimeSpan.FromSeconds(120);
			_aesEncryptionHelper = aesEncryptionHelper;
			_filePath = @"C:\SISEC\log.txt";

		}

		#endregion Constructor

        public async Task<T> ConsumirServicioGET<T>(string url, bool createToken = true,string googleCaptcha = "")
        {
			//LogMessage($"Ejecución en ConsumirServicioGET  {url}");

			string token = "";
            if (createToken)
            {
                TokenModel currentToken = await GetToken();
                token = currentToken.TokenBearer;
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(token))
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!string.IsNullOrEmpty(googleCaptcha))
            {
                request.Headers.Add("Captcha", googleCaptcha);
            }

            HttpResponseMessage response = await Cliente.SendAsync(request);

            var respuestaMetodo = await response.Content.ReadAsStringAsync();

			//LogMessage($"Reseultado en ConsumirServicioGET  {url} ({respuestaMetodo})");


			return JsonConvert.DeserializeObject<T>(respuestaMetodo);
        }

        public async Task<T> ConsumirServicioPOST<T>(string metodo, object json, bool createToken = true,string googleCaptcha = "")
        {
			//LogMessage($"Ejecución en ConsumirServicioPOST  {metodo} ({JsonConvert.SerializeObject(json)})");

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

            if (!string.IsNullOrEmpty(googleCaptcha))
            {
                request.Headers.Add("Captcha", googleCaptcha);
            }

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await Cliente.SendAsync(request);
            var respuestaMetodo = await response.Content.ReadAsStringAsync();
			
            //LogMessage($"Reseultado en ConsumirServicioPOST  {metodo} ({respuestaMetodo})");

			    return JsonConvert.DeserializeObject<T>(respuestaMetodo);
        }

        public async Task<TokenModel> GetToken()
        {
            try
            {
                string json = JsonConvert.SerializeObject(
                    new
                    {
                        _appSettings.Value.ApiFrontMiLicencia.UserName,
                        _appSettings.Value.ApiFrontMiLicencia.UserPassword,
                        key = DateTime.Now.ToString(),
                    });

                string key = await _aesEncryptionHelper.Encrypt(json);

                TokenModel tokenModel = await ConsumirServicioPOST<TokenModel>(MetodosApiFrontMiLicencia.GET_TOKEN_ML, new { key }, false,"");

                return tokenModel ?? new TokenModel();
            }
            catch (Exception)
            {
                return new TokenModel();
            }
        }

		public void LogMessage(string message)
		{
			using (StreamWriter writer = new StreamWriter(_filePath, true))
			{
				writer.WriteLine($"{DateTime.Now}: {message}");
			}
		}

	}
}


