using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.PQRSF;
using portalAdministrativoSISEC.Services.Agendamiento.Agenda;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.PQRSF
{
    public partial class PQRSF
    {
        #region Fields

        private readonly InformacionBasica _informacionBasica = new InformacionBasica();
        private ApplicationShared _applicationShared = new ApplicationShared();

        #endregion Fields

        #region Properties

        [Inject] public NavigationManager Navigation { get; set; }
        public List<TipoDocumentoDTO> TiposDeDocumento { get; set; }
        public List<string> TipoSolicitud { get; set; }
        [Inject] public IAgendaService AgendaService { get; set; }
        [Inject] private IWebHostEnvironment Environment { get; set; }
        [Inject] private IConfiguration Configuration { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [Inject] private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        #endregion Properties

        #region Public Methods

        public bool EnviarCorreoElectronico(string body, string subject, string emailTo)
        {
            MailAddress to = new(emailTo);
            MailAddress from = new(Configuration.GetSection("NetworkCredential:CorreoSaliente").Value);
            MailMessage email = new(from, to);

            email.CC.Add(from);
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;

            SmtpClient smtp = new(Configuration.GetSection("NetworkCredential:Host").Value, Convert.ToInt32(Configuration.GetSection("NetworkCredential:Puerto").Value))
            {
                Credentials = new NetworkCredential(Configuration.GetSection("NetworkCredential:CorreoSaliente").Value,
                Configuration.GetSection("NetworkCredential:Contraseña").Value),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            try
            {
                smtp.Send(email);
                return true;
            }
            catch (SmtpException ex)
            {
                ToastService.ShowError($"No se pudo enviar el correo electrónico{ex?.Message} \n {ex?.InnerException}", "Información");
                return false;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(_applicationShared.NameLocalStorage);
            _applicationShared = result.Value;
            TiposDeDocumento = await AgendaService.GetTipoDocumento();
            TiposDeDocumento = TiposDeDocumento.Where(x => x.ClienteId == 1).ToList();
            List<string> ListaSolicutd = new List<string>(new string[] { "Petición", "Queja", "Reclamo", "Solicitud", "Sugerencia", "Felicitación" });
            TipoSolicitud = ListaSolicutd;
        }

        #endregion Protected Methods

        #region Private Methods

        private void GenerarInformacion(EditContext context)
        {
            string contentRootPath = Environment.ContentRootPath;
            string webRootPath = Environment.WebRootPath;
            string rootpath = Path.Combine(webRootPath, $"Plantilla", $"pqr_portal_administrativo.html");
            string subject = string.Empty;
            DateTime now = DateTime.Now;
            if (context.Validate())
            {
                switch (_applicationShared.Plataforma)
                {
                    case "CRC":
                        subject = $"{_informacionBasica.tipo_solicitud} - {_applicationShared.Plataforma} Salud";
                        break;

                    case "CEA":
                        subject = $"{_informacionBasica.tipo_solicitud} - {_applicationShared.Plataforma} Escuela";
                        break;
                }
                if (rootpath != null)
                {
                    string html = File.ReadAllText(rootpath);
                    var plantilla = html
                        .Replace("$NombreCompleto", $"{HttpUtility.HtmlEncode(_informacionBasica.primer_nombre)}  {HttpUtility.HtmlEncode(_informacionBasica.primer_apellido)}")
                        .Replace("$Documento", $"{HttpUtility.HtmlEncode(_informacionBasica.num_id)}")
                        .Replace("$NombreCentro", $"{HttpUtility.HtmlEncode(_applicationShared.Plataforma)}")
                        .Replace("$Correo", $"{HttpUtility.HtmlEncode(_informacionBasica.email)}")
                        .Replace("$Telefono", $"{HttpUtility.HtmlEncode(_informacionBasica.telefono)}")
                        .Replace("$IdSolicitud", now.ToString("yyMMddHHmmss"))
                        .Replace("$TipoSolicitud", $"{HttpUtility.HtmlEncode(_informacionBasica.tipo_solicitud)}")
                        .Replace("$Mensaje", $"{HttpUtility.HtmlEncode(_informacionBasica.detalle_solicitud)}");

                    if (EnviarCorreoElectronico(plantilla, subject, _informacionBasica.email))
                    {
                        ToastService.ShowSuccess(
                            $@"El correo ha sido enviado correctamente",
                            "Información");
                        _informacionBasica.primer_nombre = string.Empty;
                        _informacionBasica.primer_apellido = string.Empty;
                        _informacionBasica.num_id = string.Empty;
                        _informacionBasica.segundo_nombre = string.Empty;
                        _informacionBasica.segundo_apellido = string.Empty;
                        _informacionBasica.email = string.Empty;
                        _informacionBasica.telefono = string.Empty;
                        _informacionBasica.detalle_solicitud = string.Empty;
                        _informacionBasica.tipo_id = string.Empty;
                        _informacionBasica.tipo_solicitud = string.Empty;
                    }
                    else
                    {
                        ToastService.ShowError($@"No se pudo enviar el correo electrónico", "Información");
                    }
                }
                else
                {
                    ToastService.ShowError($@"Se ha producido un error inesperado con la ruta de acceso.", "Información");
                }
            }
        }

        #endregion Private Methods
    }
}