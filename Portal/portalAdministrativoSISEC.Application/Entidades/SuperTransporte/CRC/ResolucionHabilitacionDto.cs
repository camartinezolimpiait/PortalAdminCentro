using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
	public class ResolucionHabilitacionDto
	{
        [Required]
        public bool tiene_resolucion { get; set; }
        [Required(ErrorMessage ="El campo es obligatorio")]
        public string num_resolucion { get; set; }
        [Range(1, 31,
        ErrorMessage = "El dia ingresado no es válido.")]
        public int dia { get; set; }
        [Range(1,12,
        ErrorMessage = "El mes ingresado no es válido.")]
        public int mes { get; set; }
        [Range(1900, 3000,
        ErrorMessage = "El año ingresado no es válido.")]
        public int anio { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio")]
        [ExtensionFileValid]
        public IBrowserFile resolucion_de_habilitacion { get; set; }
        public string url_adjunto_resolucion_habiltacion { get; set; }
        public string nombre_adjunto_resolucion_habiltacion { get; set; }

        public string getFecha() {
            return $"{anio}-{mes.ToString().PadLeft(2,'0')}-{dia.ToString().PadLeft(2, '0')}";
		}

        public void setFecha(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha)) {
                var fechaSplit = fecha.Split('-');
                anio = int.Parse(fechaSplit[0]);
                mes = int.Parse(fechaSplit[1]);
                dia = int.Parse(fechaSplit[2]);
            }
        }
    }
    public class ResolucionHabilitacionCls<T> {
        public bool tiene_resolucion { get; set; }
        public string num_resolucion { get; set; }
        public string fecha_resolucion { get; set; }
        public T resolucion_de_habilitacion { get; set; }
    }
    public class ResolucionHabilitacionClsFalse
    {
        public bool tiene_resolucion { get; set; }
        public string num_resolucion { get; set; }
        public string fecha_resolucion { get; set; }
    }

    public class ResolucionHabilitacionDtoRequest<T>
    {
        public ResolucionHabilitacionCls<T> resolucion_de_habilitacion { get; set; }
    }
    public class ResolucionHabilitacionDtoRequestFalse
    {
        public ResolucionHabilitacionClsFalse resolucion_de_habilitacion { get; set; }
    }

}
