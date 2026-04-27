using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class GrupoSanguineoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public int? CodigoEquCRC { get; set; }
        public int? CodigoEquCEA { get; set; }
    }
}

