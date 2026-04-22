namespace portalAdministrativoSISEC.Entidades.Devolucion
{
	public class ConsultaDevolucionPorPinRequest
	{
		public string Pin { get; set; } = "";
		public int TipoIdentificacion { get; set; }
		public string NumeroIdentificacion { get; set; } = "";
		public string IdRunt { get; set; } = "";
	}
}