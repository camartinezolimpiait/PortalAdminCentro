using System;

namespace portalAdministrativoSISEC.Entidades.PortalAdministrativo
{
    public class AuditLogDetail
    {
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public string RequestBody { get; set; } = "";
        public string ResponseBody { get; set; } = "";
        public string LatencyMs { get; set; } = "";
    }
    public class AuditLog : AuditLogDetail
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
    /// <summary>
    /// Representa la tabla Log.InfoLogs
    /// </summary>
    public class InfoLog
    {
        /// <summary>
        /// Clave primaria de tipo entero, no nulo
        /// </summary>
        public int InfoLogId { get; set; }

        /// <summary>
        /// Nivel de log (varchar(10), no nulo)
        /// </summary>
        public string? LevelLog { get; set; }

        /// <summary>
        /// Endpoint de la API (varchar(100), no nulo)
        /// </summary>
        public string? ApiEndpointLog { get; set; }

        /// <summary>
        /// Método HTTP (varchar(10), no nulo)
        /// </summary>
        public string? HttpMethodLog { get; set; }

        /// <summary>
        /// Payload de la solicitud (varchar(4000), no nulo)
        /// </summary>
        public string? RequestPayloadLog { get; set; }

        /// <summary>
        /// Payload de la respuesta (varchar(4000), nulo)
        /// </summary>
        public string? ResponsePayloadLog { get; set; }

        /// <summary>
        /// Código de estado HTTP (varchar(20), nulo)
        /// </summary>
        public string? StatusCodeLog { get; set; }

        /// <summary>
        /// Clave de correlación (varchar(25), nulo)
        /// </summary>
        public string? CorrelationKey { get; set; }

        /// <summary>
        /// Fecha y hora del registro (datetime, nulo)
        /// </summary>
        public DateTime? TimestampLog { get; set; }
    }

    /// <summary>
    /// Representa la tabla Log.ExceptionLogs
    /// </summary>
    public class ExceptionLog
    {
        /// <summary>
        /// Clave primaria de tipo entero, no nulo
        /// </summary>
        public int ExceptionLogId { get; set; }

        /// <summary>
        /// Mensaje de la excepción (varchar(max), nulo)
        /// </summary>
        public string? MessageLog { get; set; }

        /// <summary>
        /// Tipo de excepción (varchar(100), nulo)
        /// </summary>
        public string? ExceptionType { get; set; }

        /// <summary>
        /// Fecha y hora del registro (datetime, nulo)
        /// </summary>
        public DateTime? TimestampLog { get; set; }

        /// <summary>
        /// Stack trace de la excepción (varchar(max), nulo)
        /// </summary>
        public string? StackTraceLog { get; set; }

        /// <summary>
        /// ID de la transacción (varchar(max), nulo)
        /// </summary>
        public string? TransactionIdLog { get; set; }

        /// <summary>
        /// Origen de la excepción (varchar(max), nulo)
        /// </summary>
        public string? SorceLog { get; set; }

        /// <summary>
        /// Capa de origen (varchar(max), nulo)
        /// </summary>
        public string? LayerSourceLog { get; set; }

        /// <summary>
        /// Fuente de la aplicación (varchar(max), nulo)
        /// </summary>
        public string? ApplicationSourceLog { get; set; }

        /// <summary>
        /// Nombre de la máquina (varchar(max), nulo)
        /// </summary>
        public string? MachineNameLog { get; set; }

        /// <summary>
        /// Dirección IP (varchar(max), nulo)
        /// </summary>
        public string? IPAddressLog { get; set; }

        /// <summary>
        /// Usuario (varchar(max), nulo)
        /// </summary>
        public string? UserLog { get; set; }

        /// <summary>
        /// Clave de correlación (varchar(100), nulo)
        /// </summary>
        public string? CorrelationKey { get; set; }
    }

    /// <summary>
    /// Represents the severity levels for logging
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Detailed information for debugging purposes
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Informational messages about normal application operation
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning messages indicating potential issues
        /// </summary>
        Warn = 2,

        /// <summary>
        /// Error messages indicating more serious problems
        /// </summary>
        Error = 3,

        /// <summary>
        /// Critical errors that may cause application failure
        /// </summary>
        Critical = 4,

        /// <summary>
        /// Trace-level logging for very detailed diagnostic information
        /// </summary>
        Trace = 5
    }

}
