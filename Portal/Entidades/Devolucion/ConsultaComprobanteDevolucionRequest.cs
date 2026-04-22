namespace portalAdministrativoSISEC.Entidades.Devolucion
{
	public class ConsultaComprobanteDevolucionRequest
	{
		public string Pin { get; set; } = "";
		public string NumeroIdentificacion { get; set; } = "";
		public int IdOrigenCotizacion { get; set; }
	}
}
