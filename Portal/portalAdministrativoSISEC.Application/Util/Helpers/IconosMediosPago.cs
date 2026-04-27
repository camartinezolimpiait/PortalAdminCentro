using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Util.Helpers
{
    public static class IconosMediosPago
    {
        public static readonly Dictionary<int, string> Urls = new()
    {
        { (int)EnumTipoPago.PSE, "/images/iconos/pse.png" },
        { (int)EnumTipoPago.BancolombiaWompi, "/images/iconos/bancolombia.png" },
        { (int)EnumTipoPago.PinDirecto, "/img/iconos/davibank.png" },
        { (int)EnumTipoPago.PSEColpatria, "/img/iconos/pse-seeklogo.png" },
    };
    }
}
