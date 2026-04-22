using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
	public class InfraestructuraDtoCEA
    {
        public bool espacio_clases_practicas { get; set; }
        public bool oficina_admin { get; set; }
        public string ubicacion_oficina { get; set; }
        public bool recepcion { get; set; }
        public string ubicacion_recepcion { get; set; }
        public bool sala_de_espera { get; set; }
        public string ubicacion_sala_de_espera { get; set; }
        public bool servicios_aseo { get; set; }
        public string ubicacion_aseo { get; set; }
        public bool unidades_sanitarias { get; set; }
    }

    public class AulaCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo no puede tener valor 0 o negativo")]
        public int num_sillas { get; set; }
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
        [Range(1, int.MaxValue, ErrorMessage = "El campo no puede tener valor 0 o negativo")]
        public string ubicacion { get; set; }
    }

    public class PistaCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string direccion { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string departamento { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string ciudad { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public bool propio { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile contrato_vigente { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile ctl { get; set; }
    }

    public class PistaClsResponseCEA
    {
        public string direccion { get; set; }
        public string departamento { get; set; }
        public string ciudad { get; set; }
        public bool propio { get; set; }
        public GetFile contrato_vigente { get; set; }
        public GetFile ctl { get; set; }
    }

    public class PistaClsCEA
    {
        public string direccion { get; set; }
        public string departamento { get; set; }
        public string ciudad { get; set; }
        public bool propio { get; set; }
        public int? contrato_vigente { get; set; }
        public int? ctl { get; set; }
    }

    public class UnidadSanitariaCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string ubicacion_unidad_sanitaria { get; set; }
    }

    public class InfraestructuraClsCEA
    {
        public List<AulaCEA> aulas { get; set; }
        public bool espacio_clases_practicas { get; set; }
        public List<PistaClsCEA> pistas { get; set; }
        public bool oficina_admin { get; set; }
        public string ubicacion_oficina { get; set; }
        public bool recepcion { get; set; }
        public string ubicacion_recepcion { get; set; }
        public bool sala_de_espera { get; set; }
        public string ubicacion_sala_de_espera { get; set; }
        public bool servicios_aseo { get; set; }
        public string ubicacion_aseo { get; set; }
        public bool unidades_sanitarias { get; set; }
        public List<UnidadSanitariaCEA> lista_unidades_sanitarias { get; set; }
    }

    public class InfraestructuraClsResponseCEA
    {
        public List<AulaCEA> aulas { get; set; }
        public bool espacio_clases_practicas { get; set; }
        public List<PistaClsResponseCEA> pistas { get; set; }
        public bool oficina_admin { get; set; }
        public string ubicacion_oficina { get; set; }
        public bool recepcion { get; set; }
        public string ubicacion_recepcion { get; set; }
        public bool sala_de_espera { get; set; }
        public string ubicacion_sala_de_espera { get; set; }
        public bool servicios_aseo { get; set; }
        public string ubicacion_aseo { get; set; }
        public bool unidades_sanitarias { get; set; }
        public List<UnidadSanitariaCEA> lista_unidades_sanitarias { get; set; }
    }

    public class InfraestructuraDtoRequestCEA
	{
        public InfraestructuraClsCEA infraestructura { get; set; }
    }

    public class Ctl
    {
        public int id { get; set; }
    }

}
