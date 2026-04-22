using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.Perfil
{
    public class PerfilAgendaService(IHttpClientFactory ClientFactory, IApiService _apiService, IConfiguration _configuration) : IPerfilAgendaService
    {
        #region Public Methods

        public async Task<CentroResponse> UpdateCentroId(DataContact centro)
        {
            var apiEndpoint = "Centro/ActualizarDatosCentroPortal";

            ApiResponse<CentroResponse> response = await _apiService.CallApiAsync<CentroResponse>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: centro,
                apiKey: "SisecAdmin"
            );

            if (response != null && response.IsSuccess)
            {
                return response.Content;
            }

            return default;
        }


        public async Task<GetCentroResponse> GetInformationCentroId(GetCentroRequest consultaCentro)
        {
            var apiEndpoint = "Centro/ConsultarCentroxId";


            ApiResponse<GetCentroResponse> response = await _apiService.CallApiAsync<GetCentroResponse>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: consultaCentro,
                apiKey: "SisecAdmin"
            );

            if (response != null && response.IsSuccess)
            {
                await ActualizarIdRuntPerfil(response.Content.Respuesta.IdCentro, response.Content.Respuesta.CodigoRUNT ?? 0);
                return response.Content;
            }

            return default;
        }


        public async Task<bool> ActualizarIdRuntPerfil(long idCentro, long idRunt)
        {
            var apiEndpoint = "AgendaPerfil/ActualizarIdRuntPerfil";

            var request = new
            {
                idCentro,
                idRunt
            };

            ApiResponse<List<PerfilCentroClienteRequestDTO>> response = await _apiService.CallApiAsync<List<PerfilCentroClienteRequestDTO>>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: request,
                apiKey: "SisecAdmin"
            );

            if (response != null && response.IsSuccess)
            {
                return true;
            }

            return false;
        }


        public async Task<GetComercioResponse> GetInformationComercio(GetCentroRequest consultaCentro)
        {
            var apiEndpoint = "Comercio/ObtenerComercioIDAsync";

            ApiResponse<GetComercioResponse> response = await _apiService.CallApiAsync<GetComercioResponse>(
                baseAddress: _configuration["AppSettings:uriSisecParametization"],
                apiEndpoint: apiEndpoint,
                method: HttpMethod.Post,
                content: consultaCentro,
                apiKey: "SisecAdmin"
            );

            if (response != null && response.IsSuccess)
            {
                return response.Content;
            }

            return default;
        }


        #endregion Public Methods
    }
}