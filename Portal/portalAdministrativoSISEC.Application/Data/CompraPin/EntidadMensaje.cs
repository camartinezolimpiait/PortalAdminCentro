	namespace portalAdministrativoSISEC.Application.Data.CompraPin
	{
		public class EntidadMensaje
		{
			public EntidadAdjunto[] Adjuntos { get; set; }

			public string Aplicacion { get; set; }
			public string Asunto { get; set; }

			public string CodigoPlantilla { get; set; }

			public EntidadDestinatario[] Destinatarios { get; set; }

			public EntidadVariable[] Variables { get; set; }
		}

		public class EntidadAdjunto
		{
			public string Ruta { get; set; }
		}

		public class EntidadDestinatario
		{
			public string Correo { get; set; }
			public string Identificacion { get; set; }
			public int Tipo { get; set; }
		}

		public class EntidadVariable
		{
			public string Nombre { get; set; }
			public string Valor { get; set; }
		}
	}
	

