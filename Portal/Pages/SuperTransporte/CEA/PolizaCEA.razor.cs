using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using portalAdministrativoSISEC.Application.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System.Collections.Generic;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class PolizaCEA
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
		private PolizaDtoCEA _polizaDto = new PolizaDtoCEA();
        private List<SelectSuperTransporteDTO> _select = new List<SelectSuperTransporteDTO>();

        protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("ceas");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
            //applicationShared.IdCentro = 1; // OJO Prueba Eliminar
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_select = await _superTransporteService.GetListaMaestra("Aseguradora", campos);
            _select = await _superTransporteService.GetListaMaestraSuperT("aseguradoras");
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "poliza");
			if (data != null && data.data.attributes.poliza != null) {
				_polizaDto.num_poliza = data.data.attributes.poliza.num_poliza;
				_polizaDto.nombre_aseguradora = data.data.attributes.poliza.nombre_aseguradora;
				_polizaDto.valor_asegurado = data.data.attributes.poliza.valor_asegurado;
				_polizaDto.ha_presentado_reclamacion = data.data.attributes.poliza.ha_presentado_reclamacion;
				_polizaDto.motivo_reclamacion = data.data.attributes.poliza.motivo_reclamacion;
				_polizaDto.setFechaFechaExpedicion(data.data.attributes.poliza.fecha_expedicion);
				_polizaDto.setFechaFechaVencimiento(data.data.attributes.poliza.fecha_vencimiento);
			}
            var populates = new List<string>() {
                "poliza.poliza_vigente",
                "poliza.soporte_pago"
            };
            //Buscar los populates de soporte_pago y poliza_vigente que son los adjuntos de poliza para traer el nombre y url de los adjuntos
            var dataP = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.poliza != null)
            {
                if (dataP.data.attributes.poliza.poliza_vigente.data != null)
                {
                    _polizaDto.url_poliza_adjunto = dataP.data.attributes.poliza.poliza_vigente.data.attributes.url;
                    _polizaDto.nombre_poliza_adjunto = dataP.data.attributes.poliza.poliza_vigente.data.attributes.name;
                }
                if (dataP.data.attributes.poliza.soporte_pago.data != null)
                {
                    _polizaDto.url_soporte_pago_adjunto = dataP.data.attributes.poliza.soporte_pago.data.attributes.url;
                    _polizaDto.nombre_soporte_pago_adjunto = dataP.data.attributes.poliza.soporte_pago.data.attributes.name;
                }
            }
        }

		private async Task GuardarInformacion(EditContext context)
		{
            _loader.Show();
            if (context.Validate())
			{
				int idPoliza_vigente = await _superTransporteService.PostFile(_polizaDto.poliza_vigente);
				if (idPoliza_vigente > 0) {
					int idSoporte_pago = await _superTransporteService.PostFile(_polizaDto.soporte_pago);
					if (idSoporte_pago > 0) {
						var resultado = await _superTransporteService.PutCentro(new PolizaDtoRequestCEA<int,int>()
						{
							poliza = new PolizaClsCEA<int,int>()
							{
								num_poliza = _polizaDto.num_poliza,
								nombre_aseguradora = _polizaDto.nombre_aseguradora,
								valor_asegurado = _polizaDto.valor_asegurado,
								ha_presentado_reclamacion = _polizaDto.ha_presentado_reclamacion,
								motivo_reclamacion = _polizaDto.motivo_reclamacion,
								fecha_expedicion = _polizaDto.getFechaExpedicion(),
								fecha_vencimiento = _polizaDto.getFechaVencimiento(),
								poliza_vigente = idPoliza_vigente,
								soporte_pago = idSoporte_pago
							}
						}, applicationShared.IdCentroStrappi);
						if (resultado != null)
						{
							toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
							Navigation.NavigateTo("/supertransporte/centro-cea", false);
						}
					}
				}
            }
            _loader.Hide();
        }

		private void HandleArchivoPolizaSeleccionado(InputFileChangeEventArgs e)
		{
			_polizaDto.poliza_vigente = e.File;
		}

		private void HandleArchivoPagoPolizaSeleccionado(InputFileChangeEventArgs e)
		{
			_polizaDto.soporte_pago = e.File;
		}
        private async Task DownloadFilePolizaVigente()
        {
            if (_polizaDto.url_poliza_adjunto != null)
            {
                string fileUrl = _polizaDto.url_poliza_adjunto.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
        private async Task DownloadFileSoportePagoPoliza()
        {
            if (_polizaDto.url_soporte_pago_adjunto != null)
            {
                string fileUrl = _polizaDto.url_soporte_pago_adjunto.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}


