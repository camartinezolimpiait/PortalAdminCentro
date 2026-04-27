using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using portalAdministrativoSISEC.Application.Data;
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
    public partial class InfoProfesionalesSalud
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
		private InfoProfesionalesSaludDto _infoProfesionalesSaludDto = new InfoProfesionalesSaludDto();
		private lista_profesionalesSalud _profesionales = new lista_profesionalesSalud();
		private List<lista_profesionalesSalud> _lista_profesionales = new List<lista_profesionalesSalud>();
        private List<SelectSuperTransporteDTO> _selectProfesiones = new List<SelectSuperTransporteDTO>();
        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            //TODO:Se debe consultar las lista de profesiones
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_selectProfesiones = await _superTransporteService.GetListaMaestra("InfoProfesionalesSalud", campos);
            _selectProfesiones = await _superTransporteService.GetListaMaestraSuperT("profesionales");
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "prof_salud.lista_profesionales");
			if (data != null && data.data.attributes.prof_salud != null) {
                _infoProfesionalesSaludDto.cantidad_prof_salud = data.data.attributes.prof_salud.cantidad_prof_salud;
				if(data.data.attributes.prof_salud.lista_profesionales != null)
					_lista_profesionales = data.data.attributes.prof_salud.lista_profesionales;
            }
            var populates = new List<string>() {
                "prof_salud.acredita_cargue",
                "prof_salud.registro_runt"
            };

            var dataP = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.prof_salud != null)
            {
                if (dataP.data.attributes.prof_salud.acredita_cargue.data != null)
                {
                    _infoProfesionalesSaludDto.url_adjunto_acredita_cargue = dataP.data.attributes.prof_salud.acredita_cargue.data.attributes.url;
                    _infoProfesionalesSaludDto.nombre_adjunto_acredita_cargue = dataP.data.attributes.prof_salud.acredita_cargue.data.attributes.name;
                }
                if (dataP.data.attributes.prof_salud.registro_runt.data != null)
                {
                    _infoProfesionalesSaludDto.url_adjunto_registro_runt = dataP.data.attributes.prof_salud.registro_runt.data.attributes.url;
                    _infoProfesionalesSaludDto.nombre_adjunto_registro_runt = dataP.data.attributes.prof_salud.registro_runt.data.attributes.name;
                }
            }
        }

		private async Task GuardarInformacion(EditContext context)
		{
            _loader.Show();
            if (context.Validate())
			{
                int idAcredita_cargue = await _superTransporteService.PostFile(_infoProfesionalesSaludDto.acredita_cargue);
                if (idAcredita_cargue > 0)
                {
                    int idRegistro_runt = await _superTransporteService.PostFile(_infoProfesionalesSaludDto.registro_runt);
                    if (idRegistro_runt > 0)
                    {
                        var resultado = await _superTransporteService.PutCentro(new InfoProfesionalesSaludDtoRequest<int,int>()
                        {
                            prof_salud = new InfoProfesionalesSaludCls<int,int>() {
                                cantidad_prof_salud = _lista_profesionales.Select(s => s.cantidad).Sum(),
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
                    _profesionales = new lista_profesionalesSalud();
                }
                else {
                    toastService.ShowError(@"La profesión seleccionada ya se encuentra registrada. Para continuar quitar la registrada.", "Información");
                }
				
			}
		}
        void ButtonOnClickQuitar(lista_profesionalesSalud profesional)
		{
			_lista_profesionales.Remove(profesional);
        }
        

        private void HandleArchivoAcreditaCargueSeleccionado(InputFileChangeEventArgs e)
		{
            _infoProfesionalesSaludDto.acredita_cargue = e.File;
		}

        private void HandleArchivoRegistroRuntSeleccionado(InputFileChangeEventArgs e)
        {
            _infoProfesionalesSaludDto.registro_runt = e.File;
        }
        private async Task DownloadFileAcreditaCargue()
        {
            if (_infoProfesionalesSaludDto.url_adjunto_acredita_cargue != null)
            {
                string fileUrl = _infoProfesionalesSaludDto.url_adjunto_acredita_cargue.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
        private async Task DownloadFileRegistroRunt()
        {
            if (_infoProfesionalesSaludDto.url_adjunto_registro_runt != null)
            {
                string fileUrl = _infoProfesionalesSaludDto.url_adjunto_registro_runt.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}


