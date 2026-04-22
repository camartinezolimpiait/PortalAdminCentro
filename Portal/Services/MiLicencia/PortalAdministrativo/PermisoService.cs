using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
            if (plataforma == 2) // CEA
            {
                var listadoConveniosCEA = await _miLicenciaService.ObtenerConveniosCEA(
                    new ConsultaCentoId { IdCentro = idCentro }
                );
                return listadoConveniosCEA != null && listadoConveniosCEA.Any(c => c.IdOrigenPin == 4);
            }
            else if (plataforma == 1) // CRC
            {
                return true;
                var listadoConveniosCRC = await _miLicenciaService.ObtenerConveniosCRC(
                    new ConsultaCentoId { IdCentro = idCentro }
                );
                return listadoConveniosCRC != null && listadoConveniosCRC.Any(c => c.IdOrigenPin == 5);
            }

            return false;
        }

    }
}
