using System;

namespace portalAdministrativoSISEC.Application.Data.CompraPin.CDA
{
	public class DatosBasicosCDA
	{
		public int? Id { get; set; }

		public string Nombre { get; set; } = "";

		public string Apellido { get; set; } = "";

		public int? TipoDocumento { get; set; } = 0;

		public string NumDocumento { get; set; } = "";

		public DateTime? FechaNacimiento { get; set; }

		public string Correo { get; set; } = "";

		public long? Celular { get; set; } = 0;

		public int? Genero { get; set; } = 0;

		public string TipoDocumentoDescpcion { get; set; } = "";
	}
}

