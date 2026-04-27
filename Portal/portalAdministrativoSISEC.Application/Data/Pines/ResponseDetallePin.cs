using System;

namespace portalAdministrativoSISEC.Application.Data.Pines
{
    public class ResponseDetallePin
    {
        public string? Banco { get; set; }
        public DateTime? FechaDispersion { get; set; }
        public string? NumeroAutorizacion { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? Pin { get; set; }
        public float? ValorDispersado { get; set; }
    }
}

