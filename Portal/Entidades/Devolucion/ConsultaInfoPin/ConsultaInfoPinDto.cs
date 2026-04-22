namespace portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin
{
	public class ConsultaInfoPinDto
	{
		public int Codigo { get; set; }
		public string Respuesta { get; set; } = "";
		public string NumeroAuditoria { get; set; } = "";
		public Entidad Entidad { get; set; } = new();
	}
}