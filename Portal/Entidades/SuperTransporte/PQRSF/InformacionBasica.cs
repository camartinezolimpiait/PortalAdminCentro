using System.ComponentModel.DataAnnotations;
using System;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.PQRSF
{
    public class InformacionBasica
    {
        [Required]
        public string tipo_id { get; set; }
        
        [Required(ErrorMessage = "El número de documento es requerido")]
        public string num_id { get; set; }
        
        [Required(ErrorMessage = "El primer nombre es requerido")]
        [RegularExpression("([a-zA-Z0-9ñÑáàâãéèêíïóôõöúüç\\s]+)",
        ErrorMessage = "Por favor, no introduzca caracteres especiales")]
        public string primer_nombre { get; set; }

        [RegularExpression("([a-zA-Z0-9ñÑáàâãéèêíïóôõöúüç\\s]+)",
        ErrorMessage = "Por favor, no introduzca caracteres especiales")]
        public string segundo_nombre { get; set; }
        
        [Required(ErrorMessage = "El Primer apellido es requerido")]
        [RegularExpression("([a-zA-Z0-9ñÑáàâãéèêíïóôõöúüç\\s]+)",
        ErrorMessage = "Por favor, no introduzca caracteres especiales")]
        public string primer_apellido { get; set; }
        
        [Required(ErrorMessage = "El segundo apellido es requerido")]
        [RegularExpression("([a-zA-Z0-9ñÑáàâãéèêíïóôõöúüç\\s]+)",
        ErrorMessage = "Por favor, no introduzca caracteres especiales")]
        public string segundo_apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Dirección de correo electrónico no válida")]
        public string email { get; set; }
        
        [Required(ErrorMessage = "El número de telefono es requerido")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", 
        ErrorMessage = "Por favor, introduzca un número de teléfono válido.")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El tipo de solicitud es requerido")]
        public string tipo_solicitud { get; set; }
        
        [Required(ErrorMessage = "Es necesario el detalle de la solicitud")]
        public string detalle_solicitud { get; set; }
    }
}
