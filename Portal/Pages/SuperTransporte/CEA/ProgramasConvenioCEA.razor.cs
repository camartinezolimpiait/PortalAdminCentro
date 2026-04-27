using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using BlazorInputFile;
using System.Net.Http.Headers;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System.Collections.Generic;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class ProgramasConvenioCEA
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
        public int _idVigilido;
        public int _idCentro;
        private ProgramaConvenioDtoCEA _programaConvenio = new ProgramaConvenioDtoCEA();
        private ApplicationShared applicationShared = new ApplicationShared();
        private List<SelectSuperTransporteDTO> _selectNit = new List<SelectSuperTransporteDTO>();
        private List<SelectSuperTransporteDTO> _selectSecretaria = new List<SelectSuperTransporteDTO>();

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            //applicationShared.IdCentro = 1;
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            _selectNit = await _superTransporteService.GetListaMaestra("NitCEA", campos);
            //_selectSecretaria = await _superTransporteService.GetListaMaestra("SecretariasSalud", campos);
            _selectSecretaria = await _superTransporteService.GetListaMaestraSuperT("secretariasdeEducaciones");
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "convenios");
            if (data != null && data.data.attributes.convenios != null)
            {
                _programaConvenio.ofrece = (bool)(data.data.attributes.convenios?.ofrece);
                _programaConvenio.nit_cea_convenio = (long)(data.data.attributes.convenios?.nit_cea_convenio);
                _programaConvenio.secretaria = data.data.attributes.convenios?.secretaria;
            }
            var populates = new List<string>() {
                "convenios.registro"
            };
            //Buscar los populates de los adjuntos  para traer el nombre y url de los adjuntos
            var dataP = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.convenios != null)
            {
                if (dataP.data.attributes.convenios.registro.data != null)
                {
                    _programaConvenio.url_adjunto_registro = dataP.data.attributes.convenios.registro.data.attributes.url;
                    _programaConvenio.nombre_adjunto_registro = dataP.data.attributes.convenios.registro.data.attributes.name;
                }
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (!_programaConvenio.ofrece)
            {
                var resultado = await _superTransporteService.PutCentro(new ProgramaConvenioDtoRequestCEAFalse()
                {
                    convenios = new ProgramaConvenioClsCEAFalse()
                    {
                        ofrece = false,
                        nit_cea_convenio = 0,
                        secretaria = null
                    }
                }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            
            
            if (context.Validate())
            {
                int idFile = await _superTransporteService.PostFile(_programaConvenio.registro);
                if (idFile > 0)
                {
                    var resultado = await _superTransporteService.PutCentro(new ProgramaConvenioDtoRequestCEA<int>()
                    {
                        convenios = new ProgramaConvenioClsCEA<int>()
                        {
                            ofrece = _programaConvenio.ofrece,
                            nit_cea_convenio = _programaConvenio.nit_cea_convenio,
                            secretaria = _programaConvenio.secretaria,
                            registro = idFile
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
        private void HandleArchivoSeleccionado(InputFileChangeEventArgs e)
        {
            _programaConvenio.registro = e.File;
        }
        private async Task DownloadFileRegistro()
        {
            if (_programaConvenio.url_adjunto_registro != null)
            {
                string fileUrl = _programaConvenio.url_adjunto_registro.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}


