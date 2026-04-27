using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Application.Data.CompraPin.CDA
{
    public class PagoPinCDA
    {

        public DatosBasicosCDA Usuario { get; set; } = new DatosBasicosCDA();

        public int ClienteCompra { get; set; } = 0;

        public int? TipoTramite { get; set; } = 0;

        public int? TipoTramite2 { get; set; }

        public int? OpcionTramite { get; set; }

        public bool ValidacionMapa { get; set; }

        public Centro CentroSeleccionado { get; set; } = new Centro();

        public List<Convenio> ConveniosPagoEfectivo { get; set; }

        public List<MedioPago> MediosPago { get; set; }

        public List<Bancos> BancosPSE { get; set; }

        public List<Banco> BancosPSESecure { get; set; }

        public List<CostoCuotaCDA> CostoCuotas { get; set; }

        public int CostoCuotasSeleccionadas { get; set; }

        public DiscriminadoValorPinCDA ValorDiscriminadoCotizacion { get; set; } = new DiscriminadoValorPinCDA();

        public DiscriminadoValorPinCDA ValorDiscriminadoCotizacionAprox { get; set; }

        public int CategoriaVehiculo { get; set; }

        public string Categoria1 { get; set; }

        public string Categoria2 { get; set; }

        public string CategoriasActual { get; set; }

        public DateTime? FechaMatriculaVehiculo { get; set; }    

        public int EdadVehiculo { get; set; } = 0;

        public int? TipoRecaudoCtrl { get; set; }

        public int? TipoPagoEfectivo { get; set; }

        public string NomenclaturaIdentificacion { get; set; }

        public string TipoPersonaPSE { get; set; }

        public string TipoBancoPSE { get; set; }

        public int? Cuotas { get; set; }

        public string ReferenciaGenerada { get; set; }

        public bool PasarelaTuPago { get; set; }

        public bool UrlCentro { get; set; }

        public List<Categoria> Categorias { get; set; }

        public bool ObtenerPagoCrc { get; set; }

        public PasosCompraPin PasoCotizacion { get; set; }

        public List<TipoDocumentoPtesaDTO> TiposDeDocumento { get; set; }

        public string Host { get; set; }

        public bool Validarlogueado { get; set; }

        public bool TieneConvenios { get; set; }

        public bool CentroIpValidacion { get; set; }

        public bool CentroIpVolver { get; set; }

        public List<Categoria> CategoriasCentroIp { get; set; }

        public Dictionary<string, int> Plantillas { get; set; }

        public Analytics Analytics { get; set; }

        public PilotoCea PilotoCEA { get; set; }

        public int RegistroMiPerfil { get; set; }

        public List<Categoria> CategoriasCrc { get; set; }

        public List<Categoria> CategoriasCea { get; set; } = new List<Categoria>();

        public bool TramiteInstructor { get; set; }

        public string PinGenerado { get; set; } = "";

        public string CategoriaSeleccionada { get; set; }

        public bool PreSeleccionado { get; set; }

        public bool MayorEdad { get; set; }

        public string? TipoVehiculo { get; set; } 
        public string TipoCategoria { get; set; } 
        public string PlacaVehiculo { get; set; }  
        public bool IsValidDatosBasicos { get; set; }
 
        public bool IsLoadingTemp { get; set; }

    }
}

