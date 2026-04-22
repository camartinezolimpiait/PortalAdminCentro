using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class Categoria
    {
        public int? idCategoria { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public int? idServicio { get; set; }
        public bool? activo { get; set; }
        public int? idGrupo { get; set; }
        public string applicationUser { get; set; }
        public int? rowVersion { get; set; }
        public string transaccionGuid { get; set; }
    }
}
