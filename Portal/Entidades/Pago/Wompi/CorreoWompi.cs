namespace portalAdministrativoSISEC.Entidades.Pago.Wompi
{
    public class CorreoWompi
    {
        public string PinField { get; set; } = string.Empty;
        public string TotalTransaccion { get; set; } = string.Empty;
        public int Cuotas { get; set; }
        public string DispersionCentro { get; set; }
    }
}