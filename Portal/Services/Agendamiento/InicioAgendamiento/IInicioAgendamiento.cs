using portalAdministrativoSISEC.Entidades.Agendamiento.InicioAgendamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.InicioAgendamiento
{
    public interface IInicioAgendamiento
    {
        Task<List<ConfiguracionAgendamiento>> GetConfiguracionHorario(int idPerfil);
    }
}
