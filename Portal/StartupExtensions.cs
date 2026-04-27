using System;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using portalAdministrativoSISEC.Aplication;
using portalAdministrativoSISEC.Application.PortalAdministrativo;
using portalAdministrativoSISEC.Application.CompraPin;
using portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;
using portalAdministrativoSISEC.Application.Data;
// Inicio código generado por GitHub Copilot
using portalAdministrativoSISEC.Application.Contracts;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Agenda;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.ConfiguracionCuposReglas;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.InicioAgendamiento;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Horario;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Perfil;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.PowerBi;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
// Fin código generado por GitHub Copilot
using portalAdministrativoSISEC.Services;
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

namespace portalAdministrativoSISEC
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddValidatedOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AppSettings>()
                .Bind(configuration.GetSection("AppSettings"))
                .Validate(settings => HasValue(settings.uriSisecAuth), "AppSettings:uriSisecAuth is required.")
                .Validate(settings => HasValue(settings.uriSisecParametization), "AppSettings:uriSisecParametization is required.")
                .Validate(settings => HasValue(settings.ApiPortalAdministrativo?.Url), "AppSettings:ApiPortalAdministrativo:Url is required.")
                .Validate(settings => HasValue(settings.ApiFrontMiLicencia?.Url), "AppSettings:ApiFrontMiLicencia:Url is required.")
                .ValidateOnStart();

            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            return services;
        }

        public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredToast();
            services.AddScoped<IErrorBoundaryLogger, BlazorExceptionLogger>();
            services.AddScoped<ProtectedSessionStorage>();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".SISEC.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.IsEssential = true;
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.OnAppendCookie = cookieContext => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement);
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ApplicationSevice>();
            services.AddScoped<DataInformation>();
            services.AddScoped<IAesEncryptionHelper, AesEncryptionHelper>();
            services.AddScoped<ICompraPinFlowService, CompraPinFlowService>();
            services.AddScoped<ICompraPinDatosBasicosService, CompraPinDatosBasicosService>();
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IHorarioAtencionService, HorarioAtencionService>();
            services.AddScoped<IInicioAgendamiento, InicioAgendamientoServices>();
            services.AddScoped<IParametrizacionHorarioService, ParametrizacionHorarioService>();
            services.AddScoped<IPortalAdministrativoService, PortalAdministrativoService>();
            services.AddScoped<IApiMilicenciaService, ApiMilicenciaService>();
            services.AddScoped<IMiLicenciaService, MiLicenciaService>();
            services.AddScoped<IPermisoService, PermisoService>();
            services.AddScoped<IOfuscamientoService, OfuscamientoService>();
            services.AddScoped<IConfiguracionCuposReglas, ConfiguracionCuposReglas>();
            services.AddScoped<IPerfilAgendaService, PerfilAgendaService>();
            services.AddScoped<IAgendaService, AgendaService>();
            services.AddScoped<ISuperTransporteService, SuperTransporteService>();
            services.AddScoped<IReporteService, ReporteService>();
            services.AddHttpClient<ICategoriaService, CategoriaService>(client =>
            {
                client.BaseAddress = RequiredUri(configuration, "AppSettings:ApiPortalAdministrativo:Url");
            });
            services.AddHttpClient<ITokenService, TokenService>(client =>
            {
                client.BaseAddress = RequiredUri(configuration, "AppSettings:ApiPortalAdministrativo:Url");
            });
            services.AddHttpClient<IPerfilService, PerfilService>(client =>
            {
                client.BaseAddress = RequiredUri(configuration, "AppSettings:uriSisecAuth");
            });

            return services;
        }

        public static IServiceCollection AddConfiguredHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            AddNamedClient(services, configuration, "SisecAdmin", "AppSettings:uriSisecParametization");
            AddNamedClient(services, configuration, "ApiStrapi", "AppSettings:EndPointApiStrapi");
            AddNamedClient(services, configuration, "ApiSuperT", "AppSettings:EndPointApiSuperT");
            AddNamedClient(services, configuration, "ApiSuperVigilados", "AppSettings:EndPointApiVigilados");
            AddNamedClient(services, configuration, "PortalAdministrativo", "AppSettings:ApiPortalAdministrativo:Url");
            AddNamedClient(services, configuration, "ApiFrontMiLicencia", "AppSettings:ApiFrontMiLicencia:Url");

            services.AddScoped<HttpClient>(serviceProvider =>
            {
                var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
                return new HttpClient
                {
                    BaseAddress = new Uri(navigationManager.BaseUri)
                };
            });

            return services;
        }

        public static void AgregarServicios(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices();
            services.AddInfrastructureServices(configuration);
        }

        public static void AgregarHttpClient(this IServiceCollection services, IOptions<AppSettings> options)
        {
            var settings = options.Value;
            services.AddHttpClient();
            AddNamedClient(services, "SisecAdmin", settings.uriSisecParametization);
            AddNamedClient(services, "ApiStrapi", settings.EndPointApiStrapi);
            AddNamedClient(services, "ApiSuperT", settings.EndPointApiSuperT);
            AddNamedClient(services, "ApiSuperVigilados", settings.EndPointApiVigilados);
            AddNamedClient(services, "PortalAdministrativo", settings.ApiPortalAdministrativo?.Url);
            AddNamedClient(services, "ApiFrontMiLicencia", settings.ApiFrontMiLicencia?.Url);
        }

        public static void AgregarHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredHttpClients(configuration);
        }

        private static void AddNamedClient(IServiceCollection services, IConfiguration configuration, string clientName, string key)
        {
            services.AddHttpClient(clientName, client =>
            {
                client.BaseAddress = RequiredUri(configuration, key);
                client.Timeout = TimeSpan.FromSeconds(120);
            });
        }

        private static void AddNamedClient(IServiceCollection services, string clientName, string baseAddress)
        {
            services.AddHttpClient(clientName, client =>
            {
                if (string.IsNullOrWhiteSpace(baseAddress))
                {
                    throw new InvalidOperationException($"Base address for HTTP client '{clientName}' is required.");
                }

                client.BaseAddress = new Uri(baseAddress, UriKind.Absolute);
                client.Timeout = TimeSpan.FromSeconds(120);
            });
        }

        private static Uri RequiredUri(IConfiguration configuration, string key)
        {
            var value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Configuration value '{key}' is required.");
            }

            return new Uri(value, UriKind.Absolute);
        }

        private static bool HasValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                _ = httpContext.Request.Headers["User-Agent"].ToString();
            }
        }
    }
}

