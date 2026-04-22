using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class UpdateElectronicBillingRequest
    {
        #region Properties

        [Required(ErrorMessage = "El IdRunt es requerido.")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "El IdRunt debe ser mínimo 1 y máximo 30 caracteres numéricos.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El IdRunt solo debe contener caracteres numéricos.")]
        public string IdRunt { get; set; }

        [Required(ErrorMessage = "Los Pines son requeridos.")]
        [MinLength(1, ErrorMessage = "Debe especificar al menos un PIN.")]
        public List<string> Pines { get; set; }

        [Required(ErrorMessage = "El UsuarioPortal es requerido.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El UsuarioPortal debe tener entre 3 y 100 caracteres.")]
        public string UsuarioPortal { get; set; }

        #endregion Properties
    }
}