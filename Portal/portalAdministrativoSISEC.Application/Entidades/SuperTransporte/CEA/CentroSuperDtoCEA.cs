using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
    public class CentroSuperDtoCEA
    {
        public string motivos { get; set; }
        [Required]
        public bool completado { get; set; }
    }
}
