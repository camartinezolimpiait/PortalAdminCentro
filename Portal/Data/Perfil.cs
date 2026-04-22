using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class Perfil
    {
       public int perfilId { get; set; }
        public string nombre { get; set; }
        public int clienteId { get; set; }
        public int aplicacionId { get; set; }
        public bool esPerfilSistema { get; set; }
        public bool activo { get; set; }
        public string transaccionGuid { get; set; }
    }
}
