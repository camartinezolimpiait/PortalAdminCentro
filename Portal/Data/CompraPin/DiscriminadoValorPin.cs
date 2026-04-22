using System.Collections.Generic;

namespace portalAdministrativoSISEC.Data.CompraPin
{
    public class DiscriminadoValorPin
    {
        public double Sicov { get; set; } = 0;

        public double Banco { get; set; } = 0;

		public double Crc { get; set; }

        public double Ansv { get; set; } = 0;

		public List<CostoCuota> CalculoCoutas { get; set; } = new List<CostoCuota>();

        public double ValorTotal { get; set; } = 0;
	}
}
