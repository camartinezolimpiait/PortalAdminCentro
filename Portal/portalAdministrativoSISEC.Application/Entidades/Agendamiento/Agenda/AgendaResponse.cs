using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Enum;

namespace portalAdministrativoSISEC.Entidades.Agendamiento.Agenda
{
    public class AgendaResponse
    {
    }
    public class ParametrizacionAgenda
    {
        public int IdAgenda { get; set; }
        public short IdHorarioCita { get; set; }
        public int HorarioAperturaAgenda { get; set; }
        public int HorarioCierreAgenda { get; set; }
        public int Intervalo { get; set; }
        public bool IsActivo { get; set; }
    }
    public class AgendaCentro
    {
        public int IdPerfil { get; set; }
        public long IdAgenda { get; set; }
        public DateTime FechaAgenda { get; set; }
        public int HorarioInicioAgenda { get; set; }
        public short IdEstadoAgenda { get; set; }
        public string NombreUsuario { get; set; }
        public TipoCitaAgenda IdTipoCita { get; set; }
		public short IdTipoCliente { get; set; }
		public bool AgendaOtroCliente { get; set; }
	}
    public class CitasIntervalo
    {
        public string NombreIntervalo { get; set; }
        public int HoraIntervalo { get; set; }
        public List<DiasIntevalo> DiasIntervalo { get; set; }
        public bool EsHoraAlmuerzo { get; set; }
        public bool MostrarIntervalo { get; set; }
        public bool DiaInhabilitado { get; set; }

    }
    public class DiasIntevalo
    {
        public List<Cita> Cita { get; set; }
        public bool DiaInhabilitado { get; set; }
        public bool IsChange { get; set; }
        public DateTime FechaAgenda { get; set; }
    }
    public class Cita
    {
        public long IdAgenda { get; set; }
        public DateTime FechaAgenda { get; set; }
        public int HoraInicioAgenda { get; set; }
        public short IdEstadoAgenda { get; set; }
        public TipoCita TipoCita { get; set; }
        public TipoCitaAgenda TipoCitaAgenda { get; set; }
        public string NombreUsuario { get; set; }
        public bool IsCheckedForBlock { get; set; }
        public bool IsChange { get; set; }
        public long IdHorarioAtencion { get; set; }
		public bool AgendaOtroCliente { get; set; }
	}

    public class ActiveCheckboxAllDay
    {
        public string Day { get; set; }
        public bool Status { get; set; }
    }

    public class OptionAppoiment
    {
        public DateTime? ScheduleDay { get; set; }
        public short? ScheduleTime { get; set; }
        public string TimeString { get; set; }
        public long? IdHorarioAtencion { get; set; }
        public long? IdAgenda { get; set; }
        public ScheduleForms ScheduleForm { get; set; }
        public AgendaDTO Agenda { get; set; }
        public ToolButtonOptions ToolButtonOptions { get; set; }
        public DateTime? ScheduleStartDate { get; set; }
    }

    public class ToolButtonOptions
    {
        public bool ShowBlockSchedule { get; set; }
        public AgendaMode AgendaMode { get; set; }
    }
}

