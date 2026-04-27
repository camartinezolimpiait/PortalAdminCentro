using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using portalAdministrativoSISEC.Application.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class ResolucionHabilitacion
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
		private ResolucionHabilitacionDto _resolucionHabilitacion = new ResolucionHabilitacionDto();
		protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("crcs");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
			var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "resolucion_de_habilitacion");
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
            if (context.Validate())
			{
				if (!_resolucionHabilitacion.tiene_resolucion)
				{
                    var resultado = await _superTransporteService.PutCentro(new ResolucionHabilitacionDtoRequestFalse()
                    {
                        resolucion_de_habilitacion = new ResolucionHabilitacionClsFalse()
                        {
                            tiene_resolucion = false,
                            num_resolucion = null,
                            fecha_resolucion = null
                        }
                    }, applicationShared.IdCentroStrappi);
                    if (resultado != null)
                    {
                        toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                        Navigation.NavigateTo("/supertransporte/centro", false);
                    }
                }
				int idResolucion = await _superTransporteService.PostFile(_resolucionHabilitacion.resolucion_de_habilitacion);
				if (idResolucion > 0) {
					var resultado = await _superTransporteService.PutCentro(new ResolucionHabilitacionDtoRequest<int>()
					{
						resolucion_de_habilitacion = new ResolucionHabilitacionCls<int>()
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
						Navigation.NavigateTo("/supertransporte/centro", false);
					}
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


