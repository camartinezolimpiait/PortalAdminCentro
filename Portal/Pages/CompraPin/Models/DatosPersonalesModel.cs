using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPin.Models
{
	public partial class DatosPersonalesModel
	{
        #region Variables
        [Required(ErrorMessage = "Ingrese el nombre del Aspirante")]
        [StringLength(255, ErrorMessage = "El nombre no debe tener más de 255 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
    ErrorMessage = "El nombre no puede tener caracteres especiales")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Ingrese el/los apellido(s) del Aspirante")]
        [StringLength(255, ErrorMessage = "El/los apellido(s) no deben tener más de 255 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
    ErrorMessage = "El/los apellido(s) no pueden tener caracteres especiales")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        public int? TipoDocumento { get; set; } = null;

        [Required(ErrorMessage = "Ingrese el número de documento")]
        [StringLength(15, ErrorMessage = "El número de documento no debe tener más de 15 caracteres.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El número de documento solo puede contener números.")]
        public string Documento { get; set; }

        [Required(ErrorMessage = "Ingrese el correo electrónico")]
        [StringLength(50, ErrorMessage = "El correo no debe tener más de 50 caracteres.")]
        [EmailAddress(ErrorMessage = "Email no está en formato válido")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Email no está en formato válido")]
        public string Correo { get; set; }


        [Required(ErrorMessage = "Ingrese el número de celular")]
        [StringLength(10, ErrorMessage = "El número de celular debe tener 10 dígitos.", MinimumLength = 10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Debe tener 10 dígitos, número de celular no válido")]
        public string Celular { get; set; }


        private string mensajeErrorDocumento;

        public bool EmisionOtraPersona { get; set; } = false;

        #endregion Variables
    }
}