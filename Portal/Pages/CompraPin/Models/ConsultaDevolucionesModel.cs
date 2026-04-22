using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Util.Extension;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace portalAdministrativoSISEC.Pages.CompraPin.Models
{
    public partial class ConsultaDevolucionesModel : IValidatableObject
    {
        #region Atributos

        [Required(ErrorMessage = "El número del PIN es obligatorio.")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "El PIN no tiene el formato válido.")]
        [StringLength(15, ErrorMessage = "El PIN no tiene la longitud esperada.", MinimumLength = 10)]
        public string Pin { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        public int? TipoDocumento { get; set; } = null;

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        public string Documento { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoDocumento > 0 && !string.IsNullOrEmpty(Documento))
            {
                IEnumerable<ValidationResult> response = ValidarCamposExtension.ValidarDocumento(Documento, (EnumTipoDocumento)TipoDocumento);

                if (response != null && response.Any())
                {
                    foreach (var error in response)
                    {
                        yield return error;
                    }
                }
            }
        }
        #endregion Atributos
    }
}