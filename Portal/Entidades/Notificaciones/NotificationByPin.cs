namespace portalAdministrativoSISEC.Entidades.Notificaciones
{
    /// <summary>
    /// Representa los parámetros para enviar notificación por PIN
    /// </summary>
    public class NotificationByPin
    {
        /// <summary>
        /// PIN generado para la transacción
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// Tipo de notificación (1: Email referencia, 0: Email normal)
        /// </summary>
        public int NotificationType { get; set; }

        /// <summary>
        /// Destinatario de la notificación
        /// </summary>
        public int Recipient { get; set; }
    }
}
