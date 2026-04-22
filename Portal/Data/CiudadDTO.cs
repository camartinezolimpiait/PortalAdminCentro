using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class CiudadDTO
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public string NombreMunicipio { get; set; }
        public int? CodigoEquCRC { get; set; }
        public int? CodigoEquCEA { get; set; }
        public int DepartamentoId { get; set; }
    }
}
