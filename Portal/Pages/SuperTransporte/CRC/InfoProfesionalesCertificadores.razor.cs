using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using System.Collections.Generic;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using System.Linq;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class InfoProfesionalesCertificadores
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
		private InfoProfesionalesCertificadoresDto _infoProfesionalesCertificadoresDto = new InfoProfesionalesCertificadoresDto();
		private lista_profesionales _profesionales = new lista_profesionales();
		private List<lista_profesionales> _lista_profesionales = new List<lista_profesionales>();
        private List<SelectSuperTransporteDTO> _selectProfesiones = new List<SelectSuperTransporteDTO>();
        protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("crcs");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
            //TODO:Se debe consultar las lista de profesiones
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_selectProfesiones = await _superTransporteService.GetListaMaestra("InfoProfesionalesCertificadores", campos);
            _selectProfesiones = await _superTransporteService.GetListaMaestraSuperT("profesionales");
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "profesionales_certificadores.lista_profesionales");
			if (data != null && data.data.attributes.profesionales_certificadores != null) {
                _infoProfesionalesCertificadoresDto.cantidad_certificadores = data.data.attributes.profesionales_certificadores.cantidad_certificadores;
				if(data.data.attributes.profesionales_certificadores.lista_profesionales != null)
					_lista_profesionales = data.data.attributes.profesionales_certificadores.lista_profesionales;
            }
            var populates = new List<string>() {
                "profesionales_certificadores.acredita_cargue",
                "profesionales_certificadores.registro_runt"
            };
            
            var dataP = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.profesionales_certificadores != null)
            {
                if (dataP.data.attributes.profesionales_certificadores.acredita_cargue.data != null)
                {
                    _infoProfesionalesCertificadoresDto.url_adjunto_acredita_cargue = dataP.data.attributes.profesionales_certificadores.acredita_cargue.data.attributes.url;
                    _infoProfesionalesCertificadoresDto.nombre_adjunto_acredita_cargue = dataP.data.attributes.profesionales_certificadores.acredita_cargue.data.attributes.name;
                }
                if (dataP.data.attributes.profesionales_certificadores.registro_runt.data != null)
                {
                    _infoProfesionalesCertificadoresDto.url_adjunto_registro_runt = dataP.data.attributes.profesionales_certificadores.registro_runt.data.attributes.url;
                    _infoProfesionalesCertificadoresDto.nombre_adjunto_registro_runt = dataP.data.attributes.profesionales_certificadores.registro_runt.data.attributes.name;
                }
            }
        }

		private async Task GuardarInformacion(EditContext context)
		{
            _loader.Show();
            if (context.Validate())
			{
                int idAcredita_cargue = await _superTransporteService.PostFile(_infoProfesionalesCertificadoresDto.acredita_cargue);
                if (idAcredita_cargue > 0)
                {
                    int idRegistro_runt = await _superTransporteService.PostFile(_infoProfesionalesCertificadoresDto.registro_runt);
                    if (idRegistro_runt > 0)
                    {
                        var resultado = await _superTransporteService.PutCentro(new InfoProfesionalesCertificadoresDtoRequest<int,int>()
                        {
                            profesionales_certificadores = new InfoProfesionalesCertificadoresCls<int,int>() { 
								cantidad_certificadores = _lista_profesionales.Select(s => s.cantidad).Sum(),
								lista_profesionales = _lista_profesionales,
								acredita_cargue = idAcredita_cargue,
								registro_runt = idRegistro_runt
                            }
                        }, applicationShared.IdCentroStrappi);
                        if (resultado != null)
                        {
                            toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                            Navigation.NavigateTo("/supertransporte/centro", false);
                        }
                    }
                }
            }
            _loader.Hide();
        }
		private async Task addProfesional(EditContext context) 
		{
			if (context.Validate()) {
                var existe = _lista_profesionales.Where(w => w.profesion == _profesionales.profesion);
                if (!existe.Any())
                {
                    _lista_profesionales.Add(_profesionales);
                    _profesionales = new lista_profesionales();
                }
                else {
                    toastService.ShowError(@"La profesión seleccionada ya se encuentra registrada. Para continuar quitar la registrada.", "Información");
                }
				
			}
		}
        void ButtonOnClickQuitar(lista_profesionales profesional)
		{
			_lista_profesionales.Remove(profesional);
        }
        

        private void HandleArchivoAcreditaCargueSeleccionado(InputFileChangeEventArgs e)
		{
            _infoProfesionalesCertificadoresDto.acredita_cargue = e.File;
		}

        private void HandleArchivoRegistroRuntSeleccionado(InputFileChangeEventArgs e)
        {
            _infoProfesionalesCertificadoresDto.registro_runt = e.File;
        }
        private async Task DownloadFileAcreditaCargue()
        {
            if (_infoProfesionalesCertificadoresDto.url_adjunto_acredita_cargue != null)
            {
                string fileUrl = _infoProfesionalesCertificadoresDto.url_adjunto_acredita_cargue.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
        private async Task DownloadFileRegistroRunt()
        {
            if (_infoProfesionalesCertificadoresDto.url_adjunto_registro_runt != null)
            {
                string fileUrl = _infoProfesionalesCertificadoresDto.url_adjunto_registro_runt.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}
