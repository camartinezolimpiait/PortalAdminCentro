using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace portalAdministrativoSISEC.Util.Extension
{
    public static partial class StringExtension
    {
        #region Properties

        [GeneratedRegex(@"--.*?$", RegexOptions.Multiline)]
        private static partial Regex SqlCommentsRegex();

        [GeneratedRegex(@";\s*--.*?$", RegexOptions.Multiline)]
        private static partial Regex SqlTerminatorsRegex();

        [GeneratedRegex(@"[^\w\s.,@/-]")]
        private static partial Regex InvalidCharactersRegex();

        #endregion Properties

        #region Public Methods

        public static string SerializeToJson<T>(this T objeto) => JsonConvert.SerializeObject(objeto);

        public static string SanitizeString(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Escapa comillas simples
            string sanitized = text.Replace("'", "''");

            // Elimina comentarios SQL
            sanitized = RemoveSqlComments(sanitized);

            // Elimina patrones peligrosos como terminadores de consulta
            sanitized = RemoveSqlTerminators(sanitized);

            // Remueve caracteres no deseados
            sanitized = RemoveInvalidCharacters(sanitized);

            return sanitized.Trim(); // Retorna el texto limpio
        }

        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text= text.ToLowerInvariant();
            return char.ToUpper(text[0]) + text[1..];
        }

        public static string ToCamelCase(this string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
        }

        #endregion Public Methods

        #region Private Methods

        private static string RemoveSqlComments(string input) => SqlCommentsRegex().Replace(input, string.Empty);

        private static string RemoveSqlTerminators(string input) => SqlTerminatorsRegex().Replace(input, string.Empty);

        private static string RemoveInvalidCharacters(string input) => InvalidCharactersRegex().Replace(input, string.Empty);

        #endregion Private Methods
    }
}
