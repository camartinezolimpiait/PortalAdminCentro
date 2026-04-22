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
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class LicenciaFuncionamientoCEA
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
		private LicenciaFuncionamientoDtoCEA _licenciaFuncionamientoDto = new LicenciaFuncionamientoDtoCEA();
        private List<SelectSuperTransporteDTO> _select = new List<SelectSuperTransporteDTO>();

        protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("ceas");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
            //applicationShared.IdCentro = 1; // OJO Prueba Eliminar
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_select = await _superTransporteService.GetListaMaestra("SecretariasSalud", campos);
            _select = await _superTransporteService.GetListaMaestraSuperT("secretariasdeEducaciones");
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "licencia_funcionamiento");
			if (data != null && data.data.attributes.licencia_funcionamiento != null) {
				_licenciaFuncionamientoDto.acto_administrativo = data.data.attributes.licencia_funcionamiento.acto_administrativo;
				_licenciaFuncionamientoDto.secretaria_edu_expedicion = data.data.attributes.licencia_funcionamiento.secretaria_edu_expedicion;
				_licenciaFuncionamientoDto.num_radicado_actualizacion = data.data.attributes.licencia_funcionamiento.num_radicado_actualizacion;
				_licenciaFuncionamientoDto.secretaria_edu_actualizacion = data.data.attributes.licencia_funcionamiento.secretaria_edu_actualizacion;
				_licenciaFuncionamientoDto.setFechaActoExpedicion(data.data.attributes.licencia_funcionamiento.fecha_acto_expedicion);
                _licenciaFuncionamientoDto.setFechaRadicado(data.data.attributes.licencia_funcionamiento.fecha_radicado);
			}
            var populates = new List<string>() {
                "licencia_funcionamiento.acto_administrativo_doc"
            };
            //Buscar los populates de los adjuntos  para traer el nombre y url de los adjuntos
            var dataP = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.licencia_funcionamiento != null)
            {
                if (dataP.data.attributes.licencia_funcionamiento.acto_administrativo_doc.data != null)
                {
                    _licenciaFuncionamientoDto.url_adjunto_acto_administrativo= dataP.data.attributes.licencia_funcionamiento.acto_administrativo_doc.data.attributes.url;
                    _licenciaFuncionamientoDto.nombre_adjunto_acto_administrativo = dataP.data.attributes.licencia_funcionamiento.acto_administrativo_doc.data.attributes.name;
                }
            }
        }

		private async Task GuardarInformacion(EditContext context)
		{
            _loader.Show();
            if (context.Validate())
			{
				int idActoAdministrativoDoc = await _superTransporteService.PostFile(_licenciaFuncionamientoDto.acto_administrativo_doc);
				if (idActoAdministrativoDoc > 0) {
					var resultado = await _superTransporteService.PutCentro(new LicenciaFuncionamientoDtoRequestCEA<int>()
					{
						licencia_funcionamiento = new LicenciaFuncionamientoClsCEA<int>()
						{
                            acto_administrativo = _licenciaFuncionamientoDto.acto_administrativo,
                            fecha_acto_expedicion = _licenciaFuncionamientoDto.getFechaActoExpedicion(),
                            secretaria_edu_expedicion = _licenciaFuncionamientoDto.secretaria_edu_expedicion,
                            num_radicado_actualizacion = _licenciaFuncionamientoDto.num_radicado_actualizacion,
                            fecha_radicado = _licenciaFuncionamientoDto.getFechaRadicado(),
                            secretaria_edu_actualizacion = _licenciaFuncionamientoDto.secretaria_edu_actualizacion,
                            acto_administrativo_doc = idActoAdministrativoDoc
                        }
					}, applicationShared.IdCentroStrappi);
					if (resultado != null)
					{
						toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
						Navigation.NavigateTo("/supertransporte/centro-cea", false);
					}
				}
            }
            _loader.Hide();
        }

		private void HandleArchivoActoAdministrativoDoc(InputFileChangeEventArgs e)
		{
			_licenciaFuncionamientoDto.acto_administrativo_doc = e.File;
		}
        private async Task DownloadFileActoAdministrativo()
        {
            if (_licenciaFuncionamientoDto.url_adjunto_acto_administrativo != null)
            {
                string fileUrl = _licenciaFuncionamientoDto.url_adjunto_acto_administrativo.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}
