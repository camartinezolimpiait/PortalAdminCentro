using System.Collections.Generic;

namespace portalAdministrativoSISEC.Data.CompraPin.CDA
{
	public class CotizacionPinOld
	{
		public double SICOV { get; set; }
		public double BANCO { get; set; }
		public double CRC { get; set; }
		public double ANSV { get; set; }
		public List<CostoCuotaCDA> CalculoCoutas { get; set; } = new();
		public double ValorTotal { get; set; }
	}
    public class CotizacionPinCDA
    {
        public int IdCentro { get; set; }
        public int IdTramite { get; set; }
        public int IdCategoria { get; set; }
        public decimal ValorCDA { get; set; }
        public decimal ValorSicov { get; set; }
        public decimal ValorANSV { get; set; }
        public decimal ValorAliado { get; set; }
        public List<CostoCuotaCDA> CalculoCoutas { get; set; } = new List<CostoCuotaCDA>();
        public double ValorTotal { get; set; }
    }

}
