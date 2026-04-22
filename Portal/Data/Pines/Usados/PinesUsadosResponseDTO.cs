using System;

namespace portalAdministrativoSISEC.Data.Pines
{
    public class PinesUsadosResponseDTO
    {
        public string Pin {  get; set; }
        public int TipoDocumento { get; set; }
        public string Documento { get; set; }
        public decimal ValorPin { get; set; }
        public decimal ValorActor { get; set; }
        public decimal ValorANSV { get; set; }
        public decimal ValorAliado { get; set; }
        public decimal ValorSicov { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public int? IdDispersion { get; set; }
        public DateTime? FechaDispersion { get; set; }
        public string BancoDispersion { get; set; }
        public string CuentaDispersion { get; set; }
        public decimal ValorDispersion { get; set; }
        public string AgenteDispersion { get; set; }
        public string RazonSocial { get; set; }
        public string TipoPin { get; set; }
        public string PinesAsociados { get; set; }
    }
}
