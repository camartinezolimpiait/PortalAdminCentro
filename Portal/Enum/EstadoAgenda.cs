using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Enum
{
    public enum EstadoAgenda
    {
        Activo = 1,
        Cancelado = 2,
        Inhabilitado = 3,
        DiaInhabilitado = 4,
    }

    public enum TipoCita
    {
        Disponible = 1,
        BloqueadoPorConfiguracion = 2,
        BloqueadoPorRecepcion = 3,
        Ocupado = 4,
        CupoLleno = 5,
        Almuerzo = 6
    }

    enum ScheduleColumns
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }

    public enum TipoAgendaCliente
    {
        PortalAdministrativo = 1,
        MiLicencia = 2,
        Aliado = 3
    }

    enum Genero
    {
        Masculino = 1,
        Femenino = 2
    }

    enum Plataforma
    {
        CRC = 1,
        CEA = 2,
        Armas = 3
    }

    public enum TipoCitaAgenda
    {
        Enrolamiento = 1,
        ContinuarProceso = 2
    }

    public enum ScheduleForms
    {
        NewSchedule,
        ScheduleDetail,
        CancelSchedule,
        ReSchedule,
        ReScheduleConfirmation,
        None
    }

    public enum AgendaMode
    {
        Schedule,
        Search
    }
}
