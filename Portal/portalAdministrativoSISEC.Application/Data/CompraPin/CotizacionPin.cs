using System.Collections.Generic;

namespace portalAdministrativoSISEC.Application.Data.CompraPin
{
	public class CotizacionPin
	{
		public double SICOV { get; set; }
		public double BANCO { get; set; }
		public double CRC { get; set; }
		public double ANSV { get; set; }
		public List<CostoCuota> CalculoCoutas { get; set; } = new();
		public double ValorTotal { get; set; }
	}
    public class CotizacionPinOld
    {
        public int IdCentro { get; set; }
        public int IdTramite { get; set; }
        public int IdCategoria { get; set; }
        public decimal ValorSicov { get; set; }
        public decimal ValorANSV { get; set; }
        public decimal ValorAliado { get; set; }
        public decimal ValorBase { get; set; }
        public List<CostoCuota> CalculoCoutas { get; set; } = new List<CostoCuota>();
        public double ValorTotal { get; set; }
    }

}

