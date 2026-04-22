using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Services.SuperTransporte;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class InfoConstitucion
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
        private InfoConstitucionDto _constitucionDto = new InfoConstitucionDto();
        private ApplicationShared applicationShared = new ApplicationShared();
        private List<SelectSuperTransporteDTO> _select = new List<SelectSuperTransporteDTO>();

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            //TODO: Actualizar select
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_select = await _superTransporteService.GetListaMaestra("EstadoMatriculaMercantil", campos);
            _select = await _superTransporteService.GetListaMaestraSuperT("matriculaMercantilEstados");
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "constitucion");
            if (data != null && data.data.attributes.constitucion != null)
            {
                _constitucionDto.num_matricula_mercantil = data.data.attributes.constitucion?.num_matricula_mercantil;
                _constitucionDto.estado_matricula = data.data.attributes.constitucion.estado_matricula;
                _constitucionDto.setFechaMatricula(data.data.attributes.constitucion.fecha_matricula);
                _constitucionDto.setFechaRenovacion(data.data.attributes.constitucion.fecha_ultima_renovacion);
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            _superTransporteService.SetPlataforma("crcs");
            if (context.Validate())
            {
                var resultado = await _superTransporteService.PutCentro(new InfoConstitucionDtoRequest()
                {
                    constitucion = new InfoConstitucionCls()
                    {
                        num_matricula_mercantil = _constitucionDto.num_matricula_mercantil,
                        estado_matricula = _constitucionDto.estado_matricula,
                        fecha_matricula = _constitucionDto.getFechaMatricula(),
                        fecha_ultima_renovacion = _constitucionDto.getFechaRenovacion()
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
    }
}
