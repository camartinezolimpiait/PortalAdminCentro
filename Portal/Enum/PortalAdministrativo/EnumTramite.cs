using System.Collections.Generic;

namespace portalAdministrativoSISEC.Enum.PortalAdministrativo
{
	public enum EnumTramite
	{
		SinDefinir = 0,
		PrimeraVez = 1,
		Renovar = 2,
		Recategorizar = 3,
		PrimeraVezInstructor = 4,
		RecategorizarInstructor = 5,
	}

    public static class DescripcionesTramite
    {
        public static readonly Dictionary<EnumTramite, string> Descripcion = new()
    {
        { EnumTramite.SinDefinir, "Trámite no definido" },
        { EnumTramite.PrimeraVez, "Primera vez o licencia adicional" },
        { EnumTramite.Renovar, "Renovar licencia" },
        { EnumTramite.Recategorizar, "Recategorizar licencia" },
        { EnumTramite.PrimeraVezInstructor, "Nueva licencia de instructor" },
        { EnumTramite.RecategorizarInstructor, "Recategorizar licencia de instructor" }
    };
    }
}