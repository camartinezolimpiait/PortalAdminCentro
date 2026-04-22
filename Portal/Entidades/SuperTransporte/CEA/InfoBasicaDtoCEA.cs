using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
    public class InfoBasicaDtoCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string nivel { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public bool auth_cursos_normas { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_cert_conformidad { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public bool forma_instructores { get; set; }

        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_fecha_aprobacion { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_fecha_aprobacion { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_fecha_aprobacion { get; set; }
        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_fecha_vencimiento { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_fecha_vencimiento { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_fecha_vencimiento { get; set; }

        public CategoriasAutorizadas categorias_autorizadas { get; set; } = new CategoriasAutorizadas();

        public string getFechaAprobacion()
        {
            return $"{anio_fecha_aprobacion}-{mes_fecha_aprobacion.ToString().PadLeft(2, '0')}-{dia_fecha_aprobacion.ToString().PadLeft(2, '0')}";
        }

        public void setFechaAprobacion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_fecha_aprobacion = int.Parse(fechaSplit[0]);
                mes_fecha_aprobacion = int.Parse(fechaSplit[1]);
                dia_fecha_aprobacion = int.Parse(fechaSplit[2]);
            }
        }

        public string getFechaVencimiento()
        {
            return $"{anio_fecha_vencimiento}-{mes_fecha_vencimiento.ToString().PadLeft(2, '0')}-{dia_fecha_vencimiento.ToString().PadLeft(2, '0')}";
        }

        public void setFechaVencimiento(string fecha)
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
    public class InfoBasicaClsCEA
    {
        public string nivel { get; set; }
        public bool auth_cursos_normas { get; set; }
        public string num_cert_conformidad { get; set; }
        public string fecha_aprobacion_cert { get; set; }
        public string fecha_vencimiento_cert { get; set; }
        public bool forma_instructores { get; set; }
        public CategoriasAutorizadas categorias_autorizadas { get; set; }
    }

    public class InfoBasicaDtoRequestCEA
    {
        public InfoBasicaClsCEA informacion_basica { get; set; }
    }
}
