using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using System.Collections.Generic;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades;
using Microsoft.AspNetCore.Components.Forms;

namespace portalAdministrativoSISEC.Pages.SuperTransporte
{
    public partial class CentroSuper
    {
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public ISuperTransporteService _superTransporteService { get; set; }
        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }

        private ApplicationShared applicationShared = new ApplicationShared();
        private LoaderEventSubmit _loader = new LoaderEventSubmit();
        private CentroSuperDto _centroSuper = new CentroSuperDto();
        string botonInfoBasica = "boton-verde";
        string botonInfoHomologado = "boton-verde";
        string botonResolucionHabilitacion = "boton-verde";
        string botonInfoConstitucion = "boton-verde";
        string botonEstadoAcreditacion = "boton-verde";
        string botonPoliza = "boton-verde";
        string botonRegistroReps = "boton-verde";
        string botonProfesionalesCertificadores = "boton-verde";
        string botonProfesionalesSalud = "boton-verde";
        string botonInfraestructura = "boton-verde";
        string botonInterconexionRUNT = "boton-verde";
        string botonRepresentanteLegal = "boton-verde";

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "*");

            if (data != null)
            {
                if (data.data.attributes.capacidad_certificados != null) botonInfoBasica = "boton-verde"; else botonInfoBasica = "boton-primario";
                if (data.data.attributes.info_homologado != null) botonInfoHomologado = "boton-verde"; else botonInfoHomologado = "boton-primario";
                if (data.data.attributes.resolucion_de_habilitacion != null) botonResolucionHabilitacion = "boton-verde"; else botonResolucionHabilitacion = "boton-primario";
                if (data.data.attributes.constitucion != null) botonInfoConstitucion = "boton-verde"; else botonInfoConstitucion = "boton-primario";
                if (data.data.attributes.estado_acreditacion_onac != null) botonEstadoAcreditacion = "boton-verde"; else botonEstadoAcreditacion = "boton-primario";
                if (data.data.attributes.poliza != null) botonPoliza = "boton-verde"; else botonPoliza = "boton-primario";
                if (data.data.attributes.registro_reps != null) botonRegistroReps = "boton-verde"; else botonRegistroReps = "boton-primario";
                if (data.data.attributes.profesionales_certificadores != null) botonProfesionalesCertificadores = "boton-verde"; else botonProfesionalesCertificadores = "boton-primario";
                if (data.data.attributes.prof_salud != null) botonProfesionalesSalud = "boton-verde"; else botonProfesionalesSalud = "boton-primario";
                if (data.data.attributes.infraestructura != null) botonInfraestructura = "boton-verde"; else botonInfraestructura = "boton-primario";
                if (data.data.attributes.interconexion_runt != null) botonInterconexionRUNT = "boton-verde"; else botonInterconexionRUNT = "boton-primario";
                if (data.data.attributes.representante_legal != null) botonRepresentanteLegal = "boton-verde"; else botonRepresentanteLegal = "boton-primario";

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
                    Navigation.NavigateTo("/supertransporte/centro", false);
                }
            }
            _loader.Hide();
        }
        public void ComponenteInfoBasica()
        {
            Navigation.NavigateTo("/supertransporte/centro/informacion-basica", false);
        }
        public void ComponenteInfoHomologado()
        {
            Navigation.NavigateTo("/supertransporte/centro/informacion-homologado", false);
        }
        public void ComponenteResolucionHabilitacion()
        {
            Navigation.NavigateTo("/supertransporte/centro/resolucion-habilitacion", false);
        }
        public void ComponenteInfoConstitucion()
        {
            Navigation.NavigateTo("/supertransporte/centro/informacion-constitucion", false);
        }
        public void ComponenteAcreditacionONAC()
        {
            Navigation.NavigateTo("/supertransporte/centro/acreditacion-onac", false);
        }
        public void RegistroREPS()
        {
            Navigation.NavigateTo("/supertransporte/centro/registro-reps", false);
        }
        public void ComponentePoliza()
        {
            Navigation.NavigateTo("/supertransporte/centro/poliza", false);
        }
        public void ComponenteProfesionalesCertificadores()
        {
            Navigation.NavigateTo("/supertransporte/centro/profesional-certificadores", false);
        }
        public void ComponenteProfesionalesSalud()
        {
            Navigation.NavigateTo("/supertransporte/centro/profesional-salud", false);
        }
        public void Componenteinfraestructura()
        {
            Navigation.NavigateTo("/supertransporte/centro/infraestructura", false);
        }
        public void ComponenteRepresentanteLegal()
        {
            Navigation.NavigateTo("/supertransporte/centro/representante-legal-CRC", false);
        }
        public void ComponenteInterconexionRUNT()
        {
            Navigation.NavigateTo("/supertransporte/centro/interconexion-runt", false);
        }
    }
}


