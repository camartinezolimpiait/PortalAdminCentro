using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Data.Pines
{
    public class ResponseInfoPinEstado : ResponseConsultarPinesAsociados
    {
        public string Cuotas { get; set; }
        public int TotalRegistros { get; set; }
        public DateTime? FRegistro { get; set; }
        public DateTime? FDispersion { get; set; }
        public string EsPinPadre { get; set; }
        public int? IdAgenteDispersion { get; set; }
        public int? IdDispersion { get; set; }
        public string NUTVenta { get; set; }
        public int? PagosRealizados { get; set; }
        
        // Inicio código generado por GitHub Copilot
        public string TipoDevolucion { get; set; }
        // Fin código generado por GitHub Copilot
    }
}