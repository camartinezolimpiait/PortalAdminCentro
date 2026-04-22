using System;

namespace portalAdministrativoSISEC.Data.Pines
{
	public class ResponseConsultaDevolucionDTO
	{
		public string CanalVenta { get; set; }
		public string? Pin { get; set; }
		public string? NumeroIdentificacion { get; set; }
		public string? NombreCompleto { get; set; }
		public string? FechaRegistro { get; set; }
		public string? FechaDevolucion { get; set; }
		public string? TipoDevolucion { get; set; }
		public string? Banco { get; set; }
		public string? CuentaBanco { get; set; }
		public string? Correo { get; set; }
		public string? ValorDevolver { get; set; }
		public string? EstadoDevolucion { get; set; }
		public string? NovedadDevolucion { get; set; }
		public string? AgenteDispersion { get; set; }

        public string NUTVenta { get; set; }

    }
}
