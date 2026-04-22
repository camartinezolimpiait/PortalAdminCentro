// Inicio código generado por GitHub Copilot
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    /// <summary>
    /// DTO para realizar consultas de facturación electrónica
    /// </summary>
    public class RequestFilter
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? TipoDocumento { get; set; }

        public string? NumeroDocumento { get; set; }
        public string? Nombre { get; set; }

        public string? Apellido { get; set; }
        public string? Pin { get; set; }
        public string IdRunt { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? EstadoML { get; set; }
    }
}
// Fin código generado por GitHub Copilot