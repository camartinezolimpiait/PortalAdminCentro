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
    public partial class CertificacionOECCEA
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
        private CertificacionOECDtoCEA _certificacionOEC = new CertificacionOECDtoCEA();
        private ApplicationShared applicationShared = new ApplicationShared();

        private List<SelectSuperTransporteDTO> _selectEmpresa = new List<SelectSuperTransporteDTO>();
        private List<SelectSuperTransporteDTO> _selectEsquema = new List<SelectSuperTransporteDTO>();


        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;

            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_selectEmpresa = await _superTransporteService.GetListaMaestra("Empresa", campos);
            //_selectEsquema = await _superTransporteService.GetListaMaestra("Esquema", campos);
            _selectEmpresa = await _superTransporteService.GetListaMaestraSuperT("empresasExpidenCertificaciones");
            _selectEsquema = await _superTransporteService.GetListaMaestraSuperT("normasAcreditadas");
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "certificacion_oec");
            if (data != null && data.data.attributes.certificacion_oec != null)
            {
                _certificacionOEC.num_certificado = data.data.attributes.certificacion_oec?.num_certificado;
                _certificacionOEC.estado_acreditacion = data.data.attributes.certificacion_oec?.estado_acreditacion;
                _certificacionOEC.setFecha_vigilancia(data.data.attributes.certificacion_oec.fecha_vigilancia);
                _certificacionOEC.setFecha_seguimiento(data.data.attributes.certificacion_oec.fecha_seguimiento);
                _certificacionOEC.setFecha_vencimiento(data.data.attributes.certificacion_oec.fecha_vencimiento);
                _certificacionOEC.setFecha_renovacion(data.data.attributes.certificacion_oec.fecha_renovacion);
                _certificacionOEC.empresa_que_expide = data.data.attributes.certificacion_oec?.empresa_que_expide;
                _certificacionOEC.esquema = data.data.attributes.certificacion_oec?.esquema;
            }
            var populates = new List<string>() {
                "certificacion_oec.certificado_acreditacion"
            };
            //Buscar los populates de los adjuntos  para traer el nombre y url de los adjuntos
            var dataP = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.certificacion_oec != null)
            {
                if (dataP.data.attributes.certificacion_oec.certificado_acreditacion.data != null)
                {
                    _certificacionOEC.url_adjunto_certificado_acreditacion = dataP.data.attributes.certificacion_oec.certificado_acreditacion.data.attributes.url;
                    _certificacionOEC.nombre_adjunto_certificado_acreditacion = dataP.data.attributes.certificacion_oec.certificado_acreditacion.data.attributes.name;
                }
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (context.Validate())
            {
                int idFile = await _superTransporteService.PostFile(_certificacionOEC.certificado_acreditacion);
                if (idFile > 0)
                {
                    var resultado = await _superTransporteService.PutCentro(new CertificacionOECDtoRequestCEA<int>()
                    {
                        certificacion_oec = new CertificacionOECClsCEA<int>()
                        {
                            num_certificado = _certificacionOEC.num_certificado,
                            estado_acreditacion = _certificacionOEC.estado_acreditacion,
                            fecha_vigilancia = _certificacionOEC.getFecha_vigilancia(),
                            fecha_seguimiento = _certificacionOEC.getFecha_seguimiento(),
                            fecha_vencimiento = _certificacionOEC.getFecha_vencimiento(),
                            fecha_renovacion = _certificacionOEC.getFecha_renovacion(),
                            empresa_que_expide = _certificacionOEC.empresa_que_expide,
                            esquema = _certificacionOEC.esquema,
                            certificado_acreditacion = idFile
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
            _certificacionOEC.certificado_acreditacion = e.File;
        }
        private async Task DownloadFileCertificado()
        {
            if (_certificacionOEC.url_adjunto_certificado_acreditacion != null)
            {
                string fileUrl = _certificacionOEC.url_adjunto_certificado_acreditacion.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}


