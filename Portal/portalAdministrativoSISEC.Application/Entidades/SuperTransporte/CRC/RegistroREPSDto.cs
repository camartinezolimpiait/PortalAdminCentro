using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
    public class RegistroREPSDto
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string codigo_inscripcion { get; set; }

        #region fecha_inscripcion
        [Required]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_inscripcion { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_inscripcion { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_inscripcion { get; set; }
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

        #region fecha_ultima_autoevaluacion
        [Required]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_ultima_autoevaluacion { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_ultima_autoevaluacion { get; set; }
        [Required]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_ultima_autoevaluacion { get; set; }
        #endregion

        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string secretaria_de_salud { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string departamento { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string ciudad { get; set; }
        public string url_adjunto_formulario_inscripcion { get; set; }
        public string nombre_adjunto_formulario_inscripcion { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile formulario_inscripcion { get; set; }

        public string getFecha_inscripcion()
        {
            return $"{anio_inscripcion}-{mes_inscripcion.ToString().PadLeft(2, '0')}-{dia_inscripcion.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_vencimiento()
        {
            return $"{anio_vencimiento}-{mes_vencimiento.ToString().PadLeft(2, '0')}-{dia_vencimiento.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_renovacion()
        {
            return $"{anio_renovacion}-{mes_renovacion.ToString().PadLeft(2, '0')}-{dia_renovacion.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_ultima_autoevaluacion()
        {
            return $"{anio_ultima_autoevaluacion}-{mes_ultima_autoevaluacion.ToString().PadLeft(2, '0')}-{dia_ultima_autoevaluacion.ToString().PadLeft(2, '0')}";
        }

        public void setFecha_inscripcion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_inscripcion = int.Parse(fechaSplit[0]);
                mes_inscripcion = int.Parse(fechaSplit[1]);
                dia_inscripcion = int.Parse(fechaSplit[2]);
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

        public void setFecha_ultima_autoevaluacion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_ultima_autoevaluacion = int.Parse(fechaSplit[0]);
                mes_ultima_autoevaluacion = int.Parse(fechaSplit[1]);
                dia_ultima_autoevaluacion = int.Parse(fechaSplit[2]);
            }
        }
    }
    public class RegistroREPSCls<T>
    {
        public string codigo_inscripcion { get; set; }
        public string fecha_inscripcion { get; set; }
        public string fecha_vencimiento { get; set; }
        public string fecha_renovacion { get; set; }
        public string fecha_ultima_autoevaluacion { get; set; }
        public string secretaria_de_salud { get; set; }
        public string departamento { get; set; }
        public string ciudad { get; set; }
        public T formulario_inscripcion { get; set; }
    }
    public class RegistroREPSDtoRequest<T>
    {
        public RegistroREPSCls<T> registro_reps { get; set; }
    }
}
