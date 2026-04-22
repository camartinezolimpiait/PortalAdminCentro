using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class InterconexionRUNT
    {
		[Inject]
		public NavigationManager Navigation { get; set; }
		[Inject]
		public ISuperTransporteService _superTransporteService { get; set; }
		[Inject]
		private IToastService toastService { get; set; }
		[Inject]
		ProtectedSessionStorage ProtectedSessionStore { get; set; }

        private LoaderEventSubmit _loader = new LoaderEventSubmit();
        private ApplicationShared applicationShared = new ApplicationShared();
		private InterconexionRUNTDto _InterconexionRUNTDto = new InterconexionRUNTDto();
		protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("crcs");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "soporte_interconexion");
			if (data != null && data.data.attributes.interconexion_runt != null) {
               

            }
            var populates = new List<string>() {
                "interconexion_runt.soporte_interconexion"
            };

            var dataP = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.interconexion_runt != null)
            {
                if (dataP.data.attributes.interconexion_runt.soporte_interconexion.data != null)
                {
                    _InterconexionRUNTDto.url_adjunto_soporte_interconexion = dataP.data.attributes.interconexion_runt.soporte_interconexion.data.attributes.url;
                    _InterconexionRUNTDto.nombre_adjunto_soporte_interconexion = dataP.data.attributes.interconexion_runt.soporte_interconexion.data.attributes.name;
                }
            }
        }

		private async Task GuardarInformacion(EditContext context)
		{
            _loader.Show();
            _superTransporteService.SetPlataforma("crcs");
            int idSoporte = await _superTransporteService.PostFile(_InterconexionRUNTDto.soporte_interconexion);
            if (idSoporte > 0)
            {
                var resultado = await _superTransporteService.PutCentro(new InterconexionRUNTDtoRequest<int>()
                {
                    interconexion_runt = new InterconexionRUNTCls<int>()
                    {
                        soporte_interconexion = idSoporte
                    }
                }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro", false);
                }
            }
            _loader.Hide();
        }

        private void HandleArchivoSeleccionado(InputFileChangeEventArgs e)
        {
            _InterconexionRUNTDto.soporte_interconexion = e.File;
        }
        private async Task DownloadFileSoporteInterconexion()
        {
            if (_InterconexionRUNTDto.url_adjunto_soporte_interconexion != null)
            {
                string fileUrl = _InterconexionRUNTDto.url_adjunto_soporte_interconexion.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}
