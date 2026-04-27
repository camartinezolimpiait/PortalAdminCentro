using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class RespuestaErrorFacturacion
    {
        #region Properties

        public string Estado { get; set; } = string.Empty;
        public string Proveedor { get; set; } = string.Empty;
        public string ProximaFactura { get; set; } = string.Empty;
        public string UltimaActualizacion { get; set; } = string.Empty;
        public List<DetalleErrorFacturacion> DetalleErrores { get; set; } = [];

        #endregion Properties
    }
}