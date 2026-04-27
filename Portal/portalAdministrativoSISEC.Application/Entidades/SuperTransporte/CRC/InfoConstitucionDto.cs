using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
    public class InfoConstitucionDto
    {
        [Required(ErrorMessage = "El valor ingresado no es correcto.")]
        public string num_matricula_mercantil { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string estado_matricula { get; set; }
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
    }

    public class InfoConstitucionCls
    {
        [Required]
        public string num_matricula_mercantil { get; set; }
        [Required]
        public string estado_matricula { get; set; }
        [Required]
        public string fecha_matricula { get; set; }
        [Required]
        public string fecha_ultima_renovacion { get; set; }
    }

    public class InfoConstitucionDtoRequest
    {
        public InfoConstitucionCls constitucion { get; set; }
    }


}
