using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Data
{
    public class EmailSender
    {
        public Dictionary<string, string> LlaveValor { get; set; }
        public string CodigoPlantilla { get; set; }
        public List<Destinatarios> Destinatarios { get; set; }
    }

    public class Destinatarios
    {
        public string NameDestinatario { get; set; }
    }

    public enum PlantillasDeCorreo
    {
        RecuperarContrasena = 1
    }
}
