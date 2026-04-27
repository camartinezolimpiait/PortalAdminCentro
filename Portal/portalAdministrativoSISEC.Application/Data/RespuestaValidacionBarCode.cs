using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class RespuestaValidacionBarCode
    {
        public string Identificacion { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string Genero { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string FactorRH { get; set; }
        public bool Validado { get; set; }
        public string DescripcionValidacion { get; set; }
    }
}

