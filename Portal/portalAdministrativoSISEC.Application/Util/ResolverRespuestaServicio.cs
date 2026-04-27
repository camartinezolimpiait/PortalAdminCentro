using Newtonsoft.Json;
using portalAdministrativoSISEC.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util
{
    public static class ResolverRespuestaServicio<T>
    {
        /// <summary>
        /// Metodo para procesar la respuesta de peticiones formadas pro el objecto RespuestaServicios
        /// </summary>

        public static T procesarRespuesta(HttpResponseMessage response, string data, out bool estadoPeticion)
        {
            estadoPeticion = false;
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    RespuestaServicios resultServicio = JsonConvert.DeserializeObject<RespuestaServicios>(data);
                    if (resultServicio.Estado)
                    {
                        estadoPeticion = true;
                        if (!string.IsNullOrEmpty(resultServicio.Data))
                        {
                            return JsonConvert.DeserializeObject<T>(resultServicio.Data);
                        }
                        else
                            estadoPeticion = true;
                        return default;
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                estadoPeticion = false;
                return default;
            }

        }
    }
}
