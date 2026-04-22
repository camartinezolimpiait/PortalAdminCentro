using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.ProcesoDePago
{
    public partial class ProceedToPayment
    {
        private bool isLoading = false;
        [Parameter]
        public string nut { get; set; }
        [Parameter]
        public EventCallback copiaNut { get; set; }

        private string? mensajeCopiado;

        [Inject] IJSRuntime JS { get; set; } = default!;
        private bool abrirModal;

        private async Task CopiarNUT()
        {
            await JS.InvokeVoidAsync("copiarAlPortapapeles", nut);
            mensajeCopiado = "¡Copiado al portapapeles!";

            StateHasChanged();
        }

        private async Task IrPasarelaPago()
        {
            ShowDialog();
            await Task.Delay(2000);
            await copiaNut.InvokeAsync();
            CloseDialog();
        }

        /// <summary>
        /// Muestra el modal
        /// </summary>
        /// <returns></returns>
        public void ShowDialog()
        {
            abrirModal = true;
        }

        /// <summary>
        /// Oculta el modal
        /// </summary>
        /// <returns></returns>
        public void CloseDialog()
        {
            abrirModal = false;
        }
    }
}
