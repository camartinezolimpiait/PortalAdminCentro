namespace portalAdministrativoSISEC.Application.Data.CompraPin
{
	public class ConsultaCostoPinCea
	{
		public int IdCentro { get; set; }
		public int IdTramite { get; set; }
		public int IdCategoria { get; set; }
		public int Edad { get; set; }
		public int Genero { get; set; }
		public bool IsPindirecto { get; set; } = false;
		public string ValorAliado { get; set; } = "0";
	}
}

