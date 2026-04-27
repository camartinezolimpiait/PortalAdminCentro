using System;

namespace portalAdministrativoSISEC.Entidades.Pines.Devoluciones
{
    public class ConsultaDevoluciones
    {
        public string Pin { get; set; }
        public string NumeroDocumento { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string TipoDevolucion { get; set; }
        public string Banco { get; set; }
        public string CuentaBanco { get; set; }
        public string Correo { get; set; }
        public decimal ValorADevolver { get; set; }
        public string EstadoDevolucion { get; set; }
        public string Novedad { get; set; }
        public string AgenteDispersion { get; set; }
    }
}