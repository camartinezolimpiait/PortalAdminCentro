// Inicio código generado por GitHub Copilot
namespace portalAdministrativoSISEC.Enum.PortalAdministrativo
{
    /// <summary>
    /// Códigos de error para validación de cupones de convenio
    /// </summary>
    public enum EnumCodigoErrorCupon
    {
        /// <summary>
        /// Código de cupón inválido o no encontrado
        /// </summary>
        CodigoInvalido = -1,

        /// <summary>
        /// El convenio asociado al cupón ha expirado
        /// </summary>
        ConvenioExpirado = -2,

        /// <summary>
        /// El convenio asociado al cupón está inactivo
        /// </summary>
        ConvenioInactivo = -3,

        /// <summary>
        /// El convenio no aplica para el tipo de servicio actual (CEA/CRC)
        /// </summary>
        ConvenioNoAplicaServicio = -4,

        /// <summary>
        /// El convenio no está disponible actualmente
        /// </summary>
        ConvenioNoDisponible = -5
    }
}
// Fin código generado por GitHub Copilot
