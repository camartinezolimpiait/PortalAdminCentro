using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class EstadoFacturacion
    {
        #region Properties

        public string CodigoProveedorTecnologico { get; set; } = string.Empty;
        public bool HabilitadoFacturaElectronica { get; set; } = false;
        public string NombreProveedorTecnologico { get; set; } = string.Empty;

        #endregion Properties
    }
}