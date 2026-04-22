using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data.ValidaPinCEA
{
    public class ValidaPinPaynetModel
    {
        public string NumeroDocumento { get; set; }
        public string CodigoRunt { get; set; }
        public string CodigoCategoria { get; set; }
        public long idPersona { get; set; }
        public byte idTipoSolicitud { get; set; }
        public int idCategoria { get; set; }
        public long idProceso { get; set; }
        public int idCentro { get; set; }
    }
}
