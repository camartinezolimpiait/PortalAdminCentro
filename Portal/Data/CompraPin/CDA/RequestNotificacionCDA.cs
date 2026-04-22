namespace portalAdministrativoSISEC.Data.CompraPin.CDA
{
	public class RequestNotificacionCDA
	{
		public string Pin { get; set; }
		public int? TipoIdentificacion { get; set; }
		public string NumeroIdentificacion { get; set; }
		public string IdRunt { get; set; }
		public string CorreoElectonico { get; set; }
		public string Direccion { get; set; }	
		public string Centro { get; set; }
	}
}
