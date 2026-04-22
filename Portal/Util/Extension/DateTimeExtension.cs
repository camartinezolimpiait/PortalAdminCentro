using System;

namespace portalAdministrativoSISEC.Util.Extension
{
    public static class DateTimeExtension
    {
        #region Private Methods

        // Inicio refactorización/optimización por GitHub Copilot
        /// <summary>
        /// Método generado por GitHub Copilot
        /// Devuelve una cadena representando el tiempo transcurrido desde la fecha especificada hasta el momento de referencia.
        /// </summary>
        /// <param name="time">Fecha de inicio.</param>
        /// <param name="reference">Fecha de referencia para el cálculo. Si es null, se usa DateTime.UtcNow.</param>
        /// <returns>Cadena con el tiempo transcurrido.</returns>
        public static string GetTimeAgo(this DateTime time, DateTime? reference = null)
        {
            // Inicio código generado por GitHub Copilot
            var now = reference ?? DateTime.UtcNow;
            var diff = now - time;

            if (diff.TotalSeconds < 60)
                return $"{Math.Floor(diff.TotalSeconds)} seg";
            else if (diff.TotalMinutes < 60)
                return $"{Math.Floor(diff.TotalMinutes)} min";
            else if (diff.TotalHours < 24)
                return $"{Math.Floor(diff.TotalHours)} h";
            else
                return $"{Math.Floor(diff.TotalDays)} día{(diff.TotalDays >= 2 ? "s" : "")}";
            // Fin código generado por GitHub Copilot
        }

        // Fin refactorización/optimización por GitHub Copilot

        #endregion Private Methods
    }
}