using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CRC
{
	public class InterconexionRUNTDto
    {

        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile soporte_interconexion { get; set; }
        public string url_adjunto_soporte_interconexion { get; set; }
        public string nombre_adjunto_soporte_interconexion { get; set; }

    }
    public class InterconexionRUNTCls<T>
    {
        public T soporte_interconexion { get; set; }
    }

    public class InterconexionRUNTDtoRequest<T>
    {
        public InterconexionRUNTCls<T> interconexion_runt { get; set; }
    }

}
