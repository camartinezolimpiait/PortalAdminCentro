using System.Runtime.InteropServices;
using System;
using portalAdministrativoSISEC.Entidades.PowerBi;

namespace portalAdministrativoSISEC.Application.Contracts.PowerBi
{
    public interface IReporteService
    {
        EmbedParams ObtenerReporteEmbed(Guid idReporte, string IdRunt, string plataforma);
    }
}

