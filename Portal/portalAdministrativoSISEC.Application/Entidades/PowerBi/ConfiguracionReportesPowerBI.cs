using System;
using System.Collections.Generic;
using System.Text;

namespace portalAdministrativoSISEC.Entidades.PowerBi
{
    public class ConfiguracionReportesPowerBI
    {
        public string AuthorityUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string ApiUrl { get; set; }
        public string EmbedUrlBase { get; set; }
        public List<ConfiguracionReporte> Reportes { get; set; }
    }

    public class ConfiguracionReporte
    {
        public string TipoReporte { get; set; }
        public bool esMasterUser { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid ReportId { get; set; }
        public Guid ReportIdAdmin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApplicationSecret { get; set; }
        public Guid Tenant { get; set; }
        public bool UnobtrusiveJavaScriptEnabled { get; set; }
        public string UrlVaultAzure { get; set; }
        public string CertifiedName { get; set; }
        public bool esCertificate { get; set; }
        public NamePage NamePages { get; set;}
    }

    public class NamePage
    {
        public string CRC { get; set; }
        public string CEA { get; set; }
    }
}
