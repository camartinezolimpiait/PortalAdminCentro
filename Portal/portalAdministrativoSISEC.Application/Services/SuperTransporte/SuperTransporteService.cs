using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using Azure.Storage;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using portalAdministrativoSISEC.Entidades;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.SuperTransporte
{
    public class SuperTransporteService : ISuperTransporteService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        public string _plataforma = "";
        public string storageAccountName = string.Empty;
        public string storageAccountKey = string.Empty;
        public string containerName = string.Empty;
        string token = string.Empty;
        string tokenSuperVigilados = string.Empty;

        public SuperTransporteService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            storageAccountName = _configuration.GetSection("AppSettings:StogreAccountName").Value;
            storageAccountKey = _configuration.GetSection("AppSettings:StorageAccountKey").Value;
            containerName = _configuration.GetSection("AppSettings:ContainerName").Value;
            token = _configuration.GetSection("AppSettings:tokenStrapi").Value;
            tokenSuperVigilados = _configuration.GetSection("AppSettings:tokenSuperVigilados").Value;
        }
        public void SetPlataforma(string plataforma) 
        { 
            _plataforma = plataforma;
        }

        [HttpGet("GetVigilado/{nit}")]
        public async Task<VigiladoFilterDto> GetVigilado(string nit)
        {
            var resultado = new VigiladoFilterDto();
            try
            {
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.GetAsync($"vigilados?filters[NIT][$eq]={nit}");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        resultado = JsonConvert.DeserializeObject<VigiladoFilterDto>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return resultado;
        }
        [HttpGet("GetVigilado/{nit}")]
        public async Task<RespuestaServiciosGet> GetVigiladoByIdCentro(int idCentro)
        {
            var resultado = new RespuestaServiciosGet();
            try
            {
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.GetAsync($"{_plataforma}/{idCentro}?populate[0]=vigilado");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        resultado = JsonConvert.DeserializeObject<RespuestaServiciosGet>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return resultado;
        }

        [HttpGet("GetCentro/{idCentro}")]
        public async Task<RespuestaServiciosGet> GetCentro(int idCentro, string populate)
        {
            try
            {
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.GetAsync($"{_plataforma}/{idCentro}?populate={populate}");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<RespuestaServiciosGet>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return null;
        }

        [HttpGet("GetCentro/{idCentro}")]
        public async Task<RespuestaServiciosGet> GetCentroMultiPopulate(int idCentro, List<string> populates)
        {
            try
            {
                string urlPopulate = "";
                int contador = 0;
                foreach (var populate in populates) {
                    if (contador != 0)
                        urlPopulate += "&";

                    urlPopulate += $"populate[{contador}]={populate}";
                    contador++;
                }
                string url = $"{_plataforma}/{idCentro}?{urlPopulate}";
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.GetAsync(url);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<RespuestaServiciosGet>(content);
                    }
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
            return null;
        }

        [HttpGet("GetCentroCEA/{idCentro}")]
        public async Task<RespuestaServiciosGetCEA> GetCentroMultiPopulateCEA(int idCentro, List<string> populates)
        {
            try
            {
                string urlPopulate = "";
                int contador = 0;
                foreach (var populate in populates)
                {
                    if (contador != 0)
                        urlPopulate += "&";

                    urlPopulate += $"populate[{contador}]={populate}";
                    contador++;
                }
                string url = $"{_plataforma}/{idCentro}?{urlPopulate}";
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.GetAsync(url);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<RespuestaServiciosGetCEA>(content);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        [HttpGet("GetCentroCEA/{idCentro}")]
        public async Task<RespuestaServiciosGetCEA> GetCentroCEA(int idCentro, string populate)
        {
            try
            {
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.GetAsync($"{_plataforma}/{idCentro}?populate={populate}");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<RespuestaServiciosGetCEA>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return null;
        }

        [HttpPut("PutCentro/{idCentro}")]
        public async Task<RespuestaPutCentro> PutCentro<T>(T objeto, int idCentro)
        {
            var resultado = new RespuestaPutCentro();
            try
            {
                var json = JsonConvert.SerializeObject(new { data = objeto });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"{_plataforma}/{idCentro}";

                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.PutAsync(url, data);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        resultado = JsonConvert.DeserializeObject<RespuestaPutCentro>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return resultado;
        }

        [HttpPut("PutVigilado/{idVigilado}")]
        public async Task<RespuestaServiciosGetVigilado> PutVigilado<T>(T objeto, int idVigilado)
        {
            var resultado = new RespuestaServiciosGetVigilado();
            try
            {
                var json = JsonConvert.SerializeObject(new { data = objeto });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"vigilados/{idVigilado}";

                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.PutAsync(url, data);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        resultado = JsonConvert.DeserializeObject<RespuestaServiciosGetVigilado>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return resultado;
        }

        [HttpPost("UploadFile")]
        public async Task<int> PostFile(IBrowserFile file)
        {
            int idFile = 0;
            try
            {
                var url = $"upload";
                var archivoSeleccionado = file;
                byte[] fileBytes = null;
                ByteArrayContent archivo = null;
                if (archivoSeleccionado != null && archivoSeleccionado.Size > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await archivoSeleccionado.OpenReadStream(20485760).CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                    if (fileBytes != null)
                    {
                        archivo = new ByteArrayContent(fileBytes);
                        archivo.Headers.ContentType = new MediaTypeHeaderValue(archivoSeleccionado.ContentType);
                        MultipartFormDataContent formContent = new MultipartFormDataContent()
                        {
                            { archivo, "files", archivoSeleccionado.Name }
                        };

                        using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                        {
                            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                            var respuesta = await cliente.PostAsync(url, formContent);
                            if (respuesta.IsSuccessStatusCode)
                            {
                                var content = await respuesta.Content.ReadAsStringAsync();
                                List<FileResponseDto> response = JsonConvert.DeserializeObject<List<FileResponseDto>>(content);
                                idFile = response.FirstOrDefault().id;
                            }
                        }
                    }
                }
                return idFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete("DeleteFile")]
        public async Task<bool> DeleteFile(int idFile)
        {
            try
            {
                if (idFile <= 0)
                {
                    return false;
                }
                var url = $"upload/files/{idFile}";
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.DeleteAsync(url);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        FileResponseDto response = JsonConvert.DeserializeObject<FileResponseDto>(content);
                        return true;
                    }
                    else if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet("GetCentroByRunt/{runt}")]
        public async Task<RespuestaServiciosGetByRunt> GetCentroByRunt(long runt)
        {
            try
            {
                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.GetAsync($"{_plataforma}?filters[IDRUNT][$eq]={runt}&populate=vigilado");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<RespuestaServiciosGetByRunt>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return null;
        }

        [HttpPost("PostCentro")]
        public async Task<RespuestaServiciosGet> PostCentro<T>(T objeto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { data = objeto });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"{_plataforma}";

                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.PostAsync(url, data);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<RespuestaServiciosGet>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return null;
        }

        [HttpGet("GetListaMaestra")]
        public async Task<List<SelectSuperTransporteDTO>> GetListaMaestra(string metodo, List<SelectSuperTransporteIndex> campos) 
        {
            try
            {
                var resultJson = new List<SelectSuperTransporteDTO>();
                switch (metodo) {
                    case "InfoProfesionalesCertificadores":
                        resultJson = ConvertJsonDto.leerJson("InfoProfesionalesCertificadores.json");
                        break;
                    case "InfoProfesionalesSalud":
                        resultJson = ConvertJsonDto.leerJson("InfoProfesionalesSalud.json");
                        break;
                    case "EstadoMatriculaMercantil":
                        resultJson = ConvertJsonDto.leerJson("EstadoMatriculaMercantil.json");
                        break;
                    case "Aseguradora":
                        resultJson = ConvertJsonDto.leerJson("Aseguradora.json");
                        break;
                    case "SecretariasSalud":
                        resultJson = ConvertJsonDto.leerJson("SecretariasSalud.json");
                        break;
                    case "Departamentos":
                        resultJson = ConvertJsonDto.leerJson("Departamentos.json");
                        break;
                    case "Ciudades":
                        resultJson = ConvertJsonDto.leerJson("Ciudades.json");
                        break;
                    case "PlantaFisica":
                        resultJson = ConvertJsonDto.leerJson("PlantaFisica.json");
                        break;
                    case "NivelesCEA":
                        resultJson = ConvertJsonDto.leerJson("NivelesCEA.json");
                        break;
                    case "TipoIdentificacion":
                        resultJson = ConvertJsonDto.leerJson("TipoIdentificacion.json");
                        break;
                    case "NitCEA":
                        resultJson = ConvertJsonDto.leerJson("NitCEA.json");
                        break;
                    case "Empresa":
                        resultJson = ConvertJsonDto.leerJson("Empresa.json");
                        break;
                    case "Esquema":
                        resultJson = ConvertJsonDto.leerJson("Esquema.json");
                        break;
                    case "Norma":
                        resultJson = ConvertJsonDto.leerJson("Norma.json");
                        break;
                    case "ClaseVehiculo":
                        resultJson = ConvertJsonDto.leerJson("ClaseVehiculo.json");
                        break;
                    case "Territoriales":
                        resultJson = ConvertJsonDto.leerJson("Territoriales.json");
                        break;
                        
                }
                if (resultJson == null) {
                    return new List<SelectSuperTransporteDTO>();
                }
                return SelectSuperTransporteDTO.convertirSelectSuperTransporteDTO(resultJson.ToList<object>(), campos);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        [HttpPost("PostVigilado")]
        public async Task<int> PostVigilado<T>(T objeto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { data = objeto });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"vigilados";
                int idVigilado = 0;

                using (var cliente = _clientFactory.CreateClient("ApiStrapi"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var respuesta = await cliente.PostAsync(url, data);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        var response =  JsonConvert.DeserializeObject<VigiladoPostDto>(content);
                        idVigilado = response.data.id;
                        return idVigilado;
                    }
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
            return 0;
        }

        [HttpGet("GetListaMaestraSuperT")]
        public async Task<List<SelectSuperTransporteDTO>> GetListaMaestraSuperT(string lista)
        {
            try
            {
                using (var cliente = _clientFactory.CreateClient("ApiSuperT"))
                {
                    var respuesta = await cliente.GetAsync($"{lista}");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<SelectSuperTransporteDTO>>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return null;
        }

        [HttpGet("GetListaMaestraSuperVigilados")]
        public async Task<VigiladoDto> GetListaMaestraSuperVigilados(string nit)
        { 
            try
            {
                using (var cliente = _clientFactory.CreateClient("ApiSuperVigilados"))
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenSuperVigilados);
                    var respuesta = await cliente.GetAsync($"{nit}");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var content = await respuesta.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<VigiladoDto>(content);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            return null;
        }

       
        public async Task<string> GetBlobSasToken()
        {
            var credential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(30)
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            return sasBuilder.ToSasQueryParameters(credential).ToString();
        }

        private async Task pruebaAsync()
        {
            var client = _clientFactory.CreateClient("ApiStrapi");
            var request = new HttpRequestMessage(HttpMethod.Put, "http://20.185.227.206:1337/api/crcs/2");
            request.Headers.Add("Authorization", "Bearer b352a293365436bcc28de056b5bd72effd1b92b2c4420ea44e766862c78f1c2dce1829f993cc3961d04204c5107436567320c1d5a4d312bf1263d487244eef7e3d1cafae89e095f9e7881ece95ce51c7d4b7c12dba2749678a69ea79bdae210494671d98dd37be28ddd91db340f1a063f040e1f99dab561513edb910c582a26a");
            var content = new StringContent("{\n    \"data\": {\n        \"capacidad_certificados\": 100,\n        \"horas_de_atencion_por_dia\": 10\n    }\n}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}



