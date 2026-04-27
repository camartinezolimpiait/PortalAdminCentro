namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    public class ConfiguracionFacturacion
    {
        #region Properties

        public int TipoPersona { get; set; } = 0;
        public string RazonSocial { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string DV { get; set; } = string.Empty;
        public string NombreCentro { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string RegimenContributivo { get; set; } = string.Empty;
        public string DireccionCentro { get; set; } = string.Empty;
        public string DepartamentoCentro { get; set; } = string.Empty;
        public string CiudadCentro { get; set; } = string.Empty;
        public string Nota { get; set; } = string.Empty;

        public bool ActivarFacturacion { get; set; } = false;
        public int MomentoFacturacion { get; set; } = 0;

        public string NumeroResolucion { get; set; } = string.Empty;
        public string InicioResolucion { get; set; } = string.Empty;
        public string FinResolucion { get; set; } = string.Empty;
        public string PrefijoResolucion { get; set; } = string.Empty;
        public int Desde { get; set; } = 0;
        public int Hasta { get; set; } = 0;
        public bool CheckConsecutivo { get; set; } = false;
        public int EmpezarDesde { get; set; } = 0;

        public string Proveedor { get; set; } = string.Empty;
        public string ProximaFactura { get; set; } = string.Empty;

        public string Usuario { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;

        #endregion Properties
    }
}