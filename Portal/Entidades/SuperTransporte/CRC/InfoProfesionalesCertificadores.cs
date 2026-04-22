using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
	public class InfoProfesionalesCertificadoresDto
	{
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public int cantidad_certificadores { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile acredita_cargue { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile registro_runt { get; set; }
        public string url_adjunto_registro_runt { get; set; }
        public string nombre_adjunto_registro_runt { get; set; }
        public string url_adjunto_acredita_cargue { get; set; }
        public string nombre_adjunto_acredita_cargue { get; set; }

    }

    public class lista_profesionales 
	{
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string profesion { get; set; }
		[Required]
        [Range(1, 100,
        ErrorMessage = "La cantidad no es correcta.")]
        public int cantidad { get; set; }
    }

	public class InfoProfesionalesCertificadoresCls<T,X>
	{
		public int cantidad_certificadores { get; set; }
        public List<lista_profesionales> lista_profesionales { get; set; }
        public T acredita_cargue { get; set; }
        public X registro_runt { get; set; }
    }

    public class InfoProfesionalesCertificadoresDtoRequest<T,X>
	{
        public InfoProfesionalesCertificadoresCls<T,X> profesionales_certificadores { get; set; }
    }

}
