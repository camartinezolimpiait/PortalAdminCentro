namespace portalAdministrativoSISEC.Data.CompraPin
{
	public class ConvenioCentro
	{
		public int IdOrigenPin { get; set; } = 0;
		public string ConvenioNombre { get; set; } = "";
		public string VentaConvenio { get; set; } = "";
		public string NombreInterno { get; set; } = "";
		public bool GeneraPin { get; set; } = false;
        public bool ComprasCuotas { get; set; }
    }
}