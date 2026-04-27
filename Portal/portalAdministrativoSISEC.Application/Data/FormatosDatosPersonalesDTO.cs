using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class FormatosDatosPersonalesDTO
    {
        public int IdFormato { get; set; }
        public int IdServicio { get; set; }
        public bool EsParaAspirantes { get; set; }
        public int IdTipoIdentificacion { get; set; }
        public string Texto { get; set; }
        public byte[] Plantilla { get; set; }
        public bool Activo { get; set; }
        public string LoginUsuarioCrea { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string LoginUsuarioModifica { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}

