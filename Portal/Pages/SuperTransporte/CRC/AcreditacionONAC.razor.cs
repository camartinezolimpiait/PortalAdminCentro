using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Services.SuperTransporte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class AcreditacionONAC
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
        private ApplicationShared applicationShared = new ApplicationShared();
        
        private string _acreditacionCentro = string.Empty;

        private AttributesVigilado _vigilado = new AttributesVigilado();
        private int _idVigilado = 0;
        private bool _cambiarAcreditacion = false;
        private AcreditacionONACVigiladoDto<IBrowserFile> _acreditacionVigiladoForm = new AcreditacionONACVigiladoDto<IBrowserFile>();
        private AcreditacionONACVigiladoDto<soporte_adjunto> _acreditacionVigiladoAdjunto = new AcreditacionONACVigiladoDto<soporte_adjunto>();
        private AcreditacionONACVigiladoRequest _acreditacionVigiladoRequest = new AcreditacionONACVigiladoRequest();
        private AcreditacionONACVigiladoDto<int?> _acreditacionVigiladoDtoRequest = new AcreditacionONACVigiladoDto<int?>();
        private List<SelectSuperTransporteDTO> _selectNorma = new List<SelectSuperTransporteDTO>();
        private AcreditacionONACDto _acreditacionRequest = new AcreditacionONACDto();
        private string error = "";


        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_selectNorma = await _superTransporteService.GetListaMaestra("Norma", campos);
            _selectNorma = await _superTransporteService.GetListaMaestraSuperT("normasAcreditadas");
            var populates = new List<string>()
            {
                "vigilado.acreditacion_onac.certificado_onac",
                "vigilado.crcs"
            };
            var data = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (data != null) {
                if (data.data.attributes.vigilado?.data?.attributes != null)
                {
                    _vigilado = data.data.attributes.vigilado.data.attributes;
                    _idVigilado = data.data.attributes.vigilado.data.id;
                    if(data.data.attributes.estado_acreditacion_onac != null)
                        _acreditacionCentro = data.data.attributes.estado_acreditacion_onac;
                    if (data.data.attributes.vigilado.data.attributes.acreditacion_onac != null)
                    {
                        _acreditacionVigiladoForm.codigo = data.data.attributes.vigilado.data.attributes.acreditacion_onac.codigo;
                        _acreditacionVigiladoForm.estado = data.data.attributes.vigilado.data.attributes.acreditacion_onac.estado;
                        _acreditacionVigiladoForm.setFecha_publicacion(data.data.attributes.vigilado.data.attributes.acreditacion_onac.fecha_publicacion);
                        _acreditacionVigiladoForm.setFecha_vencimiento(data.data.attributes.vigilado.data.attributes.acreditacion_onac.fecha_vencimiento);
                        _acreditacionVigiladoForm.setFecha_ultima_actualizacion(data.data.attributes.vigilado.data.attributes.acreditacion_onac.fecha_ultima_actualizacion);
                        _acreditacionVigiladoForm.setFecha_renovacion(data.data.attributes.vigilado.data.attributes.acreditacion_onac.fecha_renovacion);
                        _acreditacionVigiladoForm.norma_acreditada = data.data.attributes.vigilado.data.attributes.acreditacion_onac.norma_acreditada;

                        _acreditacionVigiladoForm.url_adjunto_certificado_onac = data.data.attributes.vigilado.data.attributes.acreditacion_onac.certificado_onac.data.attributes.url;
                        _acreditacionVigiladoForm.nombre_adjunto_certificado_onac = data.data.attributes.vigilado.data.attributes.acreditacion_onac.certificado_onac.data.attributes.name;
                    }
                }
                    
            }
        }
        private async Task AddAcreditacionVigilado(EditContext context)
        {
            _loader.Show();
            if (context.Validate() && _idVigilado > 0)
            {
                GetFile certificadoGetFile = new GetFile();
                if (!String.IsNullOrEmpty(_acreditacionVigiladoForm.certificado_onac?.Name))
                {
                    int idCertificado = await _superTransporteService.PostFile(_acreditacionVigiladoForm.certificado_onac);
                    if (idCertificado > 0)
                    {
                        _acreditacionVigiladoDtoRequest.certificado_onac = idCertificado;
                        certificadoGetFile.data = new DataFile() { id = idCertificado };
                        certificadoGetFile.data.attributes = new AttributesFile { name = _acreditacionVigiladoForm.certificado_onac?.Name };
                    }
                    else
                    {
                        toastService.ShowError(@"Ha ocurrido un error al subir el archivo, intente nuevamente", "Información");
                        _loader.Hide();
                        return;
                    }
                }
                _acreditacionVigiladoDtoRequest.codigo = _acreditacionVigiladoForm.codigo;
                _acreditacionVigiladoDtoRequest.estado = _acreditacionVigiladoForm.estado;
                _acreditacionVigiladoDtoRequest.fecha_publicacion = _acreditacionVigiladoForm.getFecha_publicacion();
                _acreditacionVigiladoDtoRequest.fecha_vencimiento = _acreditacionVigiladoForm.getFecha_vencimiento();
                _acreditacionVigiladoDtoRequest.fecha_ultima_actualizacion = _acreditacionVigiladoForm.getFecha_ultima_actualizacion();
                _acreditacionVigiladoDtoRequest.fecha_renovacion = _acreditacionVigiladoForm.getFecha_renovacion();
                _acreditacionVigiladoDtoRequest.norma_acreditada = _acreditacionVigiladoForm.norma_acreditada;

                _acreditacionVigiladoRequest.acreditacion_onac = _acreditacionVigiladoDtoRequest;

                var resultado = await _superTransporteService.PutVigilado(_acreditacionVigiladoRequest, _idVigilado);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la información correctamente.", "Información");
                    _vigilado.acreditacion_onac = new AcreditacionONACVigiladoDto<GetFile>()
                    {
                        codigo = _acreditacionVigiladoForm.codigo,
                        estado = _acreditacionVigiladoForm.estado,
                        fecha_publicacion = _acreditacionVigiladoForm.getFecha_publicacion(),
                        fecha_vencimiento = _acreditacionVigiladoForm.getFecha_vencimiento(),
                        fecha_ultima_actualizacion = _acreditacionVigiladoForm.getFecha_ultima_actualizacion(),
                        fecha_renovacion = _acreditacionVigiladoForm.getFecha_renovacion(),
                        norma_acreditada = _acreditacionVigiladoForm.norma_acreditada,
                        certificado_onac = certificadoGetFile
                    };
                    _acreditacionVigiladoForm = new AcreditacionONACVigiladoDto<IBrowserFile>();
                }
            }
            _loader.Hide();
        }
        private async Task AddAcreditacionCentro(EditContext context)
        {
            string pattern = @"^[a-zA-Z0-9ñÑáàâãéèêíïóôõöúüç\s-]*$";
            _loader.Show();
            if (!string.IsNullOrEmpty(_acreditacionCentro))
            {
                if (!Regex.IsMatch(_acreditacionCentro, pattern, RegexOptions.IgnorePatternWhitespace))
                {
                    error = "Por favor, no introduzca caracteres especiales";
                    _loader.Hide();
                    return;
                }
                _acreditacionRequest.estado_acreditacion_onac = _acreditacionCentro;
                var resultado = await _superTransporteService.PutCentro(_acreditacionRequest, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro", false);
                }
            }
            else
            {
                error = "El campo no puede estar vacío";
                _loader.Hide();
                return;
            }
            _loader.Hide();
        }

        private void HandleCertificadoONAC(InputFileChangeEventArgs e)
        {
            _acreditacionVigiladoForm.certificado_onac = e.File;
        }
        private async Task DownloadFileCertificadoOnac()
        {
            if (_acreditacionVigiladoForm.url_adjunto_certificado_onac != null)
            {
                string fileUrl = _acreditacionVigiladoForm.url_adjunto_certificado_onac.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}
