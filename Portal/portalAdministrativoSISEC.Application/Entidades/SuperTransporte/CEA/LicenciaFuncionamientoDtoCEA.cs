using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
	public class LicenciaFuncionamientoDtoCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string acto_administrativo { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string secretaria_edu_expedicion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_radicado_actualizacion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string secretaria_edu_actualizacion { get; set; }
        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_fecha_acto_expedicion { get; set; }
        [Required]
        [Range(1,12,
        ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_fecha_acto_expedicion { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_fecha_acto_expedicion { get; set; }
		[Required]
		[Range(1, 31,
		ErrorMessage = "El dia ingresado no es correcto.")]
		public int dia_fecha_radicado{ get; set; }
		[Required]
		[Range(1, 12,
		ErrorMessage = "El mes ingresado no es correcto.")]
		public int mes_fecha_radicado { get; set; }
		[Required]
		[Range(1900, 3000,
		ErrorMessage = "El año ingresado no es correcto.")]
		public int anio_fecha_radicado { get; set; }

        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile acto_administrativo_doc { get; set; }
        public string url_adjunto_acto_administrativo { get; set; }
        public string nombre_adjunto_acto_administrativo { get; set; }

        public string getFechaActoExpedicion() {
            return $"{anio_fecha_acto_expedicion}-{mes_fecha_acto_expedicion.ToString().PadLeft(2,'0')}-{dia_fecha_acto_expedicion.ToString().PadLeft(2, '0')}";
		}

        public void setFechaActoExpedicion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha)) {
                var fechaSplit = fecha.Split('-');
				anio_fecha_acto_expedicion = int.Parse(fechaSplit[0]);
				mes_fecha_acto_expedicion = int.Parse(fechaSplit[1]);
				dia_fecha_acto_expedicion = int.Parse(fechaSplit[2]);
            }
        }

		public string getFechaRadicado()
		{
			return $"{anio_fecha_radicado}-{mes_fecha_radicado.ToString().PadLeft(2, '0')}-{dia_fecha_radicado.ToString().PadLeft(2, '0')}";
		}

		public void setFechaRadicado(string fecha)
		{
			if (!string.IsNullOrEmpty(fecha))
			{
				var fechaSplit = fecha.Split('-');
				anio_fecha_radicado = int.Parse(fechaSplit[0]);
				mes_fecha_radicado = int.Parse(fechaSplit[1]);
				dia_fecha_radicado = int.Parse(fechaSplit[2]);
			}
		}
	}
    public class LicenciaFuncionamientoClsCEA<T>
    {
        public string acto_administrativo { get; set; }
        public string fecha_acto_expedicion { get; set; }
        public string secretaria_edu_expedicion { get; set; }
        public string num_radicado_actualizacion { get; set; }
        public string fecha_radicado { get; set; }
        public string secretaria_edu_actualizacion { get; set; }
        public T acto_administrativo_doc { get; set; }
    }

    public class LicenciaFuncionamientoDtoRequestCEA<T>
    {
        public LicenciaFuncionamientoClsCEA<T> licencia_funcionamiento { get; set; }
    }

}
