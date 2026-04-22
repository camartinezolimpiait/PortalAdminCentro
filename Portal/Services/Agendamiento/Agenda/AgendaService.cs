using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Util;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.Agenda
{
    public class AgendaService : IAgendaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IApiService _apiService;
        private readonly string stringSisecAdmin = "SisecAdmin";

        public bool EstadoPeticion = true;
        public AgendaService(IHttpClientFactory clientFactory, IConfiguration configuration, IApiService apiService)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            this._apiService = apiService;
        }

        public async Task<List<TipoDocumentoDTO>> GetTipoDocumento()
        {
            try
            {
                // Llamada al API genérico
                ApiResponse<List<TipoDocumentoDTO>> response = await _apiService.CallApiAsync<List<TipoDocumentoDTO>>(
                    baseAddress: _configuration["AppSettings:uriSisecParametization"],
                    apiEndpoint: "TipoDocumento/ObtenerTipoDocumentoTodosAsync",
                    method: HttpMethod.Get,
                    content: null,
                    apiKey: stringSisecAdmin
                );

                // Validar si la respuesta fue exitosa
                if (response != null && response.IsSuccess && response.Content != null)
                {
                    return response.Content;
                }

                return new List<TipoDocumentoDTO>();
            }
            catch (Exception ex)
            {
                // Puedes registrar el error si tienes un logger
                // _logger.LogError(ex, "Error al obtener tipos de documento");
                return new List<TipoDocumentoDTO>();
            }
        }

        public async Task<List<TipoCitaDTO>> GetTipoCita()
        {
            try
            {
                // Llamada al API genérico
                ApiResponse<List<TipoCitaDTO>> response = await _apiService.CallApiAsync<List<TipoCitaDTO>>(
                    baseAddress: _configuration["AppSettings:uriSisecParametization"],
                    apiEndpoint: "TipoCita/ObtenerTipoCitaTodosAsync",
                    method: HttpMethod.Get,
                    content: null,
                    apiKey: stringSisecAdmin
                );

                // Validar si la respuesta fue exitosa
                if (response != null && response.IsSuccess && response.Content != null)
                {
                    return response.Content;
                }

                return new List<TipoCitaDTO>();
            }
            catch (Exception ex)
            {
                return new List<TipoCitaDTO>();
            }
        }


        public async Task<List<CategoriaDTO>> GetCategoriaId(int idCliente)
        {
            try
            {
                // Construir la URL dinámica con el idCliente
                string apiEndpoint = $"Categoria/ObtenerCategoriaxCliente/{idCliente}";

                // Llamada al API genérico
                ApiResponse<List<CategoriaDTO>> response = await _apiService.CallApiAsync<List<CategoriaDTO>>(
                    baseAddress: _configuration["AppSettings:uriSisecParametization"],
                    apiEndpoint: apiEndpoint,
                    method: HttpMethod.Get,
                    content: null,
                    apiKey: stringSisecAdmin
                );

                // Validar si la respuesta fue exitosa
                if (response != null && response.IsSuccess && response.Content != null)
                {
                    return response.Content;
                }

                return new List<CategoriaDTO>();
            }
            catch (Exception ex)
            {
                return new List<CategoriaDTO>();
            }
        }


        public async Task<List<TramiteDTO>> GetTramiteId(int idCliente)
        {
            try
            {
                // Construir el endpoint con el parámetro dinámico
                string apiEndpoint = $"Tramite/ObtenerTramitexCliente/{idCliente}";

                // Llamada al API genérico
                ApiResponse<List<TramiteDTO>> response = await _apiService.CallApiAsync<List<TramiteDTO>>(
                    baseAddress: _configuration["AppSettings:uriSisecParametization"],
                    apiEndpoint: apiEndpoint,
                    method: HttpMethod.Get,
                    content: null,
                    apiKey: stringSisecAdmin
                );

                // Validar si la respuesta fue exitosa
                if (response != null && response.IsSuccess && response.Content != null)
                {
                    return response.Content;
                }

                return new List<TramiteDTO>();
            }
            catch (Exception ex)
            {
                return new List<TramiteDTO>();
            }
        }


        public async Task<bool> GuardarAgenda(NuevaAgendaMLDTO agendaNuevaCitas)
        {
            try
            {
                ApiResponse<ResponseWrapper<object>> response = await _apiService.CallApiAsync<ResponseWrapper<object>>(
                    baseAddress: _configuration["AppSettings:uriSisecParametization"],
                    apiEndpoint: "api/AgendaMiLicencia/CrearAgenda",
                    method: HttpMethod.Post,
                    content: agendaNuevaCitas,
                    apiKey: stringSisecAdmin
                );

                if (response != null && response.IsSuccess)
                {
                    EstadoPeticion = response.Content.Data != null ? true : false;
                    return EstadoPeticion;
                }

                EstadoPeticion = false;
                return EstadoPeticion;
            }
            catch (Exception ex)
            {
                EstadoPeticion = false;
                return EstadoPeticion;
            }
        }


        public async Task<bool> SaveSchedule(AgendaDTO schedule)
        {
            ApiResponse<ResponseWrapper<object>> response = await _apiService.CallApiAsync<ResponseWrapper<object>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: "Agenda/GuardarAgenda",
                method: HttpMethod.Post,
                content: schedule,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                EstadoPeticion = response.Content.Data != null ? true:false;
                return EstadoPeticion;
            }

            EstadoPeticion = false;
            return EstadoPeticion;
        }


        public async Task<bool> BlockSchedule(AgendaDTO schedule)
        {
            ApiResponse<ResponseWrapper<object>> response = await _apiService.CallApiAsync<ResponseWrapper<object>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: "Agenda/BloquearAgenda",
                method: HttpMethod.Post,
                content: schedule,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess  && response.Content.Estado)
            {
                EstadoPeticion = response.Content.Data != null ? true : false;
                return EstadoPeticion;
            }

            EstadoPeticion = false;
            return EstadoPeticion;
        }


        public async Task<bool> BlockScheduleList(List<AgendaDTO> scheduleList)
        {
            ApiResponse<ResponseWrapper<object>> response = await _apiService.CallApiAsync<ResponseWrapper<object>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: "Agenda/BloquearListadoAgenda/",
                method: HttpMethod.Post,
                content: scheduleList,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content.Estado)
            {
                EstadoPeticion = response.Content.Data != null ? true : false;
                return EstadoPeticion;
            }

            EstadoPeticion = false;
            return EstadoPeticion;
        }


        public async Task<List<AgendaCentro>> GetScheduleCentro(int idPerfil, DateTime fromDate, DateTime toDate)
        {
            string apiEndpoint = $"Agenda/ObtenerAgendaPorPerfil/{idPerfil}/{fromDate:ddMMyyyy}/{toDate:ddMMyyyy}";

            ApiResponse<ResponseWrapper<string>> response = await _apiService.CallApiAsync<ResponseWrapper<string>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content != null)
            {
                return JsonConvert.DeserializeObject<List<AgendaCentro>>(response.Content.Data);
            }

            return new List<AgendaCentro>();
        }


        public async Task<List<MotivoDTO>> GetMotivos()
        {
            ApiResponse<List<MotivoDTO>> response = await _apiService.CallApiAsync<List<MotivoDTO>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: "Motivo/ObtenerMotivos",
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content != null)
            {
                return response.Content;
            }

            return new List<MotivoDTO>();
        }


        public async Task<AgendaDTO> GetAppoimentById(long scheduleId)
        {
            string apiEndpoint = $"Agenda/ObtenerAgendaPorId/{scheduleId}";

            ApiResponse<ResponseWrapper<string>> response = await _apiService.CallApiAsync<ResponseWrapper<string>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content != null)
            {
                return JsonConvert.DeserializeObject<AgendaDTO>(response.Content.Data);
            }

            return new AgendaDTO();
        }


        public async Task<List<ResultConsultaAgendaDTO>> GetAppoimentByParametro(int idPerfil, string startDate, string parametro)
        {
            string apiEndpoint = $"Agenda/ObtenerAgendaPorNombreXNumeroDocumento/{idPerfil}/{startDate}/{parametro}";

            ApiResponse<List<ResultConsultaAgendaDTO>> response = await _apiService.CallApiAsync<List<ResultConsultaAgendaDTO>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content != null)
            {
                return response.Content;
            }

            return new List<ResultConsultaAgendaDTO>();
        }


        public async Task<TipoDocumentoDTO> GetDocumentTypeById(int idDocumentType)
        {
            string apiEndpoint = $"TipoDocumento/ObtenerTipoDocumentoId/{idDocumentType}";

            ApiResponse<TipoDocumentoDTO> response = await _apiService.CallApiAsync<TipoDocumentoDTO>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content != null)
            {
                return response.Content;
            }

            return new TipoDocumentoDTO();
        }


        public async Task<TramiteDTO> GetTramitePorId(short idTramite)
        {
            string apiEndpoint = $"Tramite/ObtenerTramiteId/{idTramite}";

            ApiResponse<TramiteDTO> response = await _apiService.CallApiAsync<TramiteDTO>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content != null)
            {
                return response.Content;
            }

            return new TramiteDTO();
        }


        public async Task<MotivoDTO> GetMotivoPorId(short idMotivo)
        {
            string apiEndpoint = $"Motivo/ObtenerMotivoPorId/{idMotivo}";

            ApiResponse<MotivoDTO> response = await _apiService.CallApiAsync<MotivoDTO>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content != null)
            {
                return response.Content;
            }

            return new MotivoDTO();
        }


        public async Task<bool> CancelSchedule(long scheduleId, string usuarioCancelacion)
        {
            string apiEndpoint = $"Agenda/CancelarAgenda/{scheduleId}/{usuarioCancelacion}";

            ApiResponse<ResponseWrapper<object>> response = await _apiService.CallApiAsync <ResponseWrapper<object>> (
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: null,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content.Estado)
            {
                EstadoPeticion = response.Content.Data != null ? true : false;
                return EstadoPeticion;
            }

            EstadoPeticion = false;
            return EstadoPeticion;
        }


        public async Task<bool> ReSchedule(AgendaDTO reschedule)
        {
            var apiEndpoint = "Agenda/ReAgendarCita";

            var response = await _apiService.CallApiAsync<ResponseWrapper<object>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: reschedule,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess && response.Content.Estado)
            {
                EstadoPeticion = response.Content.Data != null ? true : false;
                return EstadoPeticion;
            }

            return false;
        }


        public async Task<AgendaDTO> CheckScheduleAvailableByDatosPersonaId(
            int idPerfil,
            int idIdentificationType,
            string numberIdentification,
            string startSearchDate)
        {
            var apiEndpoint =
                $"Agenda/ConsultarAgendaPorDatosPersona/{idPerfil}/?idTipoIdentificacion={idIdentificationType}" +
                $"&numeroIdentificacion={numberIdentification}&fechaInicioBusqueda={startSearchDate}";

            var response = await _apiService.CallApiAsync<ResponseWrapper<string>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Get,
                apiKey: stringSisecAdmin
            );

            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<AgendaDTO>(response.Content.Data??"");
            }

            return default;
        }

    }
}
