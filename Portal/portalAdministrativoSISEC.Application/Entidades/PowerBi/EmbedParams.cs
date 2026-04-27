using System.Collections.Generic;
using System;
using Microsoft.PowerBI.Api.Models;

namespace portalAdministrativoSISEC.Entidades.PowerBi
{
    public class EmbedParams
    {
        public string Type { get; set; }
        public List<EmbedReport> EmbedReport { get; set; }
        public EmbedToken EmbedToken { get; set; }
        public Guid Filter { get; set; }
    }
}
