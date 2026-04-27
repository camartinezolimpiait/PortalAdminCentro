using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
    public class InfoConstitucionDtoCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_matricula_mercantil { get; set; }
        //[Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string acto_de_creacion { get; set; }
        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es correcto.")]
        public int diaMatricula { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es correcto.")]
        public int mesMatricula { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es correcto.")]
        public int anioMatricula { get; set; }

        [Required]
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es correcto.")]
        public int diaRenovacion { get; set; }
        [Required]
        [Range(1, 12,
        ErrorMessage = "El mes ingresado no es correcto.")]
        public int mesRenovacion { get; set; }
        [Required]
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es correcto.")]
        public int anioRenovacion { get; set; }

        //[Required]
        //[Range(1, 31,
        //ErrorMessage = "El dia ingresado no es correcto.")]
        public int diaActoCreacion { get; set; }
        //[Required]
        //[Range(1, 12,
        //ErrorMessage = "El mes ingresado no es correcto.")]
        public int mesActoCreacion { get; set; }
        //[Required]
        //[Range(1900, 3000,
        //ErrorMessage = "El año ingresado no es correcto.")]
        public int anioActoCreacion { get; set; }
        //[Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile acto_administrativo { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string estado_matricula { get; set; }
        public string url_adjunto_acto_administrativo { get; set; }
        public string nombre_adjunto_acto_administrativo { get; set; }

        public string getFechaMatricula()
        {
            return $"{anioMatricula}-{mesMatricula.ToString().PadLeft(2, '0')}-{diaMatricula.ToString().PadLeft(2, '0')}";
        }

        public void setFechaMatricula(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anioMatricula = int.Parse(fechaSplit[0]);
                mesMatricula = int.Parse(fechaSplit[1]);
                diaMatricula = int.Parse(fechaSplit[2]);
            }
        }

        public string getFechaRenovacion()
        {
            return $"{anioRenovacion}-{mesRenovacion.ToString().PadLeft(2, '0')}-{diaRenovacion.ToString().PadLeft(2, '0')}";
        }

        public void setFechaRenovacion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anioRenovacion = int.Parse(fechaSplit[0]);
                mesRenovacion = int.Parse(fechaSplit[1]);
                diaRenovacion = int.Parse(fechaSplit[2]);
            }
        }

        public string getFechaActoCreacion()
        {
            if (anioActoCreacion.Equals(0))
                anioActoCreacion = 1901;
            if (mesActoCreacion.Equals(0))
                mesActoCreacion = 1;
            if(diaActoCreacion.Equals(0))
                diaActoCreacion= 1;
            
            return $"{anioActoCreacion}-{mesActoCreacion.ToString().PadLeft(2, '0')}-{diaActoCreacion.ToString().PadLeft(2, '0')}";
        }

        public void setFechaActoCreacion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                var anio = int.Parse(fechaSplit[0]);
                int mes = 0;
                int dia = 0;

                if (anio.Equals(1901))
                    anio = 0;
                else
                {
                    dia = int.Parse(fechaSplit[2]);
                    mes = int.Parse(fechaSplit[1]);
                }
                
                anioActoCreacion = anio;
                mesActoCreacion = mes;
                diaActoCreacion = dia;
            }
        }
    }

    public class InfoConstitucionClsCEA<T>
    {
        [Required]
        public string num_matricula_mercantil { get; set; }
        public string acto_de_creacion { get; set; }
        [Required]
        public string fecha_matricula { get; set; }
        [Required]
        public string fecha_ultima_renovacion { get; set; }        
        public string fecha_acto_creacion { get; set; }
        public T acto_administrativo { get; set; }
        public string estado_matricula { get; set; }

    }

    public class InfoConstitucionDtoRequestCEA<T>
    {
        public InfoConstitucionClsCEA<T> constitucion { get; set; }
    }
}
