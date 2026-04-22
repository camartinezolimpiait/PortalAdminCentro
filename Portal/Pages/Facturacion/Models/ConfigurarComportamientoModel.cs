using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models
{
    public class ConfigurarComportamientoModel
    {
        #region Properties

        public bool FacturacionActiva { get; set; }

        [Required(ErrorMessage = "Seleccione una opción para generar sus facturas.")]
        public string EventoFacturacion { get; set; }

        #endregion Properties
    }
}