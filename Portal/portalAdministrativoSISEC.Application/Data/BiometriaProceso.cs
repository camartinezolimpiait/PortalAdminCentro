using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class BiometriaProceso
    {
        public int Consecutivo { get; set; }
        public int IdTipo { get; set; }
        public int IdSubtipo { get; set; }
        public string Formato { get; set; }
        public string Buffer { get; set; }
        public string Template { get; set; }

    }
}

