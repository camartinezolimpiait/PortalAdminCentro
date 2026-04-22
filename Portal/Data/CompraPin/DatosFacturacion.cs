using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Data.CompraPin
{
    public class DatosFacturacion : IValidatableObject
    {
        public string ApellidosFacturacion { get; set; } = "";
        public string CorreoFacturacion { get; set; } = "";
        public string NombreComercialFacturacion { get; set; } = "";
        public string NombresFacturacion { get; set; } = "";
        public string NumeroIdentificacionFacturacion { get; set; } = "";
        public string RazonSocialFacturacion { get; set; } = "";
        public int TipoIdentificacionFacturacion { get; set; }
        public int TipoPersonaFacturacion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(NumeroIdentificacionFacturacion))
            {
                yield return new ValidationResult("El número de identificación es obligatorio.", [nameof(NumeroIdentificacionFacturacion)]);
            }

            // Persona Natural 
            if (TipoPersonaFacturacion == 2)
            {
                if (string.IsNullOrWhiteSpace(NombresFacturacion))
                    yield return new ValidationResult("El nombre es obligatorio.", [nameof(NombresFacturacion)]);

                if (string.IsNullOrWhiteSpace(ApellidosFacturacion))
                    yield return new ValidationResult("El apellido es obligatorio.", [nameof(ApellidosFacturacion)]);

                if (TipoIdentificacionFacturacion == 0 )
                {
                    yield return new ValidationResult("El tipo de identificación es obligatorio.", [nameof(TipoIdentificacionFacturacion)]);
                }

                if (!string.IsNullOrWhiteSpace(NumeroIdentificacionFacturacion) && TipoIdentificacionFacturacion > 0)
                {
                    ValidationResult validation = ValidarDocumento(NumeroIdentificacionFacturacion, TipoIdentificacionFacturacion);

                    if (validation != null)
                    {
                        yield return validation;
                    }
                }
            }

            // Persona Jurídica 
            if (TipoPersonaFacturacion == 1)
            {
                if (string.IsNullOrWhiteSpace(RazonSocialFacturacion))
                    yield return new ValidationResult("La razón social es obligatoria para persona jurídica.", new[] { nameof(RazonSocialFacturacion) });

                // Inicio código generado por GitHub Copilot
                if (string.IsNullOrWhiteSpace(NombreComercialFacturacion))
                    yield return new ValidationResult("El nombre comercial es obligatorio para persona jurídica.", new[] { nameof(NombreComercialFacturacion) });
                // Fin código generado por GitHub Copilot

                if (!string.IsNullOrWhiteSpace(NumeroIdentificacionFacturacion))
                {
                    ValidationResult validation = ValidarDocumento(NumeroIdentificacionFacturacion, 4);

                    if (validation != null)
                    {
                        yield return validation;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(CorreoFacturacion))
                yield return new ValidationResult("El correo es obligatorio.", [nameof(CorreoFacturacion)]);
        }

        private ValidationResult ValidarDocumento(string numeroIdentificacionFacturacion, int tipoIdentificacionFacturacion)
        {
            string pattern = string.Empty;
            string mensaje = string.Empty;

            switch (tipoIdentificacionFacturacion)
            {
                case 1: // Cédula de Ciudadanía
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case 2: // Cédula de Extranjería
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Cédula de Extranjería debe tener  6 dígitos y solo acepta números.";
                    break;

                case 3: // Tarjeta de Identidad
                    pattern = @"^[0-9]{6,11}$"; // Hasta 11 dígitos
                    mensaje = "La Tarjeta de Identidad debe tener entre 6 y 11 dígitos y solo acepta números.";
                    break;

                case 4: // Nit
                    pattern = @"^[0-9]{6,10}$"; // 6 o más dígitos
                    mensaje = "El NIT debe tener al menos 6 dígitos y solo acepta números.";
                    break;

                case 5: // Pasaporte
                    pattern = @"^[0-9a-zA-Z]{1,24}$"; // 6 o más caracteres alfanuméricos
                    mensaje = "El pasaporte debe tener hasta 24 caracteres";
                    break;

                case 10: // Contraseña Cédula de Ciudadanía
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Contraseña de la Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case 11: // Contraseña Cédula de Extranjería
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Contraseña de la Cédula de Extranjería debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case 13: // Permiso Protección Temporal (PPT)
                    pattern = @"^[0-9]{6,7}$"; // 6 a 7 dígitos
                    mensaje = "El Permiso por Protección Temporal debe tener entre 6 y 7 dígitos y solo acepta números.";
                    break;

                default:
                    break;
            }

            if (!string.IsNullOrEmpty(pattern) && !System.Text.RegularExpressions.Regex.IsMatch(numeroIdentificacionFacturacion, pattern))
            {
                return new ValidationResult(mensaje, [nameof(NumeroIdentificacionFacturacion)]);
            }
            return null;
        }
    }
}
