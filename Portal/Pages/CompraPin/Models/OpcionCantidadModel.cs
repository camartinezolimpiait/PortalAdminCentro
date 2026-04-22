using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPin.Models
{
    public class OpcionCantidadModel
    {
        [Required(ErrorMessage = "El tipo de trámite es obligatorio.")]
        public int? Cantidad { get; set; }
    }
}
