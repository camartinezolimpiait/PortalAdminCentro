using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion
{
    public interface IHorarioAtencionService
    {
        Task<bool> GetEstadoPeticion();
        Task<List<ConfiguracionHorario>> GetConfiguracionHorario(int idPerfil, DateTime fromDate, bool getNewSchedule);
        Task<ResultSaveParametrizationSchedule> SaveConfiguracionHorario(int idPerfil, string usuario, int tipoActualizacion, List<ConfiguracionHorario> configuracionHorarios);
        Task<bool> SaveConfiguracionAgenda(int idPerfil, string usuario, int tipoActualizacion, int intervaloCitas, int agendaDisponible, List<ConfiguracionHorario> configuracionHorarios);
    }
}
