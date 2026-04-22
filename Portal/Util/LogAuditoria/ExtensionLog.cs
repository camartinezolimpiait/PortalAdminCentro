using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util.LogAuditoria
{
    public static class ExtensionLog
    {

        public static ExceptionLog GenerateExceptionLog(Exception ex, string methodName, string componentName, string userLog,string transactionLogId, string correlationKey)
        {
            var errorLog = new ExceptionLog
            {
                MessageLog = ex.Message??"No se encontro el mensaje de la exception." ,
                ExceptionType = ex.GetType().ToString(),
                TimestampLog = DateTime.Now,
                StackTraceLog = ex.ToString(),
                TransactionIdLog = transactionLogId ?? "", // Mantener la misma transacción si existe
                SorceLog = methodName,//"RegisterExceptionLog",
                LayerSourceLog = componentName,
                ApplicationSourceLog = "Portal Administrativo", // Ajusta según tu aplicación
                MachineNameLog = Environment.MachineName,
                IPAddressLog = GetLocalIPAddress(), // Necesitarás implementar este método
                UserLog = !string.IsNullOrEmpty(userLog)? userLog:"system", // Mantener el mismo usuario o usar "system"
                CorrelationKey = correlationKey??"" // Mantener la misma correlación
            };
            return errorLog;

        }

        // Método auxiliar para obtener la dirección IP local
        private static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}
