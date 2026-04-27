using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Pago.Wompi
{
	public class Data
	{
		public string AcceptanceToken { get; set; } = "";
		public string Apellidos { get; set; } = "";
		public string Categoria { get; set; } = "";
		public string CorreoElectronico { get; set; } = "";
		public int CostoExtraRecaudo { get; set; }
		public List<Cuota> Cuotas { get; set; } = new();
		public int DispersionAliado { get; set; }
		public int DispersionAns { get; set; }
		public int DispersionCrc { get; set; }
		public int DispersionSicov { get; set; }
		public int EstadoWompi { get; set; }
		public string FechaNacimiento { get; set; } = "";
		public int IdConvenio { get; set; }
		public int IdOrigenCotizacion { get; set; }
		public string IdRunt { get; set; } = "";
		public string Nombres { get; set; } = "";
		public string NumeroIdentificacion { get; set; } = "";
		public int Sexo { get; set; }
		public string TelefonoContacto { get; set; } = "";
		public int TipoIdentificacion { get; set; }
		public Tramite Tramite { get; set; } = new();
		public string UrlRedireccion { get; set; } = "";
		public int ValorTransaccion { get; set; }

        // Campos de facturación
        public string ApellidosFacturacion { get; set; } = "";
        public string CorreoFacturacion { get; set; } = "";
        public string NombreComercialFacturacion { get; set; } = "";
        public string NombresFacturacion { get; set; } = "";
        public string NumeroIdentificacionFacturacion { get; set; } = "";
        public string RazonSocialFacturacion { get; set; } = "";
        public int TipoIdentificacionFacturacion { get; set; }
        public int TipoPersonaFacturacion { get; set; }
    }
}