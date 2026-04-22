namespace portalAdministrativoSISEC.Entidades.Devolucion
{
    public class ConsultaEstadoDevolucion
    {
        public string IdCliente { get; set; }
        public string NumeroIdentificacion { get; set; }
        public int TipoIdentificacion { get; set; }
        public string PinComprado { get; set; }
        public int IdOrigenCotizacion { get; set; }
        public string IdRunt { get; set; }
    }
}
