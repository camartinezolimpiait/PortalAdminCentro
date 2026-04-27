using System;

namespace portalAdministrativoSISEC.Application.Data.Pines
{
    public class ResponsePinesAsociados
    {
        public int CanalVenta { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaDispersion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? NumeroIdentificacion { get; set; }
        public string? Pin { get; set; }
        public string? RazonSocial { get; set; }
        public string? TipoIdentificacion { get; set; }
        public string? TipoPin { get; set; }
        public float? ValorActor { get; set; }
        public float? ValorAliado { get; set; }
        public float? ValorAns { get; set; }
        public float? ValorTransaccion { get; set; }
        public float? ValosSicov { get; set; }
        public int TotalRegistros { get; set; } = 1;
        public DateTime? FRecaudo { get; set; }

        //PinDirecto
        public string Banco { get; set; } = string.Empty;
		public string CtaDispersion { get; set; } = string.Empty;
		public float? ValorDispersado { get; set; } = 0;
		public string AgenteDispersion { get; set; } = string.Empty;
	}
}

