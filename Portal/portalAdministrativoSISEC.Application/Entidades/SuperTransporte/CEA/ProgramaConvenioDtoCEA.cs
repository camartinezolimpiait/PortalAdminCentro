using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte.CEA
{
    public class ProgramaConvenioDtoCEA
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public bool ofrece { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        [Range(0, long.MaxValue, ErrorMessage = "El valor no puede ser negativo.")]
        public long nit_cea_convenio { get; set; }
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string secretaria { get; set; }
        [Required(ErrorMessage = "El adjunto es obligatorio.")]
        [ExtensionFileValid]
        public IBrowserFile registro { get; set; }
        public string url_adjunto_registro { get; set; }
        public string nombre_adjunto_registro { get; set; }
    }
    public class ProgramaConvenioClsCEA<T>
    {
        public bool ofrece { get; set; }
        public long nit_cea_convenio { get; set; }
        public string secretaria { get; set; }
        public T registro { get; set; }
    }
    public class ProgramaConvenioClsCEAFalse
    {
        public bool ofrece { get; set; }
        public long nit_cea_convenio { get; set; }
        public string secretaria { get; set; }
    }
    public class ProgramaConvenioDtoRequestCEA<T>
    {
        public ProgramaConvenioClsCEA<T> convenios { get; set; }
    }
    public class ProgramaConvenioDtoRequestCEAFalse
    {
        public ProgramaConvenioClsCEAFalse convenios { get; set; }
    }
}
