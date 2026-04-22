using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class RespuestaEnrolarPersona
    {
        public string codigoError { get; set; }
        public string DescripcionError { get; set; }
        public long IdPersonaPortal { get; set; }
        public long IdProcesoPortal { get; set; }
        public long IdPersonaCRCCEA { get; set; }
        public long IdProcesoCRCCEA { get; set; }
        public int idTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string User { get; set; }
    }
}
