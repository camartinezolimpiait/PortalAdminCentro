using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Entidades.Agendamiento.Agenda
{
    public class AgendaEnrolar
    {
        [Required(ErrorMessage = "Debe proporcionar al menos un nombre")]
        [StringLength(100, ErrorMessage = "Nombres demasiado largo")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El Nombre solo puede contener letras y espacios.")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Debe proporcionar al menos un apellido")]
        [StringLength(100, ErrorMessage = "Apellidos demasiado largo.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de documento")]
        public int TipoDocumento { get; set; }

        [Required(ErrorMessage = "Debe proporcionar el número de documento")]
        [StringLength(20, ErrorMessage = "NumeroDocumento is too long.")]
        public string NumeroDocumento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un trámite")]
        public int? IdTramite { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        public int IdCategoria { get; set; }

        [StringLength(20, ErrorMessage = "Telefono is too long.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe proporcionar un corrre electrónico")]
        [EmailAddress(ErrorMessage = "Debe proporcionar un correo electrónico válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe proporcionar el Tipo de Cita")]
        public int IdTipoCita { get; set; }

        public string Categoria { get; set; }
        public int IdGenero { get; set; }
    }

    public class AgendaNuevaCita : AgendaEnrolar
    {
        public int IdMotivo { get; set; }
        public string Observaciones { get; set; }
    }

    public class Citas
    {
        public string Dia { get; set; }
        public string Hora { get; set; }
    }

}
