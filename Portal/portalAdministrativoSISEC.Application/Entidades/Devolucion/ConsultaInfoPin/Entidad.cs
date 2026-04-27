using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin
{
	public class Entidad
	{
        public string Pin { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int TipoTramite { get; set; }
        public int TipoTramite2 { get; set; }
        public string Categoria { get; set; }
        public string Categoria2 { get; set; }
        public string Genero { get; set; }
        public string NombreCompleto { get; set; }
        public string IdRunt { get; set; }
        public int EstadoPin { get; set; }
        public long IdPtesaPin { get; set; }
        public DateTime FechaReferencia { get; set; }
        public DateTime FechaRecaudo { get; set; }
        public DateTime FechaUso { get; set; }
        public decimal ValorTransaccion { get; set; }
        public string IdMedioRecaudo { get; set; }
        public int IdOrigenCotizacion { get; set; }
        public DateTime FechaAnulacion { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public DateTime FechaVencimientoReferencia { get; set; }
        public int IdTipoPin { get; set; }
        public int IdCliente { get; set; }
        public int CuotasPactadas { get; set; }
        public int CuotasPendientes { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal ValorAliadoCuota { get; set; }
        public decimal SaldoPendiente { get; set; }
        public List<Pago> Pagos { get; set; }
        public string NUT { get; set; }

    }
    public class Pago
    {
        public string Pin { get; set; }
        public long IdPtesaPin { get; set; }
        public decimal ValorTransaccion { get; set; }
        public DateTime FechaRecaudo { get; set; }
    }
}
