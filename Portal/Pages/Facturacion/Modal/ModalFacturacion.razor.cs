using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Modal
{
    public partial class ModalFacturacion
    {
        // Inicio código generado por GitHub Copilot

        #region Properties

        [Parameter]
        public int Total { get; set; }

        [Parameter]
        public int TotalExitosos { get; set; }

        [Parameter]
        public int MostrarModal { get; set; }

        [Parameter]
        public EventCallback OnAceptar { get; set; }

        [Parameter]
        public EventCallback OnAceptarAnulacion { get; set; }

        [Parameter]
        public EventCallback OnCancelar { get; set; }

        [Parameter]
        public EventCallback OnCerrar { get; set; }

        #endregion Properties

        #region Private Methods

        private async Task Aceptar()
        {
            await OnAceptar.InvokeAsync();
        }

        private async Task AceptarAnulacion()
        {
            await OnAceptarAnulacion.InvokeAsync();
        }

        private async Task Cancelar()
        {
            await OnCancelar.InvokeAsync();
        }

        private async Task Cerrar()
        {
            await OnCerrar.InvokeAsync();
        }

        #endregion Private Methods

        // Fin código generado por GitHub Copilot
    }
}