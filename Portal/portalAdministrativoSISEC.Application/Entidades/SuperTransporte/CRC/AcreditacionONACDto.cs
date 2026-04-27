using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
    public class AcreditacionONACDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo no puede estar vacío.")]
        [StringLength(1, ErrorMessage = "El campo no puede estar vacío.")]
        public string estado_acreditacion_onac { get; set; }
    }
}
