using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using System.Collections.Generic;
using portalAdministrativoSISEC.Data.CompraPin;
namespace portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo
{
    public interface IOfuscamientoService
    {
       Task<string> Ofuscamiento(string data);
    }
}
