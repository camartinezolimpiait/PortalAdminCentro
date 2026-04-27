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

    public class InfoLog
    {
        public int InfoLogId { get; set; }
        public string? LevelLog { get; set; }
        public string? ApiEndpointLog { get; set; }
        public string? HttpMethodLog { get; set; }
        public string? RequestPayloadLog { get; set; }
        public string? ResponsePayloadLog { get; set; }
        public string? StatusCodeLog { get; set; }
        public string? CorrelationKey { get; set; }
        public DateTime? TimestampLog { get; set; }
    }

    public class ExceptionLog
    {
        public int ExceptionLogId { get; set; }
        public string? MessageLog { get; set; }
        public string? ExceptionType { get; set; }
        public DateTime? TimestampLog { get; set; }
        public string? StackTraceLog { get; set; }
        public string? TransactionIdLog { get; set; }
        public string? SorceLog { get; set; }
        public string? LayerSourceLog { get; set; }
        public string? ApplicationSourceLog { get; set; }
        public string? MachineNameLog { get; set; }
        public string? IPAddressLog { get; set; }
        public string? UserLog { get; set; }
        public string? CorrelationKey { get; set; }
    }

    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        Critical = 4,
        Trace = 5
    }
}
