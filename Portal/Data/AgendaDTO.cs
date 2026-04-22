using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Enum;

namespace portalAdministrativoSISEC.Data
{
    public class AgendaDTO
    {
        public Int64 IdAgenda { get; set; }
        [Required]
        public long IdHorarioAtencion { get; set; }

        [Required]
        public int IdPerfil { get; set; }

        [Required]
        public DateTime FechaAgenda { get; set; }

        [Required]
        public short HorarioInicioAgenda { get; set; }

        [Required]
        public short HorarioFinAgenda { get; set; }

        [Required]
        public EstadoAgenda IdEstadoAgenda { get; set; }

        [Required]
        public TipoCitaAgenda IdTipoCita { get; set; }

        [Required]
        public TipoAgendaCliente IdTipoAgendaCliente { get; set; }

        [Required]
        public Int64 IdTramite { get; set; }

        [Required]
        public short IdMotivo { get; set; }

        public string Categoria { get; set; }

        public string NumeroPin { get; set; }

        public DatosPersonaDTO DatosPersonaDTO { get; set; }
        public TramiteDTO TramiteDto { get; set; }
        public List<int> Intervalos { get; set; }
        public string UsuarioCreacion { get; set; }
        public string Observaciones { get; set; }
		public short IdTipoCliente { get; set; }
	}

    public class ResultConsultaAgendaDTO
    {
        public Int64 IdAgenda { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaAgenda { get; set; }
        public Int16 HoraInicioAgenda { get; set; }

        public string HoraString()
        {
            if (HoraInicioAgenda <= 0 && HoraInicioAgenda.ToString().Length < 3) return string.Empty;
            string hora = HoraInicioAgenda.ToString().Length == 3 ? HoraInicioAgenda.ToString().Substring(0, 1) : HoraInicioAgenda.ToString().Substring(0, 2);
            string minutos = HoraInicioAgenda.ToString().Substring((HoraInicioAgenda.ToString().Length - 2), 2);
            return string.Concat(hora, ":", minutos);
        }
    }
}
