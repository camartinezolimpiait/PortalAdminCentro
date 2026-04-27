using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Horario;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento
{
    public class ParametrizacionHorarioService : IParametrizacionHorarioService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IApiService _apiService;
        public bool EstadoPeticion = true;
        private readonly string stringSisecAdmin= "SisecAdmin";

        public ParametrizacionHorarioService(IHttpClientFactory clientFactory, IConfiguration configuration, IApiService apiService)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _apiService = apiService;
        }
        public async Task<bool> GetEstadoPeticion()
        {
            return EstadoPeticion;
        }
        public async Task<ParametrizacionHorario> GetParametrizationScheduleByProfileId(int idPerfil, int idParametroHorario)
        {
            var apiEndpoint = $"ParametrizacionHorario/ObtenerParametrizacionHorarioPorIdPerfil/{idPerfil}/{idParametroHorario}";

            ApiResponse<ResponseWrapper<string>> response = await _apiService.CallApiAsync<ResponseWrapper<string>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                bool result = false;
                return JsonConvert.DeserializeObject<ParametrizacionHorario>(response.Content.Data);
            }

            return default;
        }

        public async Task<ParametrizacionHorario> GetParametrizationScheduleByProfileIdAndParametroHorario(int idPerfil, int idParametroHorario)
        {
            var apiEndpoint = $"ParametrizacionHorario/ObtenerParametrizacionHorarioPorIdPerfilParametroHorario/{idPerfil}/{idParametroHorario}";

            ApiResponse<ResponseWrapper<string>> response = await _apiService.CallApiAsync<ResponseWrapper<string>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<ParametrizacionHorario>(response.Content.Data);
            }

            return default;
        }


        public async Task<ResultSaveParametrizationSchedule> SaveParametrizationSchedule(ParametrizacionHorario parametrizacionHorario, DateTime fromDate)
        {
            var apiEndpoint = $"ParametrizacionHorario/GuardarParametrizacionHorario/{fromDate:ddMMyyyy}";

            ApiResponse<ResultSaveParametrizationSchedule> response = await _apiService.CallApiAsync<ResultSaveParametrizationSchedule>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: parametrizacionHorario,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = true; 
                return response.Content;
            }

            return default;
        }

    }
}


