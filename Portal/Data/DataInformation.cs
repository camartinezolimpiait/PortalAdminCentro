using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
   
    public class DataInformation
    {
        public IApiService _apiService { get; set; }
        private string apiName = null;
        private readonly string uriSisecAuth = string.Empty;
        private readonly string guidAplicacion = string.Empty;
        private readonly IConfiguration _configuration;
        public IEnumerable<MenuInfo> menuList { get; set; }
        public string UserID { get; set; }
        public string FirstNameUser { get; set; }
        private List<CitasIntervalo> citasIntervalos;
		public List<ConvenioCentro> listadoConvenios = new List<ConvenioCentro>();
		public ConsultaCentoId IdCentro;

		public List<CitasIntervalo> _CitasIntervalos
        {
            get
            {
                return citasIntervalos;
            }
            set
            {
                citasIntervalos = value;
                NotifyStateChanged();
            }
        }

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public DataInformation(IConfiguration configuration, IApiService apiService)
        {
            _configuration = configuration;
            uriSisecAuth = _configuration.GetSection("AppSettings:uriSisecAuth").Value;
            guidAplicacion = _configuration.GetSection("AppSettings:GuidAplicacion").Value;
            _apiService = apiService;
        }

        public async Task<IEnumerable<MenuInfo>> GetMenuData(string userID)
        {
            IEnumerable<MenuInfo> menuInfos = null;
            try
            {
                if (menuList != null)
                    return menuList;

                var data = new
                {
                    idUser = userID,
                    GuidAplication = guidAplicacion
                };

                // Consumiendo API con la key SisecAut
                ApiResponse<List<MenuInfo>> response = await _apiService.CallApiAsync<List<MenuInfo>>(
                    baseAddress: uriSisecAuth,
                    apiEndpoint: "AspNetUser/ConsultarMenuUsuario",
                    method: HttpMethod.Post,
                    content: data,
                    apiKey: "SisecAut"
                );

                if (response.IsSuccess)
                {
                    menuInfos = response.Content;
                }
                else
                {
                    // Si quieres, aquí puedes lanzar excepción si hubo error
                    throw new Exception($"Error consultando menú: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return menuInfos;
        }


        /// <summary>
        /// Method to obtain the userName with all information
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Return all the userName information</returns>
        public async Task<string> GetUserName(string userID)
        {
            if (!string.IsNullOrEmpty(FirstNameUser))
                return FirstNameUser;

            ApiResponse<UserName> response = await _apiService.CallApiAsync<UserName>(
                baseAddress: uriSisecAuth,
                apiEndpoint: $"AspNetUser/ObtenerAspNetUserId/{userID}",
                method: HttpMethod.Get,
                content: null,
                apiKey: "SisecAut"
            );

            if (response.IsSuccess)
            {
                FirstNameUser = response.Content.username;
                return FirstNameUser;
            }
            return null;
        }

    }
}
