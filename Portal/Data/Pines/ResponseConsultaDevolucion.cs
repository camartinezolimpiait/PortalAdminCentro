using System;

namespace portalAdministrativoSISEC.Data.Pines
{
    public class ResponseConsultaDevolucion
    {
        public DateTime? FechaDevolucion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string TipoDevolucion { get; set; } ="";
        public string Banco { get; set; } ="";
        public string CuentaBanco { get; set; } ="";
        public string Correo { get; set; } ="";
        public string ValorDevolver { get; set; } = "";
        public int TotalRegistros { get; set; }
        public string StrFechaRegistro { get; set; }="";
        public string StrFechaDevolucion { get; set; }="";
        public string Pin { get; set; }="";
        public string NumeroIdentificacion { get; set; }="";
        public string NombreCompleto { get; set; }="";
        public string EstadoDevolucion { get; set; }="";
        public string NovedadDevolucion { get; set; } = "";
        public int? IdAgenteDispersion { get; set; }
        public string AgenteDispersion { get; set; } = "";
        public string RazonSocial { get; set; }
        public int? CanalVenta { get; set; }
        public string Cuotas { get; set; }
        public int? PagosRealizados { get; set; }
        public string? NUTVenta { get; set; }
        public string IdTipoIdentificacion { get; set; }
    }
}
