using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class DatosArticulos
    {
        #region Properties

        public bool AplicaConfiguracionEspecifica { get; set; }
        public int IdTipoPin { get; set; }
        public string IdRunt { get; set; } = string.Empty;
        public List<DetalleArticulo> DetalleArticulos { get; set; } = [];
        public string UsuarioPortal { get; set; } = string.Empty;

        #endregion Properties
    }

    public class DetalleArticulo
    {
        #region Properties

        public string TipoConfiguracion { get; set; } = string.Empty;
        public string NombreCategoria { get; set; } = string.Empty;
        public string NombreArticulo { get; set; } = string.Empty;
        public string CodigoArticulo { get; set; } = string.Empty;
        public bool EstadoArticulo { get; set; } = false;

        #endregion Properties
    }
}
