using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class DatosPersonaDTO
    {
        public int IdDatosPersonas { get; set; }

        [Required(ErrorMessage = "Nombres es requerido")]
        [StringLength(100, ErrorMessage = "Nombres es muy largo")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Apellidos es requerido")]
        [StringLength(100, ErrorMessage = "Apellidos es muy largo")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Tipo de identificación es requerido")]
        public int IdTipoIdentificacion { get; set; }

        [Required(ErrorMessage = "Numero de identificación es requerido")]
        [StringLength(100, ErrorMessage = "El numero de identificación es muy largo")]
        public string NumeroIdentificacion { get; set; }

        [Required(ErrorMessage = "Numero de teléfono es requerido")]
        [StringLength(100, ErrorMessage = "El numero de teléfono es muy largo")]
        public string NumeroTelefono { get; set; }

        [Required(ErrorMessage = "Correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El genero es requerido")]
        public int IdGenero { get; set; }
    }
}

