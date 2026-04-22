using Microsoft.AspNetCore.Components;

namespace portalAdministrativoSISEC.Util.Extension
{
    public static class RenderExtension
    {
        // Inicio código generado por GitHub Copilot

        #region Private Methods

        public static RenderFragment StatusIndicator(this string status) => builder =>
        {
            if (status == "error")
            {
                builder.AddMarkupContent(0, """
                    <div class="inline-grid *:[grid-area:1/1]">
                        <div class="status status-error animate-ping"></div>
                        <div class="status status-error"></div>
                    </div>
                    <span class="text-rojo-700 ms-1 border-e border-gray-400 pe-3 font-bold"> Error </span>
                    """);
            }
            else if (status == "apagado")
            {
                builder.AddMarkupContent(1, """
                    <div class="status status-neutral"></div>
                    <span class="ms-1 border-e border-gray-400 pe-3 font-bold text-gray-700"> Apagado </span>
                    """);
            }
            else if (status == "en línea")
            {
                builder.AddMarkupContent(2, """
                    <div class="status status-success"></div>
                    <span class="text-verde-700 ms-1 border-e border-gray-400 pe-3 font-bold"> En línea </span>
                    """);
            }
            else if (status == "sin conexión")
            {
                builder.AddMarkupContent(3, """
                    <div class="status status-warning"></div>
                    <span class="text-naranja-500 ms-1 border-e border-gray-400 pe-3 font-bold">
                        Sin conexión
                    </span>
                    """);
            }
        };

        #endregion Private Methods

        // Fin código generado por GitHub Copilot
    }
}