using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Services.SuperTransporte;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace portalAdministrativoSISEC.Pages.SuperTransporte
{
    public partial class CentroSuperCEA
    {
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        public ISuperTransporteService _superTransporteService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }

        private ApplicationShared applicationShared = new ApplicationShared();
        private LoaderEventSubmit _loader = new LoaderEventSubmit();
        private CentroSuperDtoCEA _centroSuper = new CentroSuperDtoCEA();
        string botonInfoBasica = "boton-verde";
        string botonInfoHomologado = "boton-verde";
        string botonResolucionHabilitacion = "boton-verde";
        string botonInfoConstitucion = "boton-verde";
        string botonInfoPropietarios = "boton-verde";
        string botonPoliza = "boton-verde";
        string botonCertificacionOEC = "boton-verde";
        string botonProgramasConvenio = "boton-verde";
        string botonLicenciaFuncionamiento = "boton-verde";
        string botonInfraestructura = "boton-verde";
        string botonInstructores = "boton-verde";
        string botonVehiculos = "boton-verde";
        string botonRepresentanteLegal = "boton-verde";

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "*");
            if (data != null && data.data.attributes.nivel != null) botonInfoBasica = "boton-verde"; else botonInfoBasica = "boton-primario";
            if (data != null && data.data.attributes.homologado != null) botonInfoHomologado = "boton-verde"; else botonInfoHomologado = "boton-primario";
            if (data != null && data.data.attributes.resolucion_de_habilitacion != null ) botonResolucionHabilitacion = "boton-verde"; else botonResolucionHabilitacion = "boton-primario";
            if (data != null && data.data.attributes.constitucion != null) botonInfoConstitucion = "boton-verde"; else botonInfoConstitucion = "boton-primario";
            if (data != null && data.data.attributes.propietarios.Count > 0) botonInfoPropietarios = "boton-verde"; else botonInfoPropietarios = "boton-primario";
            if (data != null && data.data.attributes.poliza != null) botonPoliza = "boton-verde"; else botonPoliza = "boton-primario";
            if (data != null && data.data.attributes.certificacion_oec != null) botonCertificacionOEC = "boton-verde"; else botonCertificacionOEC= "boton-primario";
            if (data != null && data.data.attributes.convenios != null) botonProgramasConvenio = "boton-verde"; else botonProgramasConvenio = "boton-primario";
            if (data != null && data.data.attributes.licencia_funcionamiento != null) botonLicenciaFuncionamiento = "boton-verde"; else botonLicenciaFuncionamiento = "boton-primario";
            if (data != null && data.data.attributes.infraestructura != null) botonInfraestructura = "boton-verde"; else botonInfraestructura = "boton-primario";
            if (data != null && data.data.attributes.instructores.Count > 0) botonInstructores = "boton-verde"; else botonInstructores = "boton-primario";
            if (data != null && data.data.attributes.vehiculos.Count > 0) botonVehiculos = "boton-verde"; else botonVehiculos = "boton-primario";
            if (data != null && data.data.attributes.representante_legal != null) botonRepresentanteLegal = "boton-verde"; else botonRepresentanteLegal = "boton-primario";

            if (data != null)
            {
                if (data.data.attributes.motivos != null)
                {
                    _centroSuper.motivos = data.data.attributes.motivos;
                    _centroSuper.completado = data.data.attributes.completado;
                }
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (context.Validate())
            {
                var resultado = await _superTransporteService.PutCentro(_centroSuper, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            _loader.Hide();
        }
        public void ComponenteInfoBasica()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/informacion-basica", false);
        }
        public void ComponenteInfoHomologado()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/informacion-homologado", false);
        }
        public void ComponenteProgramasConvenio()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/programas-convenio", false);
        }
        public void ComponenteInfoConstitucion()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/informacion-constitucion", false);
        }       
        public void ComponenteResolucionHabilitacion()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/resolucion-habilitacion", false);
        }
        public void ComponentePoliza()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/poliza", false);
        }        
        public void ComponenteLicenciaFuncionamiento()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/licencia-funcionamiento", false);
        }
        public void ComponenteCertificacionOEC()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/certificacion-oec", false);
        }
        public void ComponenteInfraestructura()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/infraestructura", false);
        }
        public void ComponenteInfoPropietarios()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/informacion-propietarios", false);
        }
        public void ComponenteInstructoresCEA()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/instructor", false);
        }    
        public void ComponenteInfoVehiculos()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/informacion-vehiculos", false);
        }
        public void ComponenteRepresentanteLegal()
        {
            Navigation.NavigateTo("/supertransporte/centro-cea/representante-legal", false);
        }
    }
}
