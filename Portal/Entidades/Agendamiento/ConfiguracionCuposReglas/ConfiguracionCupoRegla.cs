using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Entidades.Agendamiento.ConfiguracionCuposReglas
{
    public class ConfiguracionCupoRegla
    {
        public short CuposIntervalo { get; set; } = 1;
        public short IdTipoAgenda { get; set; } = 1;
        public short TiempoHoraMinAgenda { get; set; } = 1;
        public short TiempoHoraMinCancelar { get; set; } = 1;

    }
}
