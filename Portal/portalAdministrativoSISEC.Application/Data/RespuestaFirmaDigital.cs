using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class RespuestaFirmaDigital
    {
        public int CodigoRespuesta { get; set; }
        public string DescripcionRespuesta { get; set; }
        public int IdConsulta { get; set; }
        public string NombreArchivo { get; set; }
        public string Archivo { get; set; }
    }
}

