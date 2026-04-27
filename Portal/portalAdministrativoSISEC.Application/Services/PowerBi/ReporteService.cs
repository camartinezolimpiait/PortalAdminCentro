using portalAdministrativoSISEC.Application.Contracts.PowerBi;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.PowerBi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace portalAdministrativoSISEC.Services.PowerBi
{
    public class ReporteService : IReporteService
    {
        private ConfiguracionReportesPowerBI c_configuracionReportesPowerBI;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public ReporteService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            c_configuracionReportesPowerBI = _configuration.GetSection("ConfigPowerBI").Get<ConfiguracionReportesPowerBI>();
        }

        public ReporteService(ConfiguracionReportesPowerBI configuracionReportesPowerBI)
        {
            c_configuracionReportesPowerBI = configuracionReportesPowerBI;
        }

        public EmbedParams ObtenerReporteEmbed(Guid idReporte, string IdRunt, string plataforma)
        {
            var embedParams = new EmbedParams();
            try
            {
                var configReport = ConfiguracionReporte(idReporte);
                PowerBIClient pbiClient = GetPowerBIClient(configReport);
                var pbiReport = pbiClient.Reports.GetReportInGroup(configReport.WorkspaceId, configReport.ReportId);

                var datasetIds = new List<Guid> { Guid.Parse(pbiReport.DatasetId) };

                var embedReports = new List<EmbedReport>() {
                    new EmbedReport
                    {
                        ReportId = pbiReport.Id, ReportName = pbiReport.Name, EmbedUrl = pbiReport.EmbedUrl,
                        NamePages = plataforma.Equals("CRC") ?
                        configReport.NamePages.CRC:
                        configReport.NamePages.CEA,
                        Plataforma = plataforma
                    }
                };

                var embedToken = GetEmbedToken(configReport.ReportId, datasetIds, configReport.WorkspaceId, pbiClient, IdRunt);

                embedParams = new EmbedParams
                {
                    EmbedReport = embedReports,
                    Type = "Report",
                    EmbedToken = embedToken
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error al obtener el reporte.", ex);
            }
            return embedParams;
        }

        private PowerBIClient GetPowerBIClient(ConfiguracionReporte configReport)
        {
            X509Certificate2 certificate = new X509Certificate2();
            var token = GetAccessToken(configReport, certificate);
            var tokenCredentials = new TokenCredentials(token, "Bearer");
            return new PowerBIClient(new Uri(c_configuracionReportesPowerBI.ApiUrl), tokenCredentials);
        }

        private string GetAccessToken(ConfiguracionReporte configReport, X509Certificate2 certificate)
        {
            try
            {
                AuthenticationResult authenticationResult;
                if (configReport.esMasterUser)
                {
                    IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(configReport.ApplicationId.ToString())
                        .WithAuthority(c_configuracionReportesPowerBI.AuthorityUrl).Build();
                    var userAccounts = clientApp.GetAccountsAsync().Result;
                    try
                    {
                        authenticationResult = clientApp.AcquireTokenSilent(
                            new List<string>() { c_configuracionReportesPowerBI.ResourceUrl },
                            userAccounts.FirstOrDefault()).ExecuteAsync().Result;
                    }
                    catch (MsalUiRequiredException)
                    {
                        SecureString password = new SecureString();
                        foreach (var key in configReport.Password)
                        {
                            password.AppendChar(key);
                        }
                        authenticationResult = clientApp.AcquireTokenByUsernamePassword(
                            new List<string>() { c_configuracionReportesPowerBI.ResourceUrl },
                            configReport.Username,
                            password).ExecuteAsync().Result;
                    }
                }
                else
                {
                    IConfidentialClientApplication clientApp;
                    var tenantSpecificUrl = c_configuracionReportesPowerBI.AuthorityUrl.Replace("common", configReport.Tenant.ToString());
                    if (configReport.esCertificate)
                    {
                        clientApp = ConfidentialClientApplicationBuilder
                        .Create(configReport.ApplicationId.ToString())
                        .WithCertificate(certificate)
                        .WithAuthority(tenantSpecificUrl)
                        .Build();

                        authenticationResult = clientApp.AcquireTokenForClient(new List<string>() { c_configuracionReportesPowerBI.ResourceUrl }).ExecuteAsync().Result;
                    }
                    else
                    {
                        clientApp = ConfidentialClientApplicationBuilder
                        .Create(configReport.ApplicationId.ToString())
                        .WithClientSecret(Uri.EscapeDataString(configReport.ApplicationSecret))
                        .WithAuthority(tenantSpecificUrl)
                        .Build();

                        authenticationResult = clientApp.AcquireTokenForClient(new List<string>() { c_configuracionReportesPowerBI.ResourceUrl }).ExecuteAsync().Result;
                    }
                }

                return authenticationResult.AccessToken;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error al obtener el token de Azure", ex);
            }
        }

        private EmbedToken GetEmbedToken(Guid reportId,
            IList<Guid> datasetIds
            , [Optional] Guid targetWorkspaceId
            , PowerBIClient pbiClient
            , string idRunt
            )
        {
            try
            {
                var rlsIdentity = new EffectiveIdentity(
                    username: idRunt,
                    roles: new List<string> { "IdRunt" },
                    datasets: new List<string> { datasetIds.FirstOrDefault().ToString() }
                );

                var tokenRequest = new GenerateTokenRequestV2(
                    reports: new List<GenerateTokenRequestV2Report>() { new GenerateTokenRequestV2Report(reportId) },
                    datasets: datasetIds.Select(datasetId => new GenerateTokenRequestV2Dataset(datasetId.ToString())).ToList(),
                    targetWorkspaces: targetWorkspaceId != Guid.Empty ? new List<GenerateTokenRequestV2TargetWorkspace>() { new GenerateTokenRequestV2TargetWorkspace(targetWorkspaceId) } : null,
                    identities: new List<EffectiveIdentity> { rlsIdentity }
                );

                var embedToken = pbiClient.EmbedToken.GenerateToken(tokenRequest);

                return embedToken;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error al generar token de acceso embed", ex);
            }
        }

        private ConfiguracionReporte ConfiguracionReporte(Guid idReporte)
        {
            return c_configuracionReportesPowerBI.Reportes.FirstOrDefault(x => x.ReportId == idReporte);
        }
    }
}

