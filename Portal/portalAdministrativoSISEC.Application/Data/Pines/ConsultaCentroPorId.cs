using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Application.Data.Pines
{
    public class ConsultaCentroPorId
    {
        [RegularExpression(@"^([a-zA-Z0-9]{1,5})$", ErrorMessage = "Formato de Plataforma no válido")]
        [Required(ErrorMessage = "Plataforma es requerida")]
        public string? Plataforma { get; set; }

        [RegularExpression(@"^[0-9]{1,24}$", ErrorMessage = "Id inválido")]
        public int Id { get; set; }
    }
}

