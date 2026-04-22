namespace portalAdministrativoSISEC.Entidades.Notificaciones
{
    /// <summary>
    /// Representa la respuesta al enviar una notificación por PSE.
    /// </summary>
    public class SendNotificationPseResponse
    {
        /// <summary>
        /// Código de resultado de la operación.
        /// </summary>
        public int Codigo { get; set; }

        /// <summary>
        /// Mensaje de respuesta detallado de la operación.
        /// </summary>
        public string Respuesta { get; set; }

        /// <summary>
        /// Número de auditoría asociado a la notificación enviada.
        /// </summary>
        public string NumeroAuditoria { get; set; }

        /// <summary>
        /// Datos de la entidad relacionada con la notificación.
        /// </summary>
        public EntidadData Entidad { get; set; }
    }

    /// <summary>
    /// Representa los datos de la entidad involucrada en la notificación PSE.
    /// </summary>
    public class EntidadData
    {
        /// <summary>
        /// Código del ciudadano relacionado con la notificación.
        /// </summary>
        public int CitizenCode { get; set; }

        /// <summary>
        /// Código del centro relacionado con la notificación.
        /// </summary>
        public int CenterCode { get; set; }
    }
}
