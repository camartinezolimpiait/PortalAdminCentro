using Blazored.Toast;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Services.Agendamiento.Perfil;
using portalAdministrativoSISEC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Agendamiento.Perfil
{
    public partial class Perfil
    {
        #region Constructor
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public IPerfilAgendaService _perfilAgendaService { get; set; }
        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
     

        #endregion

        #region Variables

        private ApplicationShared applicationShared = new ApplicationShared();
        private LocalStorage localStorage = new LocalStorage();
        Data.DataContact data = new Data.DataContact();
        public string message = "";
        public string urlMap;
        public bool isLoading = true;
        public bool isReadOnly = false;

        #endregion

        #region Metodos

        protected override async Task OnInitializedAsync()
        {
            urlMap = Configuration.GetSection("AppSettings:UrlMapaCentros").Value;
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;

            isReadOnly = applicationShared.Plataforma.Equals("Armas");

            await GetInformationCentro();
        }

        Dictionary<string, object> SetReadonly()
        {
            var dict = new Dictionary<string, object>();
            if (isReadOnly) dict.Add("readonly", "readonly");
            return dict;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task GetInformationCentro()
        {
            GetCentroRequest request = new GetCentroRequest
            {
                ID = applicationShared.IdCentro,
                Plataforma = applicationShared.Plataforma
            };
            GetCentroResponse respuestaCentro = await _perfilAgendaService.GetInformationCentroId(request);

            if (respuestaCentro != null)
            {
                BuildCentro(respuestaCentro);
            }
            else
            {
                isLoading = false;
                toastService.ShowWarning(@"No se encontraron Registros del centro", "Información");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centroResponse"></param>
        private void BuildCentro(GetCentroResponse centroResponse)
        {
			var propiedadesCentro = centroResponse.Respuesta.GetType().GetProperties();
			var propiedadesData = data.GetType().GetProperties();

			try
			{
				foreach (var propiedadCentro in propiedadesCentro)
				{
					foreach (var propiedadData in propiedadesData)
					{
						// Si las propiedades tienen el mismo nombre y el mismo tipo
						if (propiedadData.Name == propiedadCentro.Name && propiedadData.PropertyType == propiedadCentro.PropertyType)
						{
							// Establece el valor de la propiedad destino al valor de la propiedad origen
							propiedadData.SetValue(data, propiedadCentro.GetValue(centroResponse.Respuesta));
							break;
						}
					}
				}

				data.plataforma = applicationShared.Plataforma;
				applicationShared.IdCentro = centroResponse.Respuesta.IdCentro;
				applicationShared.IdRunt = centroResponse.Respuesta.CodigoRUNT.ToString();

				urlMap = urlMap + data.Nombre.Replace(" ", "+") + "&center=" + data.Latitud.ToString().Replace(",", ".") + "," + data.Longitud.ToString().Replace(",", ".");
			}
			catch
            {
                applicationShared.IdCentro = centroResponse.Respuesta.IdCentro;
                applicationShared.IdRunt = centroResponse.Respuesta.CodigoRUNT.ToString();
            }
            isLoading = false;
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task UpdateCentro()
        {
                isLoading = true;
                CentroResponse centroResponse = await _perfilAgendaService.UpdateCentroId(data);
                if (centroResponse != null)
                {
                    message = centroResponse.RespuestaTexto;
                    isLoading = false;
                    toastService.ShowSuccess(message, "Información");
                }
                else
                {
                    isLoading = false;
                    toastService.ShowWarning(@"Ah ocurrido un error al actualizar la información, por favor comuniquese con el admnistrador del sistema.", "Información");

                }
        }
        #endregion
    }
}
