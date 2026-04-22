using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPin.Models
{
    public class PagoCuotasModel
    {
        [Required(ErrorMessage = "Debe seleccionar una cuota.")]
        public int? NumeroCuotas { get; set; }
    }
}
