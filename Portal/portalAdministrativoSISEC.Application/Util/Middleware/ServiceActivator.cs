using Microsoft.Extensions.DependencyInjection;
using System;

namespace portalAdministrativoSISEC.Util.Middleware
{
    // Clase statica para poder levantar la captura y el guardado en log para javascript
    public static class ServiceActivator
    {
        private static IServiceProvider _serviceProvider;

        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceScope GetScope()
        {
            return _serviceProvider.CreateScope();
        }
    }
}
