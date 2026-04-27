using Microsoft.Identity.Client;
using portalAdministrativoSISEC.Enum;
// Inicio código generado por GitHub Copilot
using portalAdministrativoSISEC.Entidades.Facturacion;
// Fin código generado por GitHub Copilot
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Application.Data.CompraPin
{
	public class PagoPin
	{
		public DatosBasicos Usuario { get; set; } = new DatosBasicos();

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

		public List<CostoCuota> CostoCuotas { get; set; }

		public int CostoCuotasSeleccionadas { get; set; }

		public DiscriminadoValorPin ValorDiscriminadoCotizacion { get; set; } = new DiscriminadoValorPin();

		public DiscriminadoValorPin ValorDiscriminadoCotizacionAprox { get; set; }

		public string Categoria { get; set; } = "";

		public string Categoria1 { get; set; }

		public string Categoria2 { get; set; }

		public string CategoriasActual { get; set; }

		public int EdadAspirante { get; set; } = 0;

		public int? TipoRecaudoCtrl { get; set; }

		public int? TipoPagoEfectivo { get; set; }

		public string NomenclaturaIdentificacion { get; set; }

		public string TipoPersonaPSE { get; set; }

		public string TipoBancoPSE { get; set; }

		public int? Cuotas { get; set; }

		public string ReferenciaGenerada { get; set; }

		public bool PasarelaTuPago { get; set; }

		public bool UrlCentro { get; set; }

		public List<Categoria> Categorias { get; set; } = new List<Categoria>();

		public bool ObtenerPagoCrc { get; set; }

		public PasosCompraPin PasoCotizacion { get; set; }

		// Inicio código generado por GitHub Copilot
		public List<TipoDocumentoPtesaDTO> TiposDeDocumento { get; set; }

		public List<ConsultaGenericaTipos> TiposPersonaFacturacion { get; set; } = new();
		// Fin código generado por GitHub Copilot

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
		public bool IsValidDatosBasicos { get; set; }
		public ConfiguracionCuotas ConfiguracionCuotas { get; set; } = new(); 

	    public bool ResumenCompraExito { get; set; }
		public string? Nut { get; set; } = string.Empty;
        public string? FechaPago { get; set; } =string.Empty;
		public bool FacturacionActiva { get; set; } = false;
        // Inicio código generado por GitHub Copilot
		public int MomentoFacturacion { get; set; } = (int)portalAdministrativoSISEC.Enum.PortalAdministrativo.EnumEventoFacturacion.UsoPIN;
        // Fin código generado por GitHub Copilot
        public bool EmisionOtraPersona { get; set; }
        public DatosFacturacion DatosFacturacion { get; set; }
        public string UrlRedireccion { get; set; }

		// Inicio código generado por GitHub Copilot
		/// <summary>
		/// Código del convenio aplicado
		/// </summary>
		public string CodigoConvenio { get; set; }

		/// <summary>
		/// Nombre de la empresa del convenio aplicado
		/// </summary>
		public string EmpresaConvenio { get; set; }
		// Fin código generado por GitHub Copilot
    }
}

