using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using Microsoft.PowerBI.Api.Models;
using portalAdministrativoSISEC.Aplication;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Pages.Agendamiento;
using portalAdministrativoSISEC.Application.Contracts;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using portalAdministrativoSISEC.Services;
using portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Util.Helpers;
using portalAdministrativoSISEC.Util.Middleware;

namespace portalAdministrativoSISEC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddRazorPages();
            services.AddServerSideBlazor();

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
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<ApplicationSevice>();
            services.AddScoped<DataInformation>();
            services.AddScoped<IApiService, ApiService>();
            services.AddBlazoredToast();
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement);
            });
            // Legacy Startup remains compilable, but Program.cs owns the runtime bootstrapping.
            services.AgregarHttpClient(Configuration);
            services.AddHttpClient<ICategoriaService, CategoriaService>(client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("AppSettings:ApiPortalAdministrativo:Url").Value);
            }
            );
            services.AddHttpClient<ITokenService, TokenService>(client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("AppSettings:ApiPortalAdministrativo:Url").Value);
            }
            );
            services.AddHttpClient<IPerfilService, PerfilService>(client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("AppSettings:uriSisecAuth").Value);
            }
            );
            services.AgregarServicios(Configuration);
            if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
            {
                services.AddScoped<HttpClient>(s =>
                {
                    var uriHelper = s.GetRequiredService<NavigationManager>();
                    return new HttpClient
                    {
                        BaseAddress = new Uri(uriHelper.BaseUri)
                    };
                });
            }
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            }
        }
    }
}


