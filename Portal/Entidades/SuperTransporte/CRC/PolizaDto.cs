using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
    public class PolizaDto
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_poliza { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string nombre_aseguradora { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El valor debe ser numérico.")]
        [Range(0, long.MaxValue, ErrorMessage = "El valor no puede ser negativo.")]
        public long valor_asegurado { get; set; }
        public bool ha_presentado_reclamacion { get; set; }
        public string motivo_reclamacion { get; set; }
        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es válido.")]
        public int dia_fecha_expedicion { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es válido.")]
        public int mes_fecha_expedicion { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es válido.")]
        public int anio_fecha_expedicion { get; set; }
        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es válido.")]
        public int dia_fecha_vencimiento { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es válido.")]
        public int mes_fecha_vencimiento { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es válido.")]
        public int anio_fecha_vencimiento { get; set; }

        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile poliza_vigente { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile soporte_pago { get; set; }
        public string url_poliza_adjunto { get; set; }
        public string url_soporte_pago_adjunto { get; set; }
        public string nombre_poliza_adjunto { get; set; }
        public string nombre_soporte_pago_adjunto { get; set; }

        public string getFechaExpedicion()
        {
            return $"{anio_fecha_expedicion}-{mes_fecha_expedicion.ToString().PadLeft(2, '0')}-{dia_fecha_expedicion.ToString().PadLeft(2, '0')}";
        }

        public void setFechaFechaExpedicion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_fecha_expedicion = int.Parse(fechaSplit[0]);
                mes_fecha_expedicion = int.Parse(fechaSplit[1]);
                dia_fecha_expedicion = int.Parse(fechaSplit[2]);
            }
        }

        public string getFechaVencimiento()
        {
            return $"{anio_fecha_vencimiento}-{mes_fecha_vencimiento.ToString().PadLeft(2, '0')}-{dia_fecha_vencimiento.ToString().PadLeft(2, '0')}";
        }

        public void setFechaFechaVencimiento(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_fecha_vencimiento = int.Parse(fechaSplit[0]);
                mes_fecha_vencimiento = int.Parse(fechaSplit[1]);
                dia_fecha_vencimiento = int.Parse(fechaSplit[2]);
            }
        }
    }
    public class PolizaCls<T, X>
    {
        public string num_poliza { get; set; }
        public string fecha_expedicion { get; set; }
        public string fecha_vencimiento { get; set; }
        public string nombre_aseguradora { get; set; }
        public long valor_asegurado { get; set; }
        public bool ha_presentado_reclamacion { get; set; }
        public string motivo_reclamacion { get; set; }
        public T soporte_pago { get; set; }
        public X poliza_vigente { get; set; }
    }

    public class PolizaDtoRequest<T,X>
    {
        public PolizaCls<T,X> poliza { get; set; }
    }

    public class soporte_adjunto
    {
        public Data data { get; set; }

    }

}
