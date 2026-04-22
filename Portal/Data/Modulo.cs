using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class Modulo
    {
        public int Id { get; set; }
        public int? Padre { get; set; }
        public int AplicacionId { get; set; }
        public bool EsOpcionMenu { get; set; }
        public string Ruta { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
        public string Pagina { get; set; }
    }
}
