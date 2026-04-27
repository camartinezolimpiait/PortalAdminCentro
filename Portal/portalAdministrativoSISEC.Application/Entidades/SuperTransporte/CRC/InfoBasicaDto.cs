using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
    public class InfoBasicaDto
    {
        [Required]
        [Range(1, 500, ErrorMessage = "Debe colocar un valor entre 1 y 500")]
        public int capacidad_certificados { get; set; }
        [Required]
        [Range(1, 24, ErrorMessage = "Debe colocar un valor entre 1 y 24")]
        public int horas_de_atencion_por_dia { get; set; }
    }
}
