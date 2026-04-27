using System.Collections.Generic;

namespace portalAdministrativoSISEC.Application.Data.CompraPin.CDA
{
    public class DiscriminadoValorPinCDA
    {
        public double Sicov { get; set; } = 0;

        public double Banco { get; set; } = 0;

        public double CDA { get; set; }

        public double Ansv { get; set; } = 0;

        public List<CostoCuotaCDA> CalculoCoutas { get; set; } = new List<CostoCuotaCDA>();

        public double ValorTotal { get; set; } = 0;
    }
}

