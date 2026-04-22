using System;

namespace portalAdministrativoSISEC.Data.Pines
{
    public class ResponseInfoPinEstadoUsadosDTO
    {
        public string CanalVenta { get; set; }
        public string? Pin { get; set; }
        public string? TipoIdentificacion { get; set; }
        public string? NumeroIdentificacion { get; set; }
        public float? ValorTransaccion { get; set; }
        public float? ValorActor { get; set; }
        public float? ValorAns { get; set; }
        public float? ValorAliado { get; set; }
        public float? ValosSicov { get; set; }
        public string? FechaOperacion { get; set; }
        public string? FechaDispersion { get; set; }
        public string? Banco { get; set; }
        public string? CtaDispersion { get; set; }
        public float? ValorDispersado { get; set; }
        public string? AgenteDispersion { get; set; }
        public string? Estado { get; set; }
        public string? RazonSocial { get; set; }
        public string? TipoPin { get; set; }

        public string NUTVenta { get; set; }
    }
}
