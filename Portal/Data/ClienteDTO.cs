using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class ClienteDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public Guid ApplicationId { get; set; }
    }
}
