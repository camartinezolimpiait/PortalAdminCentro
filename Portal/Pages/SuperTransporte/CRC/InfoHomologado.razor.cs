using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using Microsoft.JSInterop;
using System.Collections.Generic;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class InfoHomologado
    {
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public ISuperTransporteService _superTransporteService { get; set; }
        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        private LoaderEventSubmit _loader = new LoaderEventSubmit();
        public int _idVigilido;
        public int _idCentro;
        private InfoHomologadoDto _infoHomologado = new InfoHomologadoDto();
        private ApplicationShared applicationShared = new ApplicationShared();


        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "info_homologado");
            if (data != null && data.data.attributes.info_homologado != null)
            {
                _infoHomologado.num_contrato = data.data.attributes.info_homologado?.num_contrato;
                _infoHomologado.setFecha(data.data.attributes.info_homologado.fecha_contrato);
            }

            var populates = new List<string>() {
                "info_homologado.contrato"
            };
            //Buscar los populates de los adjuntos  para traer el nombre y url de los adjuntos
            var dataP = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.info_homologado != null)
            {
                if (dataP.data.attributes.info_homologado.contrato.data != null)
                {
                    _infoHomologado.url_adjunto_contrato = dataP.data.attributes.info_homologado.contrato.data.attributes.url;
                    _infoHomologado.nombre_adjunto_contrato = dataP.data.attributes.info_homologado.contrato.data.attributes.name;
                }
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (context.Validate())
            {
                int idFile = await _superTransporteService.PostFile(_infoHomologado.contrato);
                if (idFile > 0)
                {
                    var resultado = await _superTransporteService.PutCentro(new InfoHomologadoDtoRequest<int>()
                    {
                        info_homologado = new InfoHomologadoCls<int>()
                        {
                            num_contrato = _infoHomologado.num_contrato,
                            fecha_contrato = _infoHomologado.fecha_contrato(),
                            contrato = idFile
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
            _infoHomologado.contrato = e.File;
        }
        private async Task DownloadFileContrato()
        {
            if (_infoHomologado.url_adjunto_contrato != null)
            {
                string fileUrl = _infoHomologado.url_adjunto_contrato.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}



