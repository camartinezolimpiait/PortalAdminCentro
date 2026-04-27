using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class InfoBasicaCEA
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
        private InfoBasicaDtoCEA _infoBasicaCEA = new InfoBasicaDtoCEA();
        private ApplicationShared applicationShared = new ApplicationShared();
        private List<SelectSuperTransporteDTO> _select = new List<SelectSuperTransporteDTO>();
        private bool CategoriasAutorizadasSeleccionadas = false;
        private bool _submitClicked = false;

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            

            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_select = await _superTransporteService.GetListaMaestra("NivelesCEA", campos);
            _select = await _superTransporteService.GetListaMaestraSuperT("clasificacionesCEA");
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "*");
            if (data != null) {
                _infoBasicaCEA.nivel = data.data.attributes.nivel;
                _infoBasicaCEA.auth_cursos_normas = data.data.attributes.auth_cursos_normas;
                _infoBasicaCEA.num_cert_conformidad = data.data.attributes.num_cert_conformidad;
                _infoBasicaCEA.forma_instructores = data.data.attributes.forma_instructores;
                if(data.data.attributes.categorias_autorizadas != null)
                    _infoBasicaCEA.categorias_autorizadas = data.data.attributes.categorias_autorizadas;
                _infoBasicaCEA.setFechaAprobacion(data.data.attributes.fecha_aprobacion_cert);
                _infoBasicaCEA.setFechaVencimiento(data.data.attributes.fecha_vencimiento_cert);
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            _submitClicked = true;
            if (context.Validate())
            {
                if (!_infoBasicaCEA.categorias_autorizadas.A1 &&
                    !_infoBasicaCEA.categorias_autorizadas.A2 &&
                    !_infoBasicaCEA.categorias_autorizadas.B1 &&
                    !_infoBasicaCEA.categorias_autorizadas.B2 &&
                    !_infoBasicaCEA.categorias_autorizadas.B3 &&
                    !_infoBasicaCEA.categorias_autorizadas.C1 &&
                    !_infoBasicaCEA.categorias_autorizadas.C2 &&
                    !_infoBasicaCEA.categorias_autorizadas.C3)
                {
                    CategoriasAutorizadasSeleccionadas = false;
                }
                else
                {
                    var resultado = await _superTransporteService.PutCentro(new InfoBasicaClsCEA()
                    {
                        nivel = _infoBasicaCEA.nivel,
                        auth_cursos_normas = _infoBasicaCEA.auth_cursos_normas,
                        num_cert_conformidad = _infoBasicaCEA.num_cert_conformidad,
                        fecha_aprobacion_cert = _infoBasicaCEA.getFechaAprobacion(),
                        fecha_vencimiento_cert = _infoBasicaCEA.getFechaVencimiento(),
                        forma_instructores = _infoBasicaCEA.forma_instructores,
                        categorias_autorizadas = _infoBasicaCEA.categorias_autorizadas
                    }, applicationShared.IdCentroStrappi);
                    if (resultado != null)
                    {
                        toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                        Navigation.NavigateTo("/supertransporte/centro-cea", false);
                        CategoriasAutorizadasSeleccionadas = true;
                    }
                }
                
            }
            _loader.Hide();
        }

        private void ActualizarCategoriasSeleccionadas(ChangeEventArgs e)
        {
            CategoriasAutorizadasSeleccionadas = _infoBasicaCEA.categorias_autorizadas.A1 ||
                                                  _infoBasicaCEA.categorias_autorizadas.A2 ||
                                                  _infoBasicaCEA.categorias_autorizadas.B1 ||
                                                  _infoBasicaCEA.categorias_autorizadas.B2 ||
                                                  _infoBasicaCEA.categorias_autorizadas.B3 ||
                                                  _infoBasicaCEA.categorias_autorizadas.C1 ||
                                                  _infoBasicaCEA.categorias_autorizadas.C2 ||
                                                  _infoBasicaCEA.categorias_autorizadas.C3;
        }
    }
}


