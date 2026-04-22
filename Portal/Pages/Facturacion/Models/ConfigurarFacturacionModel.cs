using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models
{
    public class ConfigurarFacturacionModel
    {
        #region Properties

        [Required(ErrorMessage = "Seleccione un proveedor tecnológico.")]
        public string Proveedor { get; set; } = string.Empty;

        public int ConsecutivoActual { get; set; }

        #endregion Properties
    }
}