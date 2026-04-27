using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class DataContact
    {
        public int IdCentro { get; set; }
        public string Nombre { get; set; }
        public string CodigoUso { get; set; }
        public int IdComercio { get; set; }
        public int IdDepartamento { get; set; }
        public int IdMunicipio { get; set; }
        public int IdZona { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Fijo { get; set; }
        public string Movil { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int Concurrencia { get; set; }
        public bool? PatrimonioAutonomo { get; set; }
        public bool? ActivoRUNT { get; set; }
        public bool? Activo { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public int RowVersion { get; set; }
        public Guid? ApplicationUser { get; set; }
        public string MatriculaMercantil { get; set; }
        public string Resolucion { get; set; }
        public string NumeroRegistro { get; set; }
        public int CapacidadMaxima { get; set; }
        public string ConvenioBanco { get; set; }
        public string ValorSugerido { get; set; }
        public string ValorComision { get; set; }
        public char? PorcentajeIva { get; set; }
        public string IPAutorizada { get; set; }
        public bool Recaudo { get; set; }
        public bool PermitirACH { get; set; }
        public bool Cupo { get; set; }
        public decimal? ValorMinPago { get; set; }
        public decimal? ValorMaxPago { get; set; }
        public long? CodigoRUNT { get; set; }
        public int ActivarCV { get; set; }
        public bool ActivoFacturacion { get; set; }
        public int ParamScoreHuella { get; set; }
        public int ValidaCotejoHuellas { get; set; }
        public int SaveHuellas { get; set; }
        public int AntFraude { get; set; }
        public int OCR { get; set; }
        public int POActivo { get; set; }
        public int? Version { get; set; }
        public decimal ValorCD { get; set; }
        public int ValidaLockCentro { get; set; }
        public int OnDispositivos { get; set; }
        public int AlertaValMan { get; set; }
        public int OnValManual { get; set; }
        public bool AcreditadoOnac { get; set; }
        public string plataforma { get; set; }
    }

    public class GetCentroRequest
    {
        public int ID { get; set; }
        public string Plataforma { get; set; }
    }

    public class CentroResponse
    {
        public int codigoRespuesta { get; set; }
        public string RespuestaTexto { get; set; }
    }
    public class GetComercioResponse : CentroResponse
    {
        public GetDataResponseComercio Respuesta { get; set; }
    }

    public class GetCentroResponse : CentroResponse
    {
        public GetDataResponseCentro Respuesta { get; set; }
    }

    public class GetDataResponseCentro
    {
        public int IdCentro { get; set; }
        public string Nombre { get; set; }
        public string CodigoUso { get; set; }
        public int IdComercio { get; set; }
        public int IdDepartamento { get; set; }
        public int IdMunicipio { get; set; }
        public int IdZona { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Fijo { get; set; }
        public string Movil { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int Concurrencia { get; set; }
        public bool? PatrimonioAutonomo { get; set; }
        public bool? ActivoRUNT { get; set; }
        public bool? Activo { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public int RowVersion { get; set; }
        public Guid? ApplicationUser { get; set; }
        public string MatriculaMercantil { get; set; }
        public string Resolucion { get; set; }
        public string NumeroRegistro { get; set; }
        public int CapacidadMaxima { get; set; }
        public string ConvenioBanco { get; set; }
        public string ValorSugerido { get; set; }
        public string ValorComision { get; set; }
        public char? PorcentajeIva { get; set; }
        public string? IPAutorizada { get; set; }
        public bool Recaudo { get; set; }
        public bool PermitirACH { get; set; }
        public bool Cupo { get; set; }
        public decimal? ValorMinPago { get; set; }
        public decimal? ValorMaxPago { get; set; }
        public long? CodigoRUNT { get; set; }
        public int ActivarCV { get; set; }
        public bool ActivoFacturacion { get; set; }
        public int ParamScoreHuella { get; set; }
        public int ValidaCotejoHuellas { get; set; }
        public int SaveHuellas { get; set; }
        public int AntFraude { get; set; }
        public int OCR { get; set; }
        public int POActivo { get; set; }
        public int? Version { get; set; }
        public decimal ValorCD { get; set; }
        public int ValidaLockCentro { get; set; }
        public int OnDispositivos { get; set; }
        public int AlertaValMan { get; set; }
        public int OnValManual { get; set; }
        public bool AcreditadoOnac { get; set; }
    }

    public class GetDataResponseComercio
    {
        public int IdComercio { get; set; }
        public string Nit { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string Contacto { get; set; }
        public string Direccion { get; set; }
        public string TelefonoFijo { get; set; }
        public string Celular { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
        public System.Guid ApplicationUser { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public int RowVersion { get; set; }
        public bool EsFiducia { get; set; }
    }


}

