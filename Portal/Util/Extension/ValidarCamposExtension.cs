using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace portalAdministrativoSISEC.Util.Extension
{
    public static class ValidarCamposExtension
    {
        // Inicio código generado por GitHub Copilot
        public static IEnumerable<ValidationResult> ValidarDocumento(this string documento, EnumTipoDocumento tipoDocumento)
        {
            string pattern = string.Empty;
            string mensaje = string.Empty;

            switch (tipoDocumento)
            {
                case EnumTipoDocumento.CedulaCiudadania:
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case EnumTipoDocumento.CedulaExtranjeria:
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Cédula de Extranjería debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case EnumTipoDocumento.TarjetaIdentidad:
                    pattern = @"^[0-9]{6,11}$"; // 6 a 11 dígitos
                    mensaje = "La Tarjeta de Identidad debe tener entre 6 y 11 dígitos y solo acepta números.";
                    break;

                case EnumTipoDocumento.Nit:
                    pattern = @"^[0-9]{6,10}$"; // 6 o más dígitos
                    mensaje = "El NIT debe tener al menos 6 dígitos y solo acepta números.";
                    break;

                case EnumTipoDocumento.Pasaporte:
                    pattern = @"^[0-9a-zA-Z]{6,24}$"; // 6 o más caracteres alfanuméricos
                    mensaje = "El Pasaporte debe tener al menos 6 caracteres y acepta letras y números.";
                    break;

                case EnumTipoDocumento.ContrasenaCedulaCiudadania:
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Contraseña de la Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case EnumTipoDocumento.ContrasenaCedulaExtranjeria:
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Contraseña de la Cédula de Extranjería debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case EnumTipoDocumento.PermisoPorProteccionTemporal:
                    pattern = @"^[0-9]{6,7}$"; // 6 a 7 dígitos
                    mensaje = "El Permiso por Protección Temporal debe tener entre 6 y 7 dígitos y solo acepta números.";
                    break;

                default:
                    // Si no hay un patrón definido para el tipo de documento, invalidamos por defecto
                    mensaje = "Tipo de documento no reconocido.";
                    break;
            }

            if (!string.IsNullOrEmpty(pattern) && !Regex.IsMatch(documento, pattern))
            {
                yield return new ValidationResult(
                    mensaje,
                    new[] { nameof(documento) }
                );
            }
            else if (string.IsNullOrEmpty(pattern))
            {
                yield return new ValidationResult(
                    mensaje,
                    new[] { nameof(documento) }
                );
            }
        }
        // Fin codigo generado por GitHub Copilot
        public static string CamelCase(this string texto)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(texto);
        }

        public static string FormatAsCurrency(this decimal valor)
        {
            CultureInfo culture = new("es-CO");
            culture.NumberFormat.CurrencyDecimalDigits = 0;

            return string.Format(culture, "{0:C0}", valor);
        }
    }
}
