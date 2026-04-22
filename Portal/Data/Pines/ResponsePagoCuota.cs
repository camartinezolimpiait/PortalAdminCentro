using System;

namespace portalAdministrativoSISEC.Data.Pines
{
    public class ResponsePagoCuota<T> where T : class
    {
        public RespuestaGeneric<T> Respuesta { get; set; }
        public string Nut { get; set; } = "";
        public string FechaPago { get; set; } = "";
    }

    public class RespuestaGeneric<T> where T : class
    {
        public int Codigo { get; set; }
        public string Respuesta { get; set; } = "";
        public string NumeroAuditoria { get; set; } = Guid.NewGuid().ToString();
        public T Entidad { get; set; }
    }
}
