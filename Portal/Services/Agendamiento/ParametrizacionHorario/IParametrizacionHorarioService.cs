using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;

namespace portalAdministrativoSISEC.Services.Agendamiento
{
    public interface IParametrizacionHorarioService
    {
        Task<bool> GetEstadoPeticion();
        Task<ParametrizacionHorario> GetParametrizationScheduleByProfileId(int idPerfil, int idParametroHorario);
        Task<ParametrizacionHorario> GetParametrizationScheduleByProfileIdAndParametroHorario(int idPerfil, int idParametroHorario);
        Task<ResultSaveParametrizationSchedule> SaveParametrizationSchedule(ParametrizacionHorario parametrizacionHorario,DateTime fromDate);
    }
}
