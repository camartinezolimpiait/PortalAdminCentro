using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models
{
    public class ConfigurarNumeracionModel
    {
        #region Properties

        [Required(ErrorMessage = "Ingrese el Numero de resolución del proveedor tecnológico.")]
        [StringLength(20, ErrorMessage = "El campo no debe tener mas de 20 caracteres.")]
        [RegularExpression(@"^\d{1,20}$", ErrorMessage = "El campo debe tener entre 1 y 20 caracteres numéricos.")]
        public string NumeroResolucion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione la fecha de inicio de la resolución registrada.")]
        public DateOnly FechaInicio { get; set; } = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        [Required(ErrorMessage = "Seleccione la fecha de final de la resolución registrada.")]
        public DateOnly Fechafin { get; set; } = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        [Required(ErrorMessage = "Ingrese el prefijo configurado.")]
        [StringLength(4, ErrorMessage = "El prefijo no debe tener mas de 4 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9]{1,4}$", ErrorMessage = "El prefijo debe tener entre 1 y 4 caracteres alfanuméricos.")]
        public string Prefijo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el inicio del consecutivo del proveedor tecnológico.")]
        [Range(1, 999999999999, ErrorMessage = "El campo no debe tener mas de 12 caracteres.")]
        [RegularExpression(@"^\d{1,12}$", ErrorMessage = "El campo debe tener entre 1 y 12 caracteres numéricos.")]
        public int Desde { get; set; } = 0;

        [Required(ErrorMessage = "Ingrese el fin del consecutivo del proveedor tecnológico.")]
        [Range(1, 999999999999, ErrorMessage = "El campo no debe tener mas de 12 caracteres.")]
        [RegularExpression(@"^\d{1,12}$", ErrorMessage = "El campo debe tener entre 1 y 12 caracteres numéricos.")]
        public int Hasta { get; set; } = 0;

        public bool ConsecutivoEspecifico { get; set; } = false;

        [Range(0, 999999999999, ErrorMessage = "El campo no debe tener mas de 12 caracteres.")]
        [RegularExpression(@"^\d{0,12}$", ErrorMessage = "El campo debe tener entre 1 y 12 caracteres numéricos.")]
        public int EmpezarDesde { get; set; } = 0;

        #endregion Properties
    }
}