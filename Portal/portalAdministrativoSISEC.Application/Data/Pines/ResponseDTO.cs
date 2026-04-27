using System;

namespace portalAdministrativoSISEC.Application.Data.Pines
{
	public class ResponseDTO<T> where T : class
	{
		public int Codigo { get; set; }
		public string Respuesta { get; set; } = "";
		public string NumeroAuditoria { get; set; } = Guid.NewGuid().ToString();
		public T Entidad { get; set; }
    }
}
