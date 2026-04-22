using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion
{
    public class ConfiguracionHorario
    {
        public long IdHorarioAtencion { get; set; }
        public long IdPerfil { get; set; }
        public int IdHorarioCita { get; set; }
        public int HoraApertura { get; set; }
        public int HoraCierre { get; set; }
        public int HorarioAperturaAgenda { get; set; }
        public int HorarioCierreAgenda { get; set; }
        public bool IsActivo { get; set; }
        public string Dia { get; set; }
        
        public int IdParametroHorario { get; set; }
        public DateTime FechaInicioParametrizacion { get; set; }
        public DateTime? FechaFinParametrizacion { get; set; }
        public int TotalParametrizacionAgenda { get; set; }
        public int TotalFuturasAgenda { get; set; }

        public Guid TransaccionGuid { get; set; }
        public bool IsVisible { get; set; }


        public ConfiguracionHorario()
        {
            IsActivo = true;
            TransaccionGuid = Guid.NewGuid();
        }
        public string ConvertirHoraInicio()
        {
            return ConvertirHora(HoraApertura);
        }
        public string ConvertirHoraFin()
        {
            return ConvertirHora(HoraCierre);
        }
        public string ConvertirHorarioAperturaAgenda()
        {
            return ConvertirHora(HorarioAperturaAgenda);
        }
        public string ConvertirHorarioCierreAgenda()
        {
            return ConvertirHora(HorarioCierreAgenda);
        }
        private string ConvertirHora(int horaObj)
        {
            string result = string.Empty;
            string hora = ConvertirIn16HoraMilitar(horaObj);
            if (!string.IsNullOrEmpty(hora))
            {

                DateTime horaConvert = DateTime.ParseExact(hora.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                result = horaConvert.ToString("HH:mm");
            }
            return result;
        }

        private string ConvertirIn16HoraMilitar(int hora)
        {
            string strHora = hora.ToString();
            if (!string.IsNullOrEmpty(strHora))
            {
                switch (strHora.Length)
                {
                    case 3:
                        return string.Concat("0", strHora[0], ":", strHora[1], strHora[2], ":00");
                        break;
                    case 4:
                        return string.Concat(strHora[0], strHora[1], ":", strHora[2], strHora[3], ":00");
                        break;
                    default:
                        return string.Empty;
                        break;
                }
            }
            return string.Empty;
        }
    }
}
