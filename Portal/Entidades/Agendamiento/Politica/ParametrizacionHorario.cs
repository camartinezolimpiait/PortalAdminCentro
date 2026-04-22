using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Entidades.Agendamiento.Politica
{
    /// <summary>
    /// Objeto parametrización horario
    /// </summary>
    public class ParametrizacionHorario
    {
        public int IdParametrizacionHorario { get; set; }
        public int IdPerfil { get; set; }
        public Int16 Intervalo { get; set; }
        public Int16 DisponibilidadAgendaDias { get; set; }
        public Int16 CuposPorIntervalo { get; set; }
        public Int16 TiempoHoraMinimoAgenda { get; set; }
        public Int16 TiempoHoraMinimoCancelar { get; set; }
        public Int16 IdPoliticaAgendamiento { get; set; }
        public Int16 IdTipoAgenda { get; set; }
        public string UsuarioCreacion { get; set; } = "admin";
        public int IdParametroHorario { get; set; }
        public string TransaccionGuid { get; set; }
    }
}
