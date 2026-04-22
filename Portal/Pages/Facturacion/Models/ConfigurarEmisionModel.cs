using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models
{
    public class ConfigurarEmisionModel
    {
        #region Properties

        [Required(ErrorMessage = "Ingrese el nombre del centro.")]
        [StringLength(255, ErrorMessage = "El campo no debe tener mas de 255 caracteres.")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el número de identificación del centro.")]
        [StringLength(15, ErrorMessage = "El campo no debe tener mas de 15 caracteres.")]
        [RegularExpression(@"^\d{1,15}$", ErrorMessage = "El campo debe tener entre 1 y 15 caracteres numéricos.")]
        public string NIT { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el dígito de verificación.")]
        [StringLength(1, ErrorMessage = "El campo no debe tener mas de 1 carácter.")]
        [RegularExpression(@"^[0-9]$", ErrorMessage = "El campo debe ser un dígito valido de 1 carácter numérico.")]
        public string Dv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el régimen del centro.")]
        public string RegimenContributivo { get; set; }

        [Required(ErrorMessage = "Ingrese el correo electrónico del centro.")]
        [StringLength(100, ErrorMessage = "El usuario no debe tener mas de 255 caracteres.")]
        [RegularExpression(@"^(?=.{1,100}$)[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "El campo debe ser un correo valido y tener entre 1 y 100 caracteres alfanuméricos.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el número de celular del centro.")]
        [StringLength(10, ErrorMessage = "El campo no debe tener mas de 10 caracteres.")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "Ingrese nombre de la sede o establecimiento, es posible relacionar la misma información del comercio, si no cuenta con sucursales.")]// Validar
        [StringLength(255, ErrorMessage = "El campo no debe tener mas de 255 caracteres.")]
        public string NombreComercial { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la dirección de la sede o establecimiento.")]
        [StringLength(255, ErrorMessage = "El campo no debe tener mas de 255 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione el departamento donde se encuentra el centro.")]
        public int? Departamento { get; set; }

        [Required(ErrorMessage = "Seleccione la ciudad donde se encuentra el centro.")]
        public int? Ciudad { get; set; }

        [StringLength(255, ErrorMessage = "El campo no debe tener mas de 255 caracteres.")]
        public string Observaciones { get; set; } = "Los conceptos de SICOV, ANSV y Operador de recaudo son ingresos recibidos por terceros. El numeral primero del artículo 476 del estatuto tributario señala que los servicios médicos están excluidos del IVA.";

        [Required(ErrorMessage = "Seleccione el tipo de persona del comercio.")]
        public int? TipoPersona { get; set; }

        #endregion Properties
    }
}