using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data.ValidaPinCEA
{
    public class EntidadCPPersona
    {
        public string applicationname { get; set; }
        public string celular { get; set; }
        public string correoelectronico { get; set; }
        public string direccion { get; set; }
        public int epsars { get; set; }
        public short estadocivil { get; set; }
        public DateTime fechanacimiento { get; set; }
        public string firma { get; set; }
        public string fotografia { get; set; }
        public short gradoescolaridad { get; set; }
        public int indicativotelefono { get; set; }
        public int lugarexpedicion { get; set; }
        public int lugarnacimiento { get; set; }
        public string numerodocumento { get; set; }
        public int ocupacion { get; set; }
        public string primerapellido { get; set; }
        public string segundoapellido { get; set; }
        public string primernombre { get; set; }
        public string segundonombre { get; set; }
        public short sexo { get; set; }
        public int telefono { get; set; }
        public int tipoidentificacion { get; set; }
        public byte gruposanguinio { get; set; }
        public short regimenafiliacion { get; set; }
        public DateTime fechaexpedicion { get; set; }
        public int idresidencia { get; set; }
        public long? idpersona { get; set; }
    }
}

