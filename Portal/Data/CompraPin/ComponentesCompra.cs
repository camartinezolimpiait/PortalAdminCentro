namespace portalAdministrativoSISEC.Data.CompraPin
{
    public class ComponentesCompra
    {
        public bool CompraPin { get; set; } = true;
        public bool DatosBasicos { get; set; } = false;
        public bool TipoTramite { get; set; } = false;
        public bool Categoria { get; set; }
        public bool SeleccionCentro { get; set; } = false;
        public bool DatosPersonales { get; set; } = false;
        public bool CuotaCeas { get; set; } = false;
        public bool MediosPago { get; set; } = false;
        public bool ConfirmarCompra { get; set; } = false;
        public bool CotizacionAgendamiento { get; set; } = false;
        public bool FacturaElectronica { get; set; } = false;

        // Método que desactiva todos los componentes
        public void DesactivarTodo()
        {
            CompraPin = false;
            DatosBasicos = false;
            TipoTramite = false;
            Categoria = false;
            SeleccionCentro = false;
            DatosPersonales = false;
            CuotaCeas = false;
            MediosPago = false;
            ConfirmarCompra = false;
            CotizacionAgendamiento = false;
            FacturaElectronica = false;
        }
    }
}
