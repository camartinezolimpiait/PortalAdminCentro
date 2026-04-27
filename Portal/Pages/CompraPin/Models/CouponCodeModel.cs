// Inicio código generado por GitHub Copilot
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPin.Models
{
    /// <summary>
    /// Modelo para el formulario de código de cupón/referido
 /// </summary>
    public class CouponCodeModel
    {
        /// <summary>
 /// Código de referido ingresado por el usuario
        /// </summary>
  [Required(ErrorMessage = "El código de referido es obligatorio")]
    [StringLength(15, MinimumLength = 1, ErrorMessage = "El código debe tener entre 1 y 15 caracteres")]
        [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "El código solo permite caracteres alfanuméricos en mayúsculas")]
        public string ReferralCode { get; set; }
    }
}
// Fin código generado por GitHub Copilot
