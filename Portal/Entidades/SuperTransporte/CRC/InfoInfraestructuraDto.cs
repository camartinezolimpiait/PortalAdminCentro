using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
	public class InfoInfraestructuraDto
	{
        
        public bool oficina_admin { get; set; }
        public string ubicacion_oficina { get; set; }
		public bool servicios_aseo { get; set; }
        public string ubicacion_aseo { get; set; }
		public bool recepcion { get; set; }
        public string ubicacion_recepcion { get; set; }
		public bool unidades_sanitarias { get; set; }
        public string ubicaciones_und_sanitarias { get; set; }
		public bool sala_de_espera { get; set; }
        public string ubicacion_sala_de_espera { get; set; }
	}

    public class consultorios
	{
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo no puede tener valor 0 o negativo")]
        public decimal largo { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo no puede tener valor 0 o negativo")]
        public decimal ancho { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo no puede tener valor 0 o negativo")]
        public decimal area { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string ubicacion { get; set; }
    }

	public class InfoInfraestructuraCls
	{
        public bool oficina_admin { get; set; }
        public string ubicacion_oficina { get; set; }
        public bool servicios_aseo { get; set; }
        public string ubicacion_aseo { get; set; }
        public bool recepcion { get; set; }
        public string ubicacion_recepcion { get; set; }
        public bool unidades_sanitarias { get; set; }
        public string ubicaciones_und_sanitarias { get; set; }
        public bool sala_de_espera { get; set; }
        public string ubicacion_sala_de_espera { get; set; }
        public List<consultorios> consultorios_opto { get; set; }
		public List<consultorios> consultorios_psico { get; set; }

		public List<consultorios> consultorios_fono { get; set; }

		public List<consultorios> consultorios_gene { get; set; }

		public List<consultorios> consultorios_cert { get; set; }
	}

    public class InfoInfraestructuraDtoRequest
	{
        public InfoInfraestructuraCls infraestructura { get; set; }
    }
}
