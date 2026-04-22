using Newtonsoft.Json.Serialization;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace portalAdministrativoSISEC.Entidades
{
    public class RespuestaServiciosGetVigilado
    {
        public DataVigilado data { get; set; }

    }
    public class DataVigilado
    {
        public int id { get; set; }
        public AttributesVigilado attributes { get; set; }
    }

    public class AttributesVigilado
    {
        public string NIT { get; set; }
        public string razon_social { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public RespuestaServiciosGetByRunt crcs { get; set; }
        public RespuestaServiciosGetByRuntCEA ceas { get; set; }
        public AcreditacionONACVigiladoDto<GetFile> acreditacion_onac { get; set; }
    }

    public class AcreditacionONACVigiladoRequest
    {
        public AcreditacionONACVigiladoDto<int?> acreditacion_onac;
    }

    public class AcreditacionONACVigiladoDto<T>
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string codigo { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string estado { get; set; }
        public string fecha_publicacion { get; set; } = "";
        #region fecha_publicacion
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto.")]
        public int dia_publicacion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto.")]
        public int mes_publicacion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto.")]
        public int anio_publicacion { get; set; }
        #endregion
        public string fecha_vencimiento { get; set; }
        #region fecha_vencimiento
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto")]
        public int dia_vencimiento { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto")]
        public int mes_vencimiento { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto")]
        public int anio_vencimiento { get; set; }
        #endregion
        public string fecha_ultima_actualizacion { get; set; }
        #region fecha_ultima_actualizacion
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto")]
        public int dia_ultima_actualizacion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto")]
        public int mes_ultima_actualizacion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto")]
        public int anio_ultima_actualizacion { get; set; }
        #endregion
        public string fecha_renovacion { get; set; }
        #region fecha_renovacion
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 31, ErrorMessage = "El dia ingresado no es correcto")]
        public int dia_renovacion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, 12, ErrorMessage = "El mes ingresado no es correcto")]
        public int mes_renovacion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1900, 2100, ErrorMessage = "El año ingresado no es correcto")]
        public int anio_renovacion { get; set; }
        #endregion
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string norma_acreditada { get; set; }
        public string url_adjunto_certificado_onac { get; set; }
        public string nombre_adjunto_certificado_onac { get; set; }
        public T certificado_onac { get; set; }



        public string getFecha_publicacion()
        {
            return $"{anio_publicacion}-{mes_publicacion.ToString().PadLeft(2, '0')}-{dia_publicacion.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_vencimiento()
        {
            return $"{anio_vencimiento}-{mes_vencimiento.ToString().PadLeft(2, '0')}-{dia_vencimiento.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_ultima_actualizacion()
        {
            return $"{anio_ultima_actualizacion}-{mes_ultima_actualizacion.ToString().PadLeft(2, '0')}-{dia_ultima_actualizacion.ToString().PadLeft(2, '0')}";
        }

        public string getFecha_renovacion()
        {
            return $"{anio_renovacion}-{mes_renovacion.ToString().PadLeft(2, '0')}-{dia_renovacion.ToString().PadLeft(2, '0')}";
        }

        public void setFecha_publicacion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_publicacion = int.Parse(fechaSplit[0]);
                mes_publicacion = int.Parse(fechaSplit[1]);
                dia_publicacion = int.Parse(fechaSplit[2]);
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

        public void setFecha_ultima_actualizacion(string fecha)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                var fechaSplit = fecha.Split('-');
                anio_ultima_actualizacion = int.Parse(fechaSplit[0]);
                mes_ultima_actualizacion = int.Parse(fechaSplit[1]);
                dia_ultima_actualizacion = int.Parse(fechaSplit[2]);
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
}
