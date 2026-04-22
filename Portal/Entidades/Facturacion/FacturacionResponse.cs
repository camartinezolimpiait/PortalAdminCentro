using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class FacturacionResponse<T>
    {
        #region Properties

        public bool SolicitudExitosa { get; set; } = false;
        public string Mensaje { get; set; } = "Ha ocurrido un error";
        public T Datos { get; set; }
        public List<string> Errores { get; set; } = [];

        #endregion Properties
    }
}