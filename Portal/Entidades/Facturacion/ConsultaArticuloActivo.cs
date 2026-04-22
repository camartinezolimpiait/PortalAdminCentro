namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class ConsultaArticuloActivo : ConsultaEstadoFacturacion
    {
        #region Properties

        /// <summary>
        /// 1. CRC
        /// 2. CEA
        /// </summary>
        public string IdTipoPin { get; set; } = string.Empty;

        #endregion Properties
    }
}
