using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.Models
{
	public class TipoVehiculoModel
	{
		[Required(ErrorMessage = "El campo tipo de vehiculo es  obligatorio.")]
		public int? seleccionTipoVehiculo;
	}
}
