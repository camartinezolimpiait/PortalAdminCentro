using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System.Reactive;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class RepresentanteLegalCEA
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
        private RepresentanteLegalDtoCEA _representanteLegalDto = new RepresentanteLegalDtoCEA();
        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            //applicationShared.IdCentro = 1; // OJO Prueba.. borrar
            //var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "representante_legal");
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "vigilado");
            if (data != null && data.data.attributes.vigilado.data != null)
            {
                // Consulta de datos básicos del representante legal
                var nit = data.data.attributes.vigilado.data.attributes.NIT.Trim();
                await ConsultarInformacionRL(nit);

                if (string.IsNullOrEmpty(_representanteLegalDto.num_doc))
                {
                    toastService.ShowSuccess(@$"No se ha encontrado información de Representante Legal con el NIT {nit}, por favor corrobore con el sistema VIGIA", "Información");
                }

            }
            var populates = new List<string>() {
                "representante_legal.certificado_existencia"
            };
            //Buscar los populates de soporte_pago y poliza_vigente que son los adjuntos de poliza para traer el nombre y url de los adjuntos
            var dataP = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.representante_legal != null)
            {
                if (dataP.data.attributes.representante_legal.certificado_existencia.data != null)
                {
                    _representanteLegalDto.url_adjunto_certificado_existencia = dataP.data.attributes.representante_legal.certificado_existencia.data.attributes.url;
                    _representanteLegalDto.nombre_adjunto_certificado_existencia = dataP.data.attributes.representante_legal.certificado_existencia.data.attributes.name;
                }
            }
        }

        private async Task ConsultarInformacionRL(string nit)
        {
            for (int i = 0; i < 2; i++)
            {
                var dataVigilado = await _superTransporteService.GetListaMaestraSuperVigilados(nit);
                if (dataVigilado != null && dataVigilado.nit != null)
                {
                    _representanteLegalDto.tipo_doc = dataVigilado.nombre_Documento1;
                    _representanteLegalDto.num_doc = dataVigilado.nro_Documento;
                    _representanteLegalDto.nombre = dataVigilado.nombres_Apellidos;
                    _representanteLegalDto.email = dataVigilado?.email;
                    _representanteLegalDto.direccion = dataVigilado.dir_Establecimiento;
                    _representanteLegalDto.telefono = dataVigilado.telefono_Rep;                    
                    return;
                }
                var nitOriginal = nit;
                nit = nitOriginal.Substring(0, nitOriginal.Length - 1);
            }
        }

        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            int idCertificado = await _superTransporteService.PostFile(_representanteLegalDto.certificado_existencia);
            if (idCertificado > 0)
            {
                var resultado = await _superTransporteService.PutCentro(new RepresentanteLegalDtoRequestCEA<int>()
                {
                    representante_legal = new RepresentanteLegalClsCEA<int>()
                    {
                        certificado_existencia = idCertificado
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
            _representanteLegalDto.certificado_existencia = e.File;
        }
        private async Task DownloadFileCertificado()
        {
            if (_representanteLegalDto.url_adjunto_certificado_existencia != null)
            {
                string fileUrl = _representanteLegalDto.url_adjunto_certificado_existencia.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }
        }
    }
}
