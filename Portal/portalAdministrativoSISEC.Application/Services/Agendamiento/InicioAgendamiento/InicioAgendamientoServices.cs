using portalAdministrativoSISEC.Application.Contracts.Agendamiento.InicioAgendamiento;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Entidades.Agendamiento.InicioAgendamiento;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.InicioAgendamiento
{
    public class InicioAgendamientoServices :IInicioAgendamiento
    {
		private readonly IHttpClientFactory _clientFactory;
		private readonly IConfiguration _configuration;
        private readonly IApiService _apiService;
        public bool EstadoPeticion = true;
		public InicioAgendamientoServices(IHttpClientFactory clientFactory, IConfiguration configuration, IApiService apiService)
		{
			_clientFactory = clientFactory;
			_configuration = configuration;
			_apiService = apiService;
        }

        public async Task<List<ConfiguracionAgendamiento>> GetConfiguracionHorario(int idPerfil)
        {
            var apiEndpoint = $"HorarioAgenda/GetConfiguracionAgendamiento?idPerfil={idPerfil}";

            var response = await _apiService.CallApiAsync<List<ConfiguracionAgendamiento>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: "SisecAdmin"
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = true;
                return response.Content;
            }

            EstadoPeticion = false;
            return default;
        }

    }
}

