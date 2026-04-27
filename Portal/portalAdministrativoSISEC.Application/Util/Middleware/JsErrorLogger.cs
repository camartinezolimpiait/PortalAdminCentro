using Microsoft.JSInterop;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using System.Threading.Tasks;
using System;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia.PortalAdministrativo;
using Microsoft.Extensions.DependencyInjection;

namespace portalAdministrativoSISEC.Util.Middleware
{

    // Metodo investigado para capturar los eventos de errores de javascript en el lado del cliente - Logs de auditoria
    public static class JsErrorLogger
    {
        [JSInvokable]
        public static async Task LogJsError(string message, string source, int lineNumber, string stack)
        {
            using var scope = ServiceActivator.GetScope();
            var exceptionLogService = scope.ServiceProvider.GetRequiredService<IPortalAdministrativoService>();

            var exceptionLog = new ExceptionLog
            {
                MessageLog = message,
                ExceptionType = "JavaScript Error",
                TimestampLog = DateTime.Now,
                StackTraceLog = stack,
                SorceLog = source,
                LayerSourceLog = "ClientSide",
                ApplicationSourceLog = "BlazorApp",
                MachineNameLog = Environment.MachineName,
                IPAddressLog = "Client Browser",
                UserLog = "JavaScript Client",
                CorrelationKey = Guid.NewGuid().ToString()
            };

            await exceptionLogService.RegisterExceptionLog(exceptionLog);
        }
    }
}

