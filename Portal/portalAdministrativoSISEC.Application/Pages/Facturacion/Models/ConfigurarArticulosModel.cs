using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models
{
    public class ConfigurarArticulosModel
    {
        #region Properties

        public bool AplicaConfiguracionEspecifica { get; set; } = false;

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Tarifa ANSV' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Tarifa ANSV' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Tarifa ANSV' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string TarifaAnsv { get; set; } = "803";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Tarifa del SICOV' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Tarifa del SICOV' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Tarifa del SICOV' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string TarifaSicov { get; set; } = "804";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Tarifa del Aliado de recaudo' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Tarifa del Aliado de recaudo' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Tarifa del Aliado de recaudo' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string TarifaAliado { get; set; } = "805";

        #region CEA

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccion { get; set; } = "801";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción A1' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción A1' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción A1' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionA1 { get; set; } = "701";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción A2' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción A2' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción A2' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionA2 { get; set; } = "702";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción B1' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción B1' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción B1' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionB1 { get; set; } = "703";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción B2' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción B2' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción B2' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionB2 { get; set; } = "704";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción B3' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción B3' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción B3' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionB3 { get; set; } = "705";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción C1' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción C1' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción C1' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionC1 { get; set; } = "706";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción C2' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción C2' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción C2' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionC2 { get; set; } = "707";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción C3' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción C3' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción C3' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionC3 { get; set; } = "708";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de conducción RC1' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de conducción RC1' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de conducción RC1' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionRC1 { get; set; } = "709";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionInstructor { get; set; } = "802";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción A1' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción A1' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción A1' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIA1 { get; set; } = "710";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción A2' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción A2' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción A2' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIA2 { get; set; } = "711";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción B1' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción B1' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción B1' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIB1 { get; set; } = "712";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción B2' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción B2' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción B2' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIB2 { get; set; } = "713";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción B3' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción B3' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción B3' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIB3 { get; set; } = "714";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción C1' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción C1' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción C1' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIC1 { get; set; } = "715";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción C2' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción C2' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción C2' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIC2 { get; set; } = "716";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Curso de instructor en conducción C3' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Curso de instructor en conducción C3' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Curso de instructor en conducción C3' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string CursoConduccionIC3 { get; set; } = "717";

        #endregion CEA

        #region CRC

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Examen médico' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Examen médico' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Examen médico' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string ExamenMedico { get; set; } = "801";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Examen médico sencillo' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Examen médico sencillo' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Examen médico sencillo' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string ExamenMedicoSencillo { get; set; } = "701";

        [Required(ErrorMessage = "Ingrese el parámetro del articulo 'Examen médico Combo' del centro.")]
        [StringLength(10, ErrorMessage = "El articulo 'Examen médico Combo' no puede tener mas de 10 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{3,10}$", ErrorMessage = "El campo 'Examen médico Combo' debe ser alfanumérico sin espacios, solo se permite (-) y (_) y debe tener entre 1 y 10 caracteres.")]
        public string ExamenMedicoCombo { get; set; } = "702";

        #endregion CRC

        #endregion Properties
    }
}