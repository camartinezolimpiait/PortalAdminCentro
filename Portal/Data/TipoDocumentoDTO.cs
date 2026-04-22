using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class TipoDocumentoDTO
    {
        public string Id { get; set; }
        public int ClienteId { get; set; }
        public string Codigo { get; set; }
        public string abreviatura { get; set; }
        public string descripcion { get; set; }
        public int? CodigoEquCRC { get; set; }
        public int? CodigoEquCEA { get; set; }
    }
}
