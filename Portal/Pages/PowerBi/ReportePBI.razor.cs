using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Contracts.PowerBi;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Linq;
using portalAdministrativoSISEC.Entidades.PowerBi;
using System;
using Microsoft.Extensions.Options;
using portalAdministrativoSISEC.Aplication;
using Microsoft.Extensions.Configuration;

namespace portalAdministrativoSISEC.Pages.PowerBi
{
    public partial class ReportePBI
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IReporteService _resporteService { get; set; }

        [Inject]
        private IToastService toastService { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        private ApplicationShared applicationShared = new ApplicationShared();
        private IJSObjectReference? embedModule;
        private EmbedParams embedParams;
        private string errorMessage = string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
                    applicationShared = result.Value;
                    var embedModuleTask = JS.InvokeAsync<IJSObjectReference>("import", "./js/embed.js");

                    //string IdRunt = applicationShared.Plataforma == "CRC" ? "14473357" : "1304999";

                    ConfiguracionReportesPowerBI reports = Configuration.GetSection("ConfigPowerBI").Get<ConfiguracionReportesPowerBI>();

                    embedParams = _resporteService.ObtenerReporteEmbed(reports?.Reportes?.First()?.ReportId ?? Guid.Parse("a4e6c5d0-b039-4430-8bcb-f54e45077aa4"), applicationShared.IdRunt, applicationShared.Plataforma);
                    embedModule = await embedModuleTask;
                    embedModule = await embedModuleTask;
                    StateHasChanged();
                }
                catch (Exception)
                {
                    Error();
                }
            }
            else if (embedModule is not null && embedParams is not null && string.IsNullOrEmpty(errorMessage))
            {
                try
                {
                    errorMessage = string.Empty;
                    await embedModule.InvokeVoidAsync("embedReport",
                        "reportPBI",
                        embedParams.EmbedReport.FirstOrDefault().ReportId,
                        embedParams.EmbedReport.FirstOrDefault().EmbedUrl,
                        embedParams.EmbedToken.Token,
                        embedParams.EmbedReport.FirstOrDefault().NamePages,
                        embedParams.EmbedReport.FirstOrDefault().Plataforma
                        );
                }
                catch (Exception)
                {
                    Error();
                }
            }
            else if (embedModule is null || embedParams is null)
            {
                Error();
            }
        }

        private void Error()
        {
            errorMessage = "Hubo un problema cargando el reporte, intente recargar la página. Si el error persiste, comuniquese con el administrador del sistema.";
            StateHasChanged();
        }
    }
}

