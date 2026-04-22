using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class EntradaDocumentos
    {
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public bool EsAspirante { get; set; }
        public long? IdPersona { get; set; }
        public long? IdProceso { get; set; }
        public string Archivo { get; set; }
        public string ArchivoRNEC { get; set; }
        public int? ClienteId { get; set; }
    }
}
