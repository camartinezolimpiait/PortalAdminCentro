using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class PinPrincipalProceso
    {
        public long IdProceso { get; set; }
        public string Pin { get; set; }
        public long? IdPinPrincipalProceso { get; set; }
        public int IdTipoIdentificacion { get; set; }
        public string NumeroDocumento { get; set; }
        public int IdCentro { get; set; }
        public decimal ValorPinPrincipal { get; set; }
        public int IdOrigenPin { get; set; }
        public byte IdEstadoUsoPin { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public long? CodigoProcesamiento { get; set; }
        public string Servidor { get; set; }
        public byte IntentoDiario { get; set; }
        public string DescripcionPeticion { get; set; }
    }
}

