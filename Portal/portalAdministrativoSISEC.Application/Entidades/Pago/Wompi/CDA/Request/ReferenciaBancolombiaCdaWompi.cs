namespace portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Request
{
	public class ReferenciaBancolombiaCdaWompi
	{
		public class RequestCreacionPinCDA
		{
			public int IdCliente { get; set; }
			public DataCda Data { get; set; }
		}

		public class DataCda
		{
			public string IdRunt { get; set; }
			public string Nombres { get; set; }
			public string Apellidos { get; set; }
			public int TipoIdentificacion { get; set; }
			public string NombreCentro { get; set; }
			public string DireccionCentro { get; set; }

			public string NumeroIdentificacion { get; set; }
			public string CorreoElectronico { get; set; }
			public string FechaNacimiento { get; set; }
			public int Sexo { get; set; }
			public string TelefonoContacto { get; set; }
			public int DispersionAliado { get; set; }
			public int DispersionAns { get; set; }
			public int DispersionSicov { get; set; }
			public int DispersionCrc { get; set; }
			public int ValorTransaccion { get; set; }
			public TramiteCda Tramite { get; set; }
			public int IdOrigenCotizacion { get; set; }
			public int IdConvenio { get; set; }
			public InfoVehiculo InfoVehiculo { get; set; }
		}

		public class InfoVehiculo
		{
			public int IdTipoVehiculo { get; set; }
			public string NumPlaca { get; set; }
			public int AnioVehiculo { get; set; }
		}

		public class TramiteCda
		{
			public int Categoria1 { get; set; }
			public int Categoria2 { get; set; }
			public int IdTramite1 { get; set; }
			public int IdTramite2 { get; set; }
			public string CodigoCategoria1 { get; set; }
			public string CodigoCategoria2 { get; set; }
		}
	}
}
