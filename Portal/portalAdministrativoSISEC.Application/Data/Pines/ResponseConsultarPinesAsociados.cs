using System;

namespace portalAdministrativoSISEC.Application.Data.Pines
{
    public class ResponseConsultarPinesAsociados
    {
        public int CanalVenta { get; set; }
        public string Pin { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public float? ValorTransaccion { get; set; }
        public float? ValorActor { get; set; }
        public DateTime? FechaRegistro { get; set; }
        // Inicio código generado por GitHub Copilot
        public DateTime? FechaDevolucion { get; set; }
        public string? NovedadDevolucion { get; set; }
        public string? Correo { get; set; }
        public string CuentaBanco { get; set; }
        // Fin código generado por GitHub Copilot
        public string Estado { get; set; }
        public string FechaDispersion { get; set; }
        public string Banco { get; set; }
        public string CtaDispersion { get; set; }
        public float? ValorDispersado { get; set; }
        public float? ValorAliado { get; set; }
        public float? ValorAns { get; set; }
        public float? ValosSicov { get; set; }
        public string RazonSocial { get; set; }
        public string TipoPin { get; set; }
        public string AgenteDispersion { get; set; }

        public int? PagosRealizados { get; set; }
        public string? NombreCompleto { get; set; }
        public string? NUTVenta { get; set; }

    }
}
