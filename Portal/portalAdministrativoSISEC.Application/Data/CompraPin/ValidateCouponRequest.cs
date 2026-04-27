// Inicio c�digo generado por GitHub Copilot
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Application.Data.CompraPin
{
    /// <summary>
    /// Modelo de solicitud para validar cupones de convenio
    /// </summary>
    public class ValidateCouponRequest
 {
        /// <summary>
        /// Nombre del convenio asociado al cup�n
        /// </summary>
        [Required(ErrorMessage = "El nombre del convenio es obligatorio.")]
    [StringLength(15, MinimumLength = 1, ErrorMessage = "El nombre del convenio debe tener entre 1 y 15 caracteres.")]
     [RegularExpression("^[A-Z0-9]{1,15}$", ErrorMessage = "El nombre del convenio solo permite caracteres alfanum�ricos en may�sculas y m�ximo 15 caracteres.")]
      public string NombreConvenio { get; set; }

     /// <summary>
        /// Tipo de negocio del convenio (valores permitidos: 1-10)
      /// </summary>
        [Required(ErrorMessage = "El tipo de negocio es obligatorio.")]
        [Range(1, 10, ErrorMessage = "El tipo de negocio debe ser un n\u00famero entre 1 y 10.")]
        public int TipoNegocio { get; set; }
    }
}
// Fin c�digo generado por GitHub Copilot

