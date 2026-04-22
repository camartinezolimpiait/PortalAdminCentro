using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPin.Models
{
    public partial class DatosBasicosModel
	{
		#region Atributos

		[Required(ErrorMessage = "El día es obligatorio.")]
		[RegularExpression(@"^(?:[1-9]|1\d|2[0-9]|3[0-1])$", ErrorMessage = "El día no es válido.")]
		public int? Dia { get; set; }

		[Required(ErrorMessage = "El mes es obligatorio.")]
		[RegularExpression(@"^(?:[1-9]|1[0-2])$", ErrorMessage = "El mes no es válido.")]
		public int? Mes { get; set; }

		[Required(ErrorMessage = "El año es obligatorio.")]
		[RegularExpression(@"^(18[2-9]\d{2}|19\d{2}|20\d{2}|21\d{2}|22[0-2]\d)$", ErrorMessage = "El año no es válido.")]
		public int? Anio { get; set; }

		[Required(ErrorMessage = "El campo sexo es obligatorio.")]
		public int? Sexo { get; set; }

		#endregion Atributos
	}
}