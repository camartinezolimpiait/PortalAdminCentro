using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class EntradaRegistroFirma
    {
        public int idTipoDocumento { get; set; }
        public string Numerodocumento { get; set; }
        public string keyFirma { get; set; }
        public int idCliente { get; set; }
        public int IdAfis { get; set; }
    }
}
