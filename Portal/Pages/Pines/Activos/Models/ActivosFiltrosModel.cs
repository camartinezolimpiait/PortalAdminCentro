using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.Pines.Activos.Models
{
    // Inicio código generado por GitHub Copilot
    public class ActivosFiltrosModel : IValidatableObject
    {
        // Inicio refactorización/optimización por GitHub Copilot
        private static readonly IReadOnlyDictionary<int, (string Pattern, string Mensaje)> ReglasDocumentoPorTipo =
            new Dictionary<int, (string Pattern, string Mensaje)>
            {
                [1] = (@"^[0-9]{6,10}$", "La Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números."),
                [2] = (@"^[0-9]{6,10}$", "La Cédula de Extranjería debe tener entre 6 y 10 dígitos y solo acepta números."),
                [3] = (@"^[0-9]{6,10}$", "La Tarjeta de Identidad debe tener entre 6 y 10 dígitos y solo acepta números."),
                [4] = (@"^[0-9]{6,10}$", "El NIT debe tener al menos 6 dígitos y solo acepta números."),
                [5] = (@"^[0-9a-zA-Z]{1,24}$", "El pasaporte debe tener hasta 24 caracteres."),
                [10] = (@"^[0-9]{6,10}$", "La Contraseña de la Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números."),
                [11] = (@"^[0-9]{6,10}$", "La Contraseña de la Cédula de Extranjería debe tener entre 6 y 10 dígitos y solo acepta números."),
                [13] = (@"^[0-9]{6,7}$", "El Permiso por Protección Temporal debe tener entre 6 y 7 dígitos y solo acepta números."),
            };
        // Fin refactorización/optimización por GitHub Copilot

        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }

        [StringLength(24, ErrorMessage = "El PIN debe tener hasta 24 caracteres.")]
        public string? Pin { get; set; }
        public int CanalVenta { get; set; }
        public int EstadoPin { get; set; }
        public int? AliadoRecaudo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Documento))
            {
                yield break;
            }

            if (!int.TryParse(TipoDocumento, out var tipoDoc))
            {
                yield return new ValidationResult(
                    "Seleccione un tipo de documento para validar el número de documento.",
                    new[] { nameof(Documento) });
                yield break;
            }

            if (!ReglasDocumentoPorTipo.TryGetValue(tipoDoc, out var regla))
            {
                yield break;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(Documento, regla.Pattern))
            {
                yield return new ValidationResult(regla.Mensaje, new[] { nameof(Documento) });
            }
        }
    }
    // Fin código generado por GitHub Copilot
}
