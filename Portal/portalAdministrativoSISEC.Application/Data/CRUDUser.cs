using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class CRUDUser
    {
        [JsonProperty("idCentro")]
        public int IdCentro { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("CodigoUso")]
        public string CodigoUso { get; set; }
        [JsonProperty("IdComercio")]
        public int IdComercio { get; set; }
        [JsonProperty("IdDepartamento")]
        public int IdDepartamento { get; set; }
        [JsonProperty("IdMunicipio")]
        public int IdMunicipio { get; set; }
        [JsonProperty("IdZona")]
        public int IdZona { get; set; }
        [JsonProperty("Direccion")]
        public string Direccion { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("Fijo")]
        public string Fijo { get; set; }
    }
}

