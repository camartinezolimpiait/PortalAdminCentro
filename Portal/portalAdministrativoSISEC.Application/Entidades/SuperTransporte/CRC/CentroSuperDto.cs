using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
    public class CentroSuperDto
    {
        public string motivos { get; set; }
        [Required]
        public bool completado { get; set; }
    }
}
