using System.Globalization;
using System;
using System.Linq;

namespace portalAdministrativoSISEC.Util.Helpers
{
	public class DateTimeTransformations
	{
		public static string FormatShortDate(DateTime date)
		{
			// Retorna formato "8 de abril"
			return date.ToString("d 'de' MMMM", new CultureInfo("es-ES")).TrimStart('0');
		}

		public static string FormatHour(string timeString)
		{
			// Retorna formato "9:30 am"
			// Si el string está vacío o es nulo, retornamos vacío
			if (string.IsNullOrWhiteSpace(timeString))
				return string.Empty;

			try
			{
                DateTime time;

                if (DateTime.TryParse(timeString, out time))
                {
                    var resultHour2 = time.ToString("h:mm tt").ToLower();
					return time.ToString("h:mm tt").ToLower();
                }

                // Limpiamos el string de cualquier caracter no numérico
                string cleanTime = new string(timeString.Where(char.IsDigit).ToArray());

				// Aseguramos que tenga al menos 3 dígitos
				if (cleanTime.Length < 3)
					return string.Empty;

				// Extraemos las horas y minutos
				int hours = int.Parse(cleanTime.Substring(0, cleanTime.Length - 2));
				int minutes = int.Parse(cleanTime.Substring(cleanTime.Length - 2));

				// Creamos un DateTime con la hora
				time = DateTime.Today.AddHours(hours).AddMinutes(minutes);

                // Formateamos la hora en 12 horas con am/pm
                return time.ToString("h:mm tt").ToLower();
			}
			catch
			{
				return string.Empty;
			}
		}

		public static string FormatDay(DateTime date)
		{

			string dia = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetDayName(date.DayOfWeek);
			return char.ToUpper(dia[0]) + dia.Substring(1);
		}

		public static string FormatYear(DateTime date)
		{
			// Retorna el año
			return date.Year.ToString();
		}
	}
}
