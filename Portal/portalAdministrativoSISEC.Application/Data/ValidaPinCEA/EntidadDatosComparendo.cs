using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data.ValidaPinCEA
{
    public class EntidadDatosComparendo
    {
        public Int32 idDatosComparendo { get; set; }
        public Int32 idProceso { get; set; }
        public String NombreOrganismoTransito { get; set; }
        public byte IdTipoComparendo { get; set; }
        public DateTime FechaComparendo { get; set; }
        public string HoraComparendo { get; set; }
        public float ValorComparendo { get; set; }
        public Int32 idCodigosComparendo { get; set; }
        public string NumeroResolucion { get; set; }
    }
}

