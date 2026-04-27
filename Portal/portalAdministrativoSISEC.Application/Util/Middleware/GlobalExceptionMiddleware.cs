using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using System.Threading.Tasks;
using System;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia.PortalAdministrativo;
using System.Linq;

namespace portalAdministrativoSISEC.Util.Middleware
{
    // Metodo realizado para capturar los errores del middleware del portal administrativo 
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, IPortalAdministrativoService exceptionLogService)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en la aplicación");

                // Registrar en tu sistema de logs
                await RegisterException(ex, httpContext, exceptionLogService);

                throw; // Re-lanzar para que el framework maneje la respuesta
            }
        }

        private async Task RegisterException(Exception ex, HttpContext context, IPortalAdministrativoService exceptionLogService)
        {
            var exceptionLog = new ExceptionLog
            {
                MessageLog = ex.Message,
                ExceptionType = ex.GetType().ToString(),
                TimestampLog = DateTime.Now,
                StackTraceLog = ex.ToString(),
                TransactionIdLog = context.TraceIdentifier,
                SorceLog = "GlobalExceptionMiddleware",
                LayerSourceLog = "Middleware",
                ApplicationSourceLog = "BlazorApp", // Ajusta al nombre de tu aplicación
                MachineNameLog = Environment.MachineName,
                IPAddressLog = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                UserLog = context.User?.Identity?.Name ?? "Anonymous",
                CorrelationKey = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString()
            };

            await exceptionLogService.RegisterExceptionLog(exceptionLog);
        }
    }
}

