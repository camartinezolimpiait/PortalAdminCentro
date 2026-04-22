namespace portalAdministrativoSISEC.Entidades.Pago.Wompi
{
	public class ResponseReferenciaWompi
	{
		public Respuesta Respuesta { get; set; } = new();
		public string UrlRedireccion { get; set; } = "";
		public string? Nut { get; set; } =string.Empty;	
		public string? FechaPago { get; set; }=string.Empty;
	}
}