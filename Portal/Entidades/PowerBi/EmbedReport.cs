using System;

namespace portalAdministrativoSISEC.Entidades.PowerBi
{
    public class EmbedReport
    {
        public Guid ReportId { get; set; }
        public string ReportName { get; set; }
        public string EmbedUrl { get; set; }
        public string NamePages { get; set; }
        public string Plataforma { get; set;}
    }
}
