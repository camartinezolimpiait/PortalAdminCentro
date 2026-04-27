using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Util.Const
{
	public static class CredencialesConst
	{
		internal static readonly Dictionary<EnumCredenciales, string> DescripcionCredenciales = new()
		{
			{ EnumCredenciales.ClientId, "Prueba"},
			{ EnumCredenciales.ClientSecret, "123"}
			 
		};
	}
	 
}
