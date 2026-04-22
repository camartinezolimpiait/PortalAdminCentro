namespace portalAdministrativoSISEC.Entidades.Devolucion
{
	public class ConsultaDevolucion
	{
		public string Pin { get; set; } = "";
		public string TipoNegocio { get; set; } = "";
		public string Centro { get; set; } = "";
		public string FechaDeSolicitud { get; set; } = "";
		public string Estado { get; set; } = "";
		public string FechaDevolucion { get; set; } = "";
		public string TipoDevolucion { get; set; } = "";
		public string Observacion { get; set; } = "";
		public string OrigenRecaudo { get; set; } = "";
		public string OrigenRegistro { get; set; } = "";
		public bool PermitirIntento { get; set; }
	}
}