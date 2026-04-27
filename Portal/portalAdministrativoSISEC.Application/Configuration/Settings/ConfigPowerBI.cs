namespace portalAdministrativoSISEC.Entidades.Settings
{
    public class ConfigPowerBI
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string AuthorityUrl { get; set; } = string.Empty;
        public string EmbedUrlBase { get; set; } = string.Empty;
        public List<Reporte> Reportes { get; set; } = [];
        public string ResourceUrl { get; set; } = string.Empty;
    }

    public class Reporte
    {
        public string TipoReporte { get; set; } = string.Empty;
        public bool EsMasterUser { get; set; }
        public string ApplicationId { get; set; } = string.Empty;
        public string WorkspaceId { get; set; } = string.Empty;
        public string ReportId { get; set; } = string.Empty;
        public NamePages NamePages { get; set; } = new();
        public string ReportIdAdmin { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ApplicationSecret { get; set; } = string.Empty;
        public string Tenant { get; set; } = string.Empty;
        public bool UnobtrusiveJavaScriptEnabled { get; set; }
        public string UrlVaultAzure { get; set; } = string.Empty;
        public string CertifiedName { get; set; } = string.Empty;
        public bool EsCertificate { get; set; }
    }

    public class NamePages
    {
        public string CRC { get; set; } = string.Empty;
        public string CEA { get; set; } = string.Empty;
    }
}
