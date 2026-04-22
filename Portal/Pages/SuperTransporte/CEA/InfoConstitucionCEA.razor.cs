using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using portalAdministrativoSISEC.Services.SuperTransporte;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class InfoConstitucionCEA
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
        private InfoConstitucionDtoCEA _constitucionDto = new InfoConstitucionDtoCEA();
        private ApplicationShared applicationShared = new ApplicationShared();
        private List<SelectSuperTransporteDTO> _select = new List<SelectSuperTransporteDTO>();

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            //applicationShared.IdCentro = 1; // Quitar.. prueba
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_select = await _superTransporteService.GetListaMaestra("EstadoMatriculaMercantil", campos);
            _select = await _superTransporteService.GetListaMaestraSuperT("matriculaMercantilEstados");
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "constitucion");
            if (data != null && data.data.attributes.constitucion != null)
            {
                _constitucionDto.num_matricula_mercantil = data.data.attributes.constitucion?.num_matricula_mercantil;
                _constitucionDto.acto_de_creacion = data.data.attributes.constitucion.acto_de_creacion;
                _constitucionDto.estado_matricula = data.data.attributes.constitucion.estado_matricula;
                _constitucionDto.setFechaMatricula(data.data.attributes.constitucion.fecha_matricula);
                _constitucionDto.setFechaRenovacion(data.data.attributes.constitucion.fecha_ultima_renovacion);
                _constitucionDto.setFechaActoCreacion(data.data.attributes.constitucion.fecha_acto_creacion);
            }
            var populates = new List<string>() {
                "constitucion.acto_administrativo"
            };
            //Buscar los populates de los adjuntos  para traer el nombre y url de los adjuntos
            var dataP = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.constitucion != null)
            {
                if (dataP.data.attributes.constitucion.acto_administrativo.data != null)
                {
                    _constitucionDto.url_adjunto_acto_administrativo = dataP.data.attributes.constitucion.acto_administrativo.data.attributes.url;
                    _constitucionDto.nombre_adjunto_acto_administrativo = dataP.data.attributes.constitucion.acto_administrativo.data.attributes.name;
                }
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (context.Validate())
            {
                int idActoAdministrativo = 0;
                if (_constitucionDto.acto_administrativo != null)
                    idActoAdministrativo = await _superTransporteService.PostFile(_constitucionDto.acto_administrativo);

                var resultado = new RespuestaPutCentro();

                if (idActoAdministrativo > 0)
                {
                    var objetoSend = new InfoConstitucionDtoRequestCEA<int>()
                    {
                        constitucion = new InfoConstitucionClsCEA<int>()
                        {
                            num_matricula_mercantil = _constitucionDto.num_matricula_mercantil,
                            acto_de_creacion = _constitucionDto.acto_de_creacion,
                            fecha_matricula = _constitucionDto.getFechaMatricula(),
                            fecha_ultima_renovacion = _constitucionDto.getFechaRenovacion(),
                            estado_matricula = _constitucionDto.estado_matricula,
                            fecha_acto_creacion = _constitucionDto.getFechaActoCreacion(),
                            acto_administrativo = idActoAdministrativo
                        }
                    };
                    resultado = await _superTransporteService.PutCentro(objetoSend, applicationShared.IdCentroStrappi);
                }
                else
                {
                    var objetoSend = new InfoConstitucionDtoRequestCEA<DataFile>()
                    {
                        constitucion = new InfoConstitucionClsCEA<DataFile>()
                        {
                            num_matricula_mercantil = _constitucionDto.num_matricula_mercantil,
                            acto_de_creacion = _constitucionDto.acto_de_creacion,
                            fecha_matricula = _constitucionDto.getFechaMatricula(),
                            fecha_ultima_renovacion = _constitucionDto.getFechaRenovacion(),
                            estado_matricula = _constitucionDto.estado_matricula,
                            fecha_acto_creacion = _constitucionDto.getFechaActoCreacion(),
                            acto_administrativo = null
                        }
                    };
                    resultado = await _superTransporteService.PutCentro(objetoSend, applicationShared.IdCentroStrappi);
                }


                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            _loader.Hide();
        }

        private void HandleArchivoActoAdministrativo(InputFileChangeEventArgs e)
        {
            _constitucionDto.acto_administrativo = e.File;
        }
        private async Task DownloadFileActoAdministrativo()
        {
            if (_constitucionDto.url_adjunto_acto_administrativo != null)
            {
                string fileUrl = _constitucionDto.url_adjunto_acto_administrativo.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}
