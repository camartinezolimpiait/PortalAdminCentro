using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using portalAdministrativoSISEC.Application.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class RepresentanteLegal
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
        private RepresentanteLegalDto _representanteLegalDto = new RepresentanteLegalDto();
        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "vigilado");
            if (data != null && data.data.attributes.vigilado.data != null)
            {
                // Consulta de datos básicos del representante legal
                var nit = data.data.attributes.vigilado.data.attributes.NIT.Trim();
                await ConsultarInformacionRL(nit);

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
            _superTransporteService.SetPlataforma("crcs");
            int idCertificado = await _superTransporteService.PostFile(_representanteLegalDto.certificado_existencia);
            if (idCertificado > 0)
            {
                var resultado = await _superTransporteService.PutCentro(new RepresentanteLegalDtoRequest<int>()
                {
                    representante_legal = new RepresentanteLegalCls<int>()
                    {
                        certificado_existencia = idCertificado
                    }
                }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro", false);
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


