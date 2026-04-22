using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class RespuestaProcesoZeus
    {
        public RespuestaProcesoZeus()
        {
            Biometrias = new List<BiometriaRespuesta>();
        }
        public List<BiometriaRespuesta> Biometrias { get; set; }
        public CandidatoRespuesta Candidato { get; set; }
        public string codigoError { get; set; }
        public string DescripcionError { get; set; }
        public string Estado { get; set; }
        public string EstadoPeticion { get; set; }
    }

    public class BiometriaRespuesta
    {
        public int Consecutivo { get; set; }
        public int IdTipo { get; set; }
        public int IdSubtipo { get; set; }
        public int Score { get; set; }
        public string Resultado { get; set; }
        public string Error { get; set; }
        public string Formato { get; set; }
        public string Buffer { get; set; }

    }

    public class CandidatoRespuesta
    {
        public int IdTipoDocumento { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string ExpLugar { get; set; }
        public string ExpFecha { get; set; }
        public string Vigencia { get; set; }
    }
}
