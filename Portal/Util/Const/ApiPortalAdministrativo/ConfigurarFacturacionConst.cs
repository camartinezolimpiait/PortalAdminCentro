using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo
{
    public static class ConfigurarFacturacionConst
    {
        #region Descripcion Enums

        internal static readonly Dictionary<EnumEventoFacturacion, string> DescripcionEventoFacturacion = new()
        {
            { EnumEventoFacturacion.RecaudoPIN, "Recaudo del PIN"},
            { EnumEventoFacturacion.UsoPIN, "Uso del PIN"},
        };

        #endregion Descripcion Enums
    }
}