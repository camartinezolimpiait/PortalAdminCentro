using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data.CompraPin.CDA;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.ResumenCompraCDA
{
    public partial class ResumenCompraCDA
    {

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Parameter]
        public PagoPinCDA PagoPinCda { get; set; }

        private void Salir()
        {
            Navigation.NavigateTo("/configuracion/PerfilMilicencia");
        }

        private void ComprarOtroPin()
        {
            //Navigation.NavigateTo("/compradepin");
            var rutaActual = Navigation.Uri;

            // Recargar la ruta actual
            Navigation.NavigateTo(rutaActual, forceLoad: true);
        }

    }
}

