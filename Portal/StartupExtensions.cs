using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using portalAdministrativoSISEC.Aplication;
using portalAdministrativoSISEC.Services.Agendamiento;
using portalAdministrativoSISEC.Services.Agendamiento.Agenda;
using portalAdministrativoSISEC.Services.Agendamiento.ConfigruracionCuposReglas;
using portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Services.Agendamiento.InicioAgendamiento;
using portalAdministrativoSISEC.Services.Agendamiento.Perfil;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo;
using portalAdministrativoSISEC.Services.PowerBi;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Util.Helpers;
using portalAdministrativoSISEC.Util.Middleware;
using System;
using System.Net.Http;

namespace portalAdministrativoSISEC
{
	public static class StartupExtensions
	{
		public static void AgregarServicios(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IHorarioAtencionService, HorarioAtencionService>();
			services.AddScoped<IAesEncryptionHelper, AesEncryptionHelper>();
			services.AddScoped<IInicioAgendamiento, InicioAgendamientoServices>();
			services.AddScoped<IParametrizacionHorarioService, ParametrizacionHorarioService>();
			services.AddScoped<IPortalAdministrativoService, PortalAdministrativoService>();
			services.AddScoped<IApiMilicenciaService, ApiMilicenciaService>();
			services.AddScoped<IMiLicenciaService, MiLicenciaService>();
			services.AddScoped<IPermisoService,PermisoService>();
            services.AddScoped<IOfuscamientoService, OfuscamientoService>();
            services.AddScoped<IConfiguracionCuposReglas, ConfiguracionCuposReglas>();
			services.AddScoped<IPerfilAgendaService, PerfilAgendaService>();
			services.AddScoped<IAgendaService, AgendaService>();
			services.AddScoped<ISuperTransporteService, SuperTransporteService>();
			services.AddScoped<IReporteService, ReporteService>();
            services.AddScoped<IErrorBoundaryLogger, BlazorExceptionLogger>();
            services.AddScoped<ProtectedSessionStorage>();
        }

		public static void AgregarHttpClient(this IServiceCollection services, IOptions<AppSettings> options)
		{
			services.AddHttpClient();
			services.AddHttpClient("SisecAdmin", c =>
			{
				c.BaseAddress = new Uri(options.Value.uriSisecParametization);
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				}
			});

			services.AddHttpClient("ApiStrapi", c =>
			{
				c.BaseAddress = new Uri(options.Value.EndPointApiStrapi);
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				}
			});
			services.AddHttpClient("ApiSuperT", c =>
			{
				c.BaseAddress = new Uri(options.Value.EndPointApiSuperT);
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				}
			});
			services.AddHttpClient("ApiSuperVigilados", c =>
			{
				c.BaseAddress = new Uri(options.Value.EndPointApiVigilados);
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				}
			});

			services.AddHttpClient("PortalAdministrativo", c =>
			{
				c.BaseAddress = new Uri(options.Value.ApiPortalAdministrativo.Url);
			})
			.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback =
				(httpRequestMessage, cert, cetChain, policyErrors) =>
				{
					return true;
				}
			});
 
		}

		private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
				.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
		}
	}
}