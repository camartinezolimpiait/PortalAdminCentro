namespace portalAdministrativoSISEC.Entidades.Pago.Wompi
{
	public class ReferenciaBancolombiaWompi
	{
		public int IdCliente { get; set; }
		public Data Data { get; set; } = new();
		public long IdDescripcionUserOTP { get; set; }
	}
}