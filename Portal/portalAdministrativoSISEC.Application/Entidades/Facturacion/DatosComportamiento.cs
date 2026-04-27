namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class DatosComportamiento
    {
        #region Properties

        public string IdRunt { get; set; } = string.Empty;
        public bool ActivarFacturacion { get; set; } = false;
        public string CodigoMomentoFacturacion { get; set; } = string.Empty;
        public string UsuarioPortal { get; set; } = string.Empty;

        #endregion Properties
    }
}
