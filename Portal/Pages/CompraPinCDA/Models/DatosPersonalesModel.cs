using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.Models
{
	public partial class DatosPersonalesModel
	{
		#region Variables

		[Required(ErrorMessage = "El correo es obligatorio.")]
		[EmailAddress(ErrorMessage = "Debe ser una dirección de correo válida.")]
		[RegularExpression(@"[A-z0-9._%+-]+@[A-z0-9.-]+\.[A-z]{2,3}$", ErrorMessage = "Verifique su dirección de correo electronico")]
		public string Correo { get; set; }

		[Required(ErrorMessage = "El número de celular es obligatorio.")]
		[Phone(ErrorMessage = "Debe ser un número de celular válido.")]
		[StringLength(10, ErrorMessage = "El número de celular debe tener 10 dígitos.", MinimumLength = 10)]
		[RegularExpression(@"^3\d*$", ErrorMessage = "El número de celular debe comenzar con el numero 3 .")]
		public string Celular { get; set; }

		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(200, ErrorMessage = "El nombre no debe tener mas de 200 letras.")]
		[RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ,.\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
		public string Nombre { get; set; }

		[Required(ErrorMessage = "Los apellidos son obligatorios.")]
		[StringLength(200, ErrorMessage = "El apellido no debe tener mas de 200 letras.")]
		[RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ,.\s]+$", ErrorMessage = "Los apellidos solo pueden contener letras.")]
		public string Apellidos { get; set; }

		[Required(ErrorMessage = "El tipo de documento es obligatorio.")]
		public int? TipoDocumento { get; set; } = null;

		[Required(ErrorMessage = "El número de documento es obligatorio.")]
		public string Documento { get; set; }

		private string mensajeErrorDocumento;

		#endregion Variables
	}
}