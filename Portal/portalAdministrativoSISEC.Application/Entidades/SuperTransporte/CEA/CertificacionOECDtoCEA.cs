using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
    public class CertificacionOECDtoCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_certificado { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string estado_acreditacion { get; set; }

        #region fecha_vigilancia
        [Required]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_vigilancia { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_vigilancia { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_vigilancia { get; set; }
        #endregion

        #region fecha_seguimiento
        [Required]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_seguimiento { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_seguimiento { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_seguimiento { get; set; }
        #endregion

        #region fecha_vencimiento
        [Required]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_vencimiento { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_vencimiento { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_vencimiento { get; set; }
        #endregion

        #region fecha_renovacion
        [Required]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_renovacion { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_renovacion { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_renovacion { get; set; }
        #endregion

        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string empresa_que_expide { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string esquema { get; set; }

        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile certificado_acreditacion { get; set; }
        public string url_adjunto_certificado_acreditacion { get; set; }
        public string nombre_adjunto_certificado_acreditacion { get; set; }

        public string getFecha_vigilancia()
        {
            return $"{anio_vigilancia}-{mes_vigilancia.ToString().PadLeft(2, '0')}-{dia_vigilancia.ToString().PadLeft(2, '0')}";
        }
        public string getFecha_seguimiento()
        {
            return $"{anio_seguimiento}-{mes_seguimiento.ToString().PadLeft(2, '0')}-{dia_seguimiento.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_vencimiento()
        {
            return $"{anio_vencimiento}-{mes_vencimiento.ToString().PadLeft(2, '0')}-{dia_vencimiento.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_renovacion()
        {
            return $"{anio_renovacion}-{mes_renovacion.ToString().PadLeft(2, '0')}-{dia_renovacion.ToString().PadLeft(2, '0')}";
        }

        public void setFecha_vigilancia(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_vigilancia = int.Parse(fechaSplit[0]);
                mes_vigilancia = int.Parse(fechaSplit[1]);
                dia_vigilancia = int.Parse(fechaSplit[2]);
            }
        }

        public void setFecha_seguimiento(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_seguimiento = int.Parse(fechaSplit[0]);
                mes_seguimiento = int.Parse(fechaSplit[1]);
                dia_seguimiento = int.Parse(fechaSplit[2]);
            }
        }

        public void setFecha_vencimiento(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_vencimiento = int.Parse(fechaSplit[0]);
                mes_vencimiento = int.Parse(fechaSplit[1]);
                dia_vencimiento = int.Parse(fechaSplit[2]);
            }
        }

        public void setFecha_renovacion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_renovacion = int.Parse(fechaSplit[0]);
                mes_renovacion = int.Parse(fechaSplit[1]);
                dia_renovacion = int.Parse(fechaSplit[2]);
            }
        }
    }
    public class CertificacionOECClsCEA<T>
    {
        public string num_certificado { get; set; }
        public string estado_acreditacion { get; set; }
        public string fecha_vigilancia { get; set; }
        public string fecha_seguimiento { get; set; }
        public string fecha_vencimiento { get; set; }
        public string fecha_renovacion { get; set; }
        public string empresa_que_expide { get; set; }
        public string esquema { get; set; }
        public T certificado_acreditacion { get; set; }
    }
    public class CertificacionOECDtoRequestCEA<T>
    {
        public CertificacionOECClsCEA<T> certificacion_oec { get; set; }
    }
}
