using System;
using System.Drawing;
namespace portalAdministrativoSISEC.Data.Agendamiento

{
    public class RequestNotificacionCitas
    {
        public string TipoCliente { get; set; }
        public string Plataforma { get; set; }
        public string Aspirante { get; set; }
        public string NombreCentro { get; set; }
        public string Correo { get; set; }
        public string NumeroTelefono { get; set; }
        public string Tramite { get; set; }
        public string Categoria { get; set; }
        public string FechaAgenda { get; set; }
        public string HoraAgenda { get; set; }

    }
}
