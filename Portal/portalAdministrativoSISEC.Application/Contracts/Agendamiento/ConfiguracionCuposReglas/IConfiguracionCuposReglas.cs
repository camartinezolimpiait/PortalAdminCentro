using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.Agendamiento.ConfiguracionCuposReglas;

namespace portalAdministrativoSISEC.Application.Contracts.Agendamiento.ConfiguracionCuposReglas
{
    public interface IConfiguracionCuposReglas
    {
        Task<bool> SaveConfiguracionCuposReglas(ConfiguracionCupoRegla configuracionCupoRegla);
    }
}

