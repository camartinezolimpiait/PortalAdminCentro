using portalAdministrativoSISEC.Application.Contracts.Agendamiento.HorarioAtencion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Entidades.Agendamiento;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion
{
    public class HorarioAtencionService : IHorarioAtencionService
    {
        #region Fields

        public bool EstadoPeticion = true;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IApiService _apiService;
        private readonly string stringSisecAdmin = "SisecAdmin";

        #endregion Fields

        #region Public Constructors

        public HorarioAtencionService(IHttpClientFactory clientFactory, IConfiguration configuration, IApiService apiService)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _apiService = apiService;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<bool> GetEstadoPeticion()
        {
            return EstadoPeticion;
        }

        public async Task<List<ConfiguracionHorario>> GetConfiguracionHorario(int idPerfil, DateTime fromDate, bool getNewSchedule)
        {
            // Inicio refactorización por GitHub Copilot
            var apiEndpoint = $"HorarioAtencion/GetConfiguracionHorario/{idPerfil}/{fromDate:ddMMyyyy}/{getNewSchedule.ToString().ToLower()}";
            // Fin refactorización por GitHub Copilot

            var response = await _apiService.CallApiAsync<ResponseWrapper<string>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = response.Content != null;
                return JsonConvert.DeserializeObject<List<ConfiguracionHorario>>(response.Content.Data) ?? new List<ConfiguracionHorario>();
            }

            EstadoPeticion = false;
            return new List<ConfiguracionHorario>();
        }


        public async Task<List<ConfiguracionHorario>> GetConfiguracionHorario(int idPerfil, string dateFrom)
        {
            var apiEndpoint = $"HorarioAtencion/GetConfiguracionHorario/{idPerfil}/{dateFrom}/false";

            var response = await _apiService.CallApiAsync<List<ConfiguracionHorario>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = response.Content != null;
                return response.Content;
            }

            EstadoPeticion = false;
            return default;
        }


        public async Task<ResultSaveParametrizationSchedule> SaveConfiguracionHorario(
            int idPerfil, string usuario, int tipoActualizacion, List<ConfiguracionHorario> configuracionHorarios)
        {
            var apiEndpoint = $"HorarioAtencion/SaveAndEditHorarioAtencion?idPerfil={idPerfil}&usuario={usuario}";

            var response = await _apiService.CallApiAsync<ResultSaveParametrizationSchedule>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: configuracionHorarios,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = response.Content != null;
                return response.Content;
            }

            EstadoPeticion = false;
            return default;
        }


        public async Task<bool> SaveConfiguracionAgenda(
            int idPerfil, string usuario, int tipoActualizacion, int intervaloCitas, int agendaDisponible,
            List<ConfiguracionHorario> configuracionHorarios)
        {
            var apiEndpoint = $"HorarioAtencion/SaveAndEditHorarioAgenda?idPerfil={idPerfil}&usuario={usuario}&intervaloCitas={intervaloCitas}&agendaDisponible={agendaDisponible}";

            var response = await _apiService.CallApiAsync<bool>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: configuracionHorarios,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = response.Content;
                return EstadoPeticion;
            }

            EstadoPeticion = false;
            return default;
        }


        #endregion Public Methods
    }
}
