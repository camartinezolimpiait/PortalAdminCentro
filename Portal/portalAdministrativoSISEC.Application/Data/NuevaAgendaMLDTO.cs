using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class NuevaAgendaMLDTO : IdentificacionMLDTO
    {
        public int IdHorarioAtencion { get; set; }
        public DateTime FechaAgenda { get; set; }
        public short HoraInicio { get; set; }
        public short IdTipoCita { get; set; }
        public short IdTipoAgendaCliente { get; set; }
        public string Categoria { get; set; }
        public string pin { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NumeroTelefono { get; set; }
        public string Email { get; set; }
        public short IdGenero { get; set; }
    }

    public class IdentificacionMLDTO : ParametrosAgendaMLDTO
    {
        public int IdTipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
    }

    public class ParametrosAgendaMLDTO
    {
        public string IdRunt { get; set; }
        public long IdCliente { get; set; }
    }
}

