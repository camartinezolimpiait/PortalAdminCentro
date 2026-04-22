using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System.Collections.Generic;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class ResolucionHabilitacionCEA
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
		private ResolucionHabilitacionDtoCEA _resolucionHabilitacion = new ResolucionHabilitacionDtoCEA();
		protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("ceas");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
            
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "resolucion_de_habilitacion");
			if (data != null && data.data.attributes.resolucion_de_habilitacion != null) {
				_resolucionHabilitacion.num_resolucion = data.data.attributes.resolucion_de_habilitacion?.num_resolucion;
				_resolucionHabilitacion.tiene_resolucion = data.data.attributes.resolucion_de_habilitacion.tiene_resolucion;
				_resolucionHabilitacion.setFecha(data.data.attributes.resolucion_de_habilitacion.fecha_resolucion);
            }
            var populates = new List<string>() {
                "resolucion_de_habilitacion.resolucion_de_habilitacion"
            };

            var dataP = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.resolucion_de_habilitacion != null)
            {
                if (dataP.data.attributes.resolucion_de_habilitacion.resolucion_de_habilitacion.data != null)
                {
                    _resolucionHabilitacion.url_adjunto_resolucion_habiltacion = dataP.data.attributes.resolucion_de_habilitacion.resolucion_de_habilitacion.data.attributes.url;
                    _resolucionHabilitacion.nombre_adjunto_resolucion_habiltacion = dataP.data.attributes.resolucion_de_habilitacion.resolucion_de_habilitacion.data.attributes.name;
                }
            }
        }

		private async Task GuardarInformacion(EditContext context)
		{
            _loader.Show();
            if (!_resolucionHabilitacion.tiene_resolucion)
            {
                var resultado = await _superTransporteService.PutCentro(new ResolucionHabilitacionDtoRequestFalseCEA()
                {
                    resolucion_de_habilitacion = new ResolucionHabilitacionClsFalseCEA()
                    {
                        tiene_resolucion = false,
                        num_resolucion = null,
                        fecha_resolucion = null
                    }
                }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            int idResolucion = await _superTransporteService.PostFile(_resolucionHabilitacion.resolucion_de_habilitacion);
            if (idResolucion > 0)
            {
                var resultado = await _superTransporteService.PutCentro(new ResolucionHabilitacionDtoRequestCEA<int>()
                {
                    resolucion_de_habilitacion = new ResolucionHabilitacionClsCEA<int>()
                    {
                        tiene_resolucion = _resolucionHabilitacion.tiene_resolucion,
                        num_resolucion = _resolucionHabilitacion.num_resolucion,
                        fecha_resolucion = _resolucionHabilitacion.getFecha(),
                        resolucion_de_habilitacion = idResolucion
                    }
                }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            _loader.Hide();
        }

        private void HandleArchivoSeleccionado(InputFileChangeEventArgs e)
        {
            _resolucionHabilitacion.resolucion_de_habilitacion = e.File;
        }
        private async Task DownloadFileResolucionHabilitacion()
        {
            if (_resolucionHabilitacion.url_adjunto_resolucion_habiltacion != null)
            {
                string fileUrl = _resolucionHabilitacion.url_adjunto_resolucion_habiltacion.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }


        }
    }
}
