using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia.PortalAdministrativo;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo
{
    public class PermisoService : IPermisoService
    {
        private GetDataResponseCentro getCentroResponse = new GetDataResponseCentro();
        private readonly IMiLicenciaService _miLicenciaService;
        public List<ConvenioCentro> listadoConvenios = new List<ConvenioCentro>();
        ProtectedSessionStorage ProtectedSessionStore { get; set; }

        public PermisoService(IMiLicenciaService miLicenciaService)
        {
            _miLicenciaService = miLicenciaService;
        }

        public async Task<bool> TienePermisoParaCompraPin(int idCentro, int plataforma)
        {
            if (plataforma == 2)
            {
                var listadoConveniosCEA = await _miLicenciaService.ObtenerConveniosCEA(
                    new ConsultaCentoId { IdCentro = idCentro });
                return listadoConveniosCEA != null && listadoConveniosCEA.Any(c => c.IdOrigenPin == 4);
            }

            if (plataforma == 1)
            {
                return true;
#pragma warning disable CS0162
                var listadoConveniosCRC = await _miLicenciaService.ObtenerConveniosCRC(
                    new ConsultaCentoId { IdCentro = idCentro });
                return listadoConveniosCRC != null && listadoConveniosCRC.Any(c => c.IdOrigenPin == 5);
#pragma warning restore CS0162
            }

            return false;
        }
    }
}


