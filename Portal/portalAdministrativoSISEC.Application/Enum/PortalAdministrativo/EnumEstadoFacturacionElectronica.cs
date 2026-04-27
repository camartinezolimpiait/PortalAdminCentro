namespace portalAdministrativoSISEC.Enum.PortalAdministrativo
{
    // Inicio código generado por GitHub Copilot
    /// <summary>
    /// Enum que define los estados de facturación electrónica
    /// Cada valor representa una pestaña en la interfaz de consulta
    /// </summary>
    public enum EnumEstadoFacturacionElectronica
    {
        /// <summary>
        /// Agrupa: "En error de configuración", "En error de conexión"
        /// </summary>
        PorRevisar = 1,

        /// <summary>
        /// Agrupa: "Facturada"
        /// </summary>
        Facturadas = 2,

        /// <summary>
        /// Agrupa: "Anulado"
        /// </summary>
        Anuladas = 3,

        /// <summary>
        /// Agrupa: "Registrado", "Listo para procesar", "Encolados", "Procesando"
        /// </summary>
        EnCola = 4
    }
    // Fin código generado por GitHub Copilot
}
