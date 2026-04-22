using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using System.Threading.Tasks;
using System;
using portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo;
using System.Linq;

namespace portalAdministrativoSISEC.Util.Middleware
{
    // Clase creada para realizar la captura de errores desde los componentes de blazor 
    public class BlazorExceptionLogger : IErrorBoundaryLogger
    {
        private readonly IPortalAdministrativoService _exceptionLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BlazorExceptionLogger(IPortalAdministrativoService exceptionLogService, IHttpContextAccessor httpContextAccessor)
        {
            _exceptionLogService = exceptionLogService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask LogErrorAsync(Exception exception)
        {
            var context = _httpContextAccessor.HttpContext;

            var exceptionLog = new ExceptionLog
            {
                MessageLog = exception.Message,
                ExceptionType = exception.GetType().ToString(),
                TimestampLog = DateTime.Now,
                StackTraceLog = exception.ToString(),
                TransactionIdLog = context?.TraceIdentifier ?? Guid.NewGuid().ToString(),
                SorceLog = "BlazorComponent",
                LayerSourceLog = "UI",
                ApplicationSourceLog = "Portal Administrativo", // Ajusta al nombre de tu aplicación
                MachineNameLog = Environment.MachineName,
                IPAddressLog = context?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown",
                UserLog = context?.User?.Identity?.Name ?? "Anonymous",
                CorrelationKey = context?.Request?.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString()
            };

            await _exceptionLogService.RegisterExceptionLog(exceptionLog);
        }
    }
    // Implementacion en front  pendiente por validar

    //    <ErrorBoundary>
    //    <ChildContent>
    //        <!-- Tus componentes aquí -->
    //    </ChildContent>
    //    <ErrorContent Context = "exception" >
    //        < p > Ocurrió un error: @exception.Message</p>
    //    </ErrorContent>
    //</ErrorBoundary>

}
