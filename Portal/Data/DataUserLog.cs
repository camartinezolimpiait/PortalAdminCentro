using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class DataUserLog
    {
        public bool Respuesta { get; set; }
        public string MensajeError { get; set; }
        public string Application { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string OCP { get; set; }
        public bool ChangePassword { get; set; }
        public int? IdPerfil { get; set; }
        public InformacionUsuario InformacionUsuario { get; set; }

    }
    public class InformacionUsuario
    {

        public int Dedo { get; set; }
        public int IdCentro { get; set; }
        public int IdComercio { get; set; }
        public long IdPersona { get; set; }
        public System.Guid IdRol { get; set; }
        public System.Guid IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string TemplateHuella { get; set; }
        public bool TieneExcepcion { get; set; }
        public string UserName { get; set; }
    }

}
