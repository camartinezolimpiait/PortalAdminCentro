using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class TramiteDTO
    {
        public Int16 IdTramite { get; set; }
        public Int16 IdTipoCliente { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public Int16 IdTramiteCliente { get; set; }
    }
}

