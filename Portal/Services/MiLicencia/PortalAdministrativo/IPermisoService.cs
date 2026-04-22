using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using System.Collections.Generic;
using portalAdministrativoSISEC.Data.CompraPin;
namespace portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo
{
	public interface IPermisoService
	{
		Task<bool>TienePermisoParaCompraPin(int idCentro, int plataforma);
	}
}
