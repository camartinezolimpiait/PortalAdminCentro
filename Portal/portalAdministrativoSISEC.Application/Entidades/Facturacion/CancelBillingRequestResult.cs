using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class CancelBillingRequestResult
    {
        #region Properties

        public List<CancelBillingResult> Resultados { get; set; }
        public int TotalProcesados { get; set; }
        public int TotalExitosos { get; set; }
        public int TotalFallidos { get; set; }

        #endregion Properties
    }

    public class CancelBillingResult
    {
        #region Properties

        public string PIN { get; set; }
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }

        #endregion Properties
    }
}