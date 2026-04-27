using System;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class DatosNumeracion
    {
        #region Properties

        public string IdRunt { get; set; } = string.Empty;
        public string NumeroResolucion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Prefijo { get; set; } = string.Empty;
        public string NumeroDesde { get; set; } = string.Empty;
        public string NumeroHasta { get; set; } = string.Empty;
        public bool EmpezarDesde { get; set; } = false;
        public int NumeroEmpezarDesde { get; set; } = 0;
        public string UsuarioPortal { get; set; } = string.Empty;

        #endregion Properties
    }
}