using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Entidades.Agendamiento.ConfiguracionCuposReglas;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.ConfigruracionCuposReglas
{
    public class ConfiguracionCuposReglas: IConfiguracionCuposReglas
    {
		private readonly IHttpClientFactory _clientFactory;
		private readonly IConfiguration _configuration;
        private readonly IApiService _apiService;
        public bool EstadoPeticion = true;

		public ConfiguracionCuposReglas(IHttpClientFactory clientFactory, IConfiguration configuration, IApiService apiService)
		{
			_clientFactory = clientFactory;
			_configuration = configuration;
            _apiService = apiService;
        }
        public async Task<bool> SaveConfiguracionCuposReglas(ConfiguracionCupoRegla configuracionCupoRegla)
        {
            var apiEndpoint = "ConfiguracionCuposReglas/SaveConfiguracionCuposReglas";

            var response = await _apiService.CallApiAsync<bool>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: configuracionCupoRegla,
                apiKey: "SisecAdmin"
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = response.Content;
                return EstadoPeticion;
            }

            EstadoPeticion = false;
            return default;
        }

    }
}
