using portalAdministrativoSISEC.Util.Const.Facturacion;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models
{
    public class ConfigurarCredencialesModel
    {
        #region Properties

        [Required(ErrorMessage = "Ingrese el usuario registrado en el portal del proveedor tecnológico.")]
        [StringLength(15, ErrorMessage = "El usuario no debe tener más de 255 caracteres.")]
        [RegularExpression(FacturacionConst.USUARIO_REGEX, ErrorMessage = "El campo debe tener entre 1 y 15 caracteres alfanuméricos.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la contraseña registrada en el portal del proveedor tecnológico.")]
        [StringLength(50, ErrorMessage = "La contraseña no debe tener más de 50 caracteres.")]
        [RegularExpression(FacturacionConst.CLAVE_REGEX, ErrorMessage = "El campo debe tener al menos una mayúscula y un carácter especial.")]
        public string Clave { get; set; } = string.Empty;

        #endregion Properties
    }
}