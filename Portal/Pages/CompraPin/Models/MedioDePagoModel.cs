using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.CompraPin.Models
{
    public class MedioDePagoModel
    {
        [Required(ErrorMessage = "Debe seleccionar un medio de pago.")]
        public int? TipoRecaudoCtrl { get; set; }
    }
}
