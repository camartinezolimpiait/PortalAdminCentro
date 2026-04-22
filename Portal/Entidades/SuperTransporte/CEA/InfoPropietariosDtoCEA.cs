using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
	public class InfoPropietariosDtoCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string tipo_id { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string num_id { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string primer_nombre { get; set; }
        
        public string segundo_nombre { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string primer_apellido { get; set; }
        
        public string segundo_apellido { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string email { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public Int64 telefono { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string direccion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string departamento { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string ciudad { get; set; }
    }
    public class InfoPropietariosClsCEA
    {
        public List<InfoPropietariosDtoCEA> lista_propietarios { get; set; }
    }

    public class InfoPropietariosDtoRequestCEA
    {
        public List<InfoPropietariosDtoCEA> propietarios { get; set; }
    }

}
