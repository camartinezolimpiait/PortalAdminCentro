using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
    public class InstructorDtoCEA<T>
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string tipo_doc { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_doc { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string primer_nombre { get; set; }
        public string segundo_nombre { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string primer_apellido { get; set; }
        public string segundo_apellido { get; set; }
        public CategoriasAutorizadas categorias_autorizadas { get; set; } = new CategoriasAutorizadas();
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_licencia_instructor { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public T licencia_instructor { get; set; }
        public string categoriasAutorizadas { get; set; } = "";
    }
}
