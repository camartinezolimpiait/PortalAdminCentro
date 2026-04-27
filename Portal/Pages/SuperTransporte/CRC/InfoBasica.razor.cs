using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class InfoBasica
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
        private InfoBasicaDto _infoBasica = new InfoBasicaDto();
        private ApplicationShared applicationShared = new ApplicationShared();


        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var data = await _superTransporteService.GetCentro(applicationShared.IdCentroStrappi, "*");
            if (data != null) {
                if(data.data.attributes.capacidad_certificados != null)
                    _infoBasica.capacidad_certificados = (int)data.data.attributes.capacidad_certificados;
                if (data.data.attributes.horas_de_atencion_por_dia != null)
                    _infoBasica.horas_de_atencion_por_dia = (int)data.data.attributes.horas_de_atencion_por_dia;
            }
        }
        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (context.Validate())
            {
                var resultado = await _superTransporteService.PutCentro(_infoBasica, applicationShared.IdCentroStrappi);
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


