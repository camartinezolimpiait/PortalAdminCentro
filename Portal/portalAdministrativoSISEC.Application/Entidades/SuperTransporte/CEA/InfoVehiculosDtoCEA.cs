using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
	public class InfoVehiculosDtoCEA<T>
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string placa { get; set; }
		
		public CategoriasAutorizadas categorias_autorizadas { get; set; } = new CategoriasAutorizadas();
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string modelo { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [ExtensionFileValid]
        public T licencia_de_transito { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string clase_de_vehiculo { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public int num_tarjeta_servicio { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string dt_que_expide_ts { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [ExtensionFileValid]
        public T copia_tarjeta_servicio { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public bool ha_estado_en_siniestro { get; set; }

        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_fecha_expedicion_ts { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_fecha_expedicion_ts { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_fecha_expedicion_ts { get; set; }
        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_fecha_vencimiento_ts { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_fecha_vencimiento_ts { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_fecha_vencimiento_ts { get; set; }
        public List<SiniestroClases> detalle_siniestros { get; set; }
        public string categoriasAutorizadas { get; set; } = "";
        public string detalleSiniestros { get; set; } = "";
        public string fecha_expedicion_ts { get; set; } = "";
        public string fecha_vencimiento_ts { get; set; } = "";

        public string getFechaExpedicionTs()
        {
            return $"{anio_fecha_expedicion_ts}-{mes_fecha_expedicion_ts.ToString().PadLeft(2, '0')}-{dia_fecha_expedicion_ts.ToString().PadLeft(2, '0')}";
        }

        public void setFechaExpedicionTs(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_fecha_expedicion_ts = int.Parse(fechaSplit[0]);
                mes_fecha_expedicion_ts = int.Parse(fechaSplit[1]);
                dia_fecha_expedicion_ts = int.Parse(fechaSplit[2]);
            }
        }

        public string getFechaVencimientoTs()
        {
            return $"{anio_fecha_vencimiento_ts}-{mes_fecha_vencimiento_ts.ToString().PadLeft(2, '0')}-{dia_fecha_vencimiento_ts.ToString().PadLeft(2, '0')}";
        }

        public void setFechaVencimientoTs(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_fecha_vencimiento_ts = int.Parse(fechaSplit[0]);
                mes_fecha_vencimiento_ts = int.Parse(fechaSplit[1]);
                dia_fecha_vencimiento_ts = int.Parse(fechaSplit[2]);
            }
        }
    }

    public class SiniestroClases
    {
        public string id_siniestro { get; set; } = "";
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public bool fue_durante_clase { get; set; }
    }

}
