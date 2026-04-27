using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public int GrupoId { get; set; }
        public int ClienteId { get; set; }
    }
}

