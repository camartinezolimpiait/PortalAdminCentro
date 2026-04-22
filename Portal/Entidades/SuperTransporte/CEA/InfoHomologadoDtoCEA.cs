using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
    public class InfoHomologadoDtoCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_contrato { get; set; }
        [Required]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_contrato { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_contrato { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_contrato { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile contrato { get; set; }
        public string url_adjunto_contrato { get; set; }
        public string nombre_adjunto_contrato { get; set; }

        public string fecha_contrato()
        {
            return $"{anio_contrato}-{mes_contrato.ToString().PadLeft(2,'0')}-{dia_contrato.ToString().PadLeft(2, '0')}";
        }

        public void setFecha(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_contrato = int.Parse(fechaSplit[0]);
                mes_contrato = int.Parse(fechaSplit[1]);
                dia_contrato = int.Parse(fechaSplit[2]);
            }
        }
    }
    public class InfoHomologadoClsCEA<T>
    {
        public string num_contrato { get; set; }
        public string fecha_contrato { get; set; }
        public T contrato { get; set; }
    }
    public class InfoHomologadoDtoRequestCEA<T>
    {
        public InfoHomologadoClsCEA<T> homologado { get; set; }
    }
}
