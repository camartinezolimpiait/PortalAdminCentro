using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class EntradaRegistroRostro
    {
        public int idTipoDocumento { get; set; }
        public string Numerodocumento { get; set; }
        public string ImagenRostroBase64 { get; set; }
        public int idCliente { get; set; }
        public int IdAfis { get; set; }
    }
}

