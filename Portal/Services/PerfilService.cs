using portalAdministrativoSISEC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services
{
    public class PerfilService:IPerfilService
    {
        private readonly HttpClient httpClient;
        public PerfilService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<ResponseBody> GetPerfil(String token)
        {
            ResponseBody emp = new ResponseBody();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://10.130.0.12:6236/Principal/ObtenerPerfilesTodos");
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                var response = await httpClient.SendAsync(request);

                string resultContent = response.Content.ReadAsStringAsync().Result;

                var weatherForecast = JsonSerializer.Deserialize<ResponseBody>(resultContent);
                var weatherForecast2 = JsonSerializer.Deserialize<ResponseResult>(weatherForecast.response);

                //aplica para lista
                string cad = weatherForecast2.result.Replace("\\", "");
                cad = cad.Substring(1, cad.Length - 2);
                var weatherForecast3 = JsonSerializer.Deserialize<List<Perfil>>(cad);
                Console.WriteLine("Perfil: "+weatherForecast3[0].nombre);
            }
            catch (Exception e)
            {
                Console.WriteLine("error api" + e);

            }

            Console.WriteLine(emp);
            return emp;
        }
    }
}
