using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util
{
    public static class DateTimeExtension
    {
        public static string ToResultadoBusqueda(this DateTime dt)
        {
            CultureInfo ci = new CultureInfo("es-ES");
            var monthDate = dt.ToString("MMMM", ci);
            var day = dt.ToString("dddd", ci);
            return string.Format("{0}, {1} {2} de {3}", string.Concat(char.ToUpper(day[0]), day.Substring(1)), dt.Day, monthDate, dt.Year);
        }

        public static string GetDiaString(this DateTime dt)
        {
            CultureInfo ci = new CultureInfo("es-ES");
            return dt.ToString("dddd", ci);
        }

        public static string GetMesString(this DateTime dt)
        {
            CultureInfo ci = new CultureInfo("es-ES");
            return dt.ToString("MMMM", ci);
        }
        public static int GetindicelistDates(this string day)
        {
            int result = 0;
            string[] arrayDays = { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo" };

            for (int i = 0; i <= arrayDays.Length-1; i++)
            {
                result = i;
                if (arrayDays[i] == day)
                {
                    break;
                }
            }
            return result;
        }
    }
}
