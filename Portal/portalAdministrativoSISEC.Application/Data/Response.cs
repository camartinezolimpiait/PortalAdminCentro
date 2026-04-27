using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class Response
    {
        [JsonProperty("IdCentro")]
        public int IdCentro { get; set; }
        [JsonProperty("Nombre")]
        public string Nombre { get; set; }
        [JsonProperty("CodigoUso")]
        public string CodigoUso { get; set; }
        [JsonProperty("IdComercio")]
        public int IdComercio { get; set; }
        [JsonProperty("IdDepartamento")]
        public int IdDepartamento { get; set; }
        [JsonProperty("IdMunicipio")]
        public int IdMunicipio { get; set; }
        [JsonProperty("IdZona")]
        public int IdZona { get; set; }
        [JsonProperty("Direccion")]
        public string Direccion { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("Fijo")]
        public string Fijo { get; set; }
        [JsonProperty("Movil")]
        public string Movil { get; set; }
        [JsonProperty("Latitud")]
        public decimal Latitud { get; set; }
        [JsonProperty("Longitud")]
        public decimal Longitud { get; set; }
        [JsonProperty("Concurrencia")]
        public int Concurrencia { get; set; }
        [JsonProperty("PatrimonioAutonomo")]
        public bool? PatrimonioAutonomo { get; set; }
        [JsonProperty("ActivoRUNT")]
        public bool? ActivoRUNT { get; set; }
        [JsonProperty("Activo")]
        public bool? Activo { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }
        [JsonProperty("Modified")]
        public DateTime Modified { get; set; }
        [JsonProperty("ModifiedBy")]
        public string ModifiedBy { get; set; }
        [JsonProperty("RowVersion")]
        public int RowVersion { get; set; }
        [JsonProperty("ApplicationUser")]
        public Guid? ApplicationUser { get; set; }
        [JsonProperty("MatriculaMercantil")]
        public string MatriculaMercantil { get; set; }
        [JsonProperty("Resolucion")]
        public string Resolucion { get; set; }
        [JsonProperty("NumeroRegistro")]
        public string NumeroRegistro { get; set; }
        [JsonProperty("CapacidadMaxima")]
        public int CapacidadMaxima { get; set; }
        [JsonProperty("ConvenioBanco")]
        public string ConvenioBanco { get; set; }
        [JsonProperty("ValorSugerido")]
        public string ValorSugerido { get; set; }
        [JsonProperty("ValorComision")]
        public string ValorComision { get; set; }
        [JsonProperty("PorcentajeIva")]
        public char? PorcentajeIva { get; set; }
        [JsonProperty("IPAutorizada")]
        public string? IPAutorizada { get; set; }
        [JsonProperty("Recaudo")]
        public bool Recaudo { get; set; }
        [JsonProperty("PermitirACH")]
        public bool PermitirACH { get; set; }
        [JsonProperty("Cupo")]
        public bool Cupo { get; set; }
        [JsonProperty("ValorMinPago")]
        public decimal? ValorMinPago { get; set; }
        [JsonProperty("ValorMaxPago")]
        public decimal? ValorMaxPago { get; set; }
        [JsonProperty("CodigoRUNT")]
        public long? CodigoRUNT { get; set; }
        [JsonProperty("ActivarCV")]
        public int ActivarCV { get; set; }
        [JsonProperty("ActivoFacturacion")]
        public bool ActivoFacturacion { get; set; }
        [JsonProperty("ParamScoreHuella")]
        public int ParamScoreHuella { get; set; }
        [JsonProperty("ValidaCotejoHuellas")]
        public int ValidaCotejoHuellas { get; set; }
        [JsonProperty("SaveHuellas")]
        public int SaveHuellas { get; set; }
        [JsonProperty("AntFraude")]
        public int AntFraude { get; set; }
        [JsonProperty("OCR")]
        public int OCR { get; set; }
        [JsonProperty("POActivo")]
        public int POActivo { get; set; }
        [JsonProperty("Version")]
        public int? Version { get; set; }
        [JsonProperty("ValorCD")]
        public decimal ValorCD { get; set; }
        [JsonProperty("ValidaLockCentro")]
        public int ValidaLockCentro { get; set; }
        [JsonProperty("OnDispositivos")]
        public int OnDispositivos { get; set; }
        [JsonProperty("AlertaValMan")]
        public int AlertaValMan { get; set; }
        [JsonProperty("OnValManual")]
        public int OnValManual { get; set; }
    }
}

