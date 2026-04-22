using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Services.SuperTransporte;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using BlazorInputFile;
using System.Net.Http.Headers;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class RegistroREPS
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
        private RegistroREPSDto _registroREPS = new RegistroREPSDto();
        private ApplicationShared applicationShared = new ApplicationShared();
        private List<SelectSuperTransporteDTO> _selectSecretaria = new List<SelectSuperTransporteDTO>();
        private List<DepartamentosSuperTransporteDTO> _selectDepartamento = new List<DepartamentosSuperTransporteDTO>();

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var campos = SelectSuperTransporteIndex.llenarCamposConCiudades();
            //_selectSecretaria = await _superTransporteService.GetListaMaestra("SecretariasSalud", campos);
            var selectDepartamento = await _superTransporteService.GetListaMaestra("Departamentos", campos);
            var ciudades = await _superTransporteService.GetListaMaestra("Ciudades", campos);
            _selectSecretaria = await _superTransporteService.GetListaMaestraSuperT("secretariasSalud");
            //var selectDepartamento = await _superTransporteService.GetListaMaestraSuperT("territoriales");
            //var ciudades = await _superTransporteService.GetListaMaestraSuperT("municipios");
            _selectDepartamento = selectDepartamento.Select(s => new DepartamentosSuperTransporteDTO
            { 
                id = s.id,
                codigoDep = s.codigoDep,
                nombre = s.nombre,
                ciudades = ciudades.Where(w => w.codigoDep == s.codigoDep).Select(sc => new CiudadesSuperTransporteDTO { codigo = sc.codigo, codigoDep = sc.codigoDep, nombre = sc.nombre }).ToList()
            }).ToList();

            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "registro_reps");
            if (data != null && data.data.attributes.registro_reps != null)
            {
                _registroREPS.codigo_inscripcion = data.data.attributes.registro_reps?.codigo_inscripcion;
                _registroREPS.setFecha_inscripcion(data.data.attributes.registro_reps.fecha_inscripcion);
                _registroREPS.setFecha_vencimiento(data.data.attributes.registro_reps.fecha_vencimiento);
                _registroREPS.setFecha_renovacion(data.data.attributes.registro_reps.fecha_renovacion);
                _registroREPS.setFecha_ultima_autoevaluacion(data.data.attributes.registro_reps.fecha_ultima_autoevaluacion);
                _registroREPS.secretaria_de_salud = data.data.attributes.registro_reps?.secretaria_de_salud;
                _registroREPS.departamento = data.data.attributes.registro_reps?.departamento;
                _registroREPS.ciudad = data.data.attributes.registro_reps?.ciudad;
            }
            var populates = new List<string>() {
                "registro_reps.formulario_inscripcion"
            };
            
            var dataP = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (dataP != null && dataP.data.attributes.registro_reps != null)
            {
                if (dataP.data.attributes.registro_reps.formulario_inscripcion.data != null)
                {
                    _registroREPS.url_adjunto_formulario_inscripcion= dataP.data.attributes.registro_reps.formulario_inscripcion.data.attributes.url;
                    _registroREPS.nombre_adjunto_formulario_inscripcion = dataP.data.attributes.registro_reps.formulario_inscripcion.data.attributes.name;
                }
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (context.Validate())
            {
                int idFile = await _superTransporteService.PostFile(_registroREPS.formulario_inscripcion);
                if (idFile > 0)
                {
                    var resultado = await _superTransporteService.PutCentro(new RegistroREPSDtoRequest<int>()
                    {
                        registro_reps = new RegistroREPSCls<int>()
                        {
                            codigo_inscripcion = _registroREPS.codigo_inscripcion,
                            fecha_inscripcion = _registroREPS.getFecha_inscripcion(),
                            fecha_vencimiento = _registroREPS.getFecha_vencimiento(),
                            fecha_renovacion = _registroREPS.getFecha_renovacion(),
                            fecha_ultima_autoevaluacion = _registroREPS.getFecha_ultima_autoevaluacion(),
                            secretaria_de_salud = _registroREPS.secretaria_de_salud,
                            departamento = _registroREPS.departamento,
                            ciudad = _registroREPS.ciudad,
                            formulario_inscripcion = idFile
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
            _registroREPS.formulario_inscripcion = e.File;
        }

        private bool validarCidudadseleccionada() {
            if (_selectDepartamento.Find(w => w.codigoDep == _registroREPS.departamento) != null)
                if (_selectDepartamento.Find(w => w.codigoDep == _registroREPS.departamento).ciudades != null)
                    if (_selectDepartamento.Find(w => w.codigoDep == _registroREPS.departamento).ciudades.Find(f => f.codigo == _registroREPS.ciudad) == null)
                        _registroREPS.ciudad = "";
            return true;
        }
        private async Task DownloadFileFormulario()
        {
            if (_registroREPS.url_adjunto_formulario_inscripcion!= null)
            {
                string fileUrl = _registroREPS.url_adjunto_formulario_inscripcion.ToString();
                string sasToken = await _superTransporteService.GetBlobSasToken();
                string fileDownload = fileUrl + sasToken;

                await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
            }

        }
    }
}
