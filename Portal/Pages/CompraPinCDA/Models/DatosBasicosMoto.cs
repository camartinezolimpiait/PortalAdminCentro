using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.Models
{
    public class DatosBasicosMoto
    {
        [Required(ErrorMessage = "El campo placa es obligatorio.")]
        [RegularExpression(@"^[A-Za-z]{3}\d{2}[A-Za-z]?$", ErrorMessage = "El formato de la placa no es válido.")]
        public string? nombrePlacaMoto { get; set; }
    }
}
