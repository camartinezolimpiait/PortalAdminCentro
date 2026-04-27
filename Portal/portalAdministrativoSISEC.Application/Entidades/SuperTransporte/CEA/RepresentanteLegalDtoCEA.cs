using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
	public class RepresentanteLegalDtoCEA
    {
        public string tipo_doc { get; set; }
        public string num_doc { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }

        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile certificado_existencia { get; set; }
        public string url_adjunto_certificado_existencia { get; set; }
        public string nombre_adjunto_certificado_existencia { get; set; }
    }
    public class RepresentanteLegalClsCEA<T>
    {
        public string tipo_doc { get; set; }
        public string num_doc { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public T certificado_existencia { get; set; }
    }

    public class RepresentanteLegalDtoRequestCEA<T>
    {
        public RepresentanteLegalClsCEA<T> representante_legal { get; set; }
    }

}
