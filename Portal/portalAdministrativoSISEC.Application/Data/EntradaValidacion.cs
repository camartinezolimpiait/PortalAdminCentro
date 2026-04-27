using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class EntradaValidacion
    {
        public List<String> ListKeyBiometrias { get; set; }
        public string Base64FormatoAutorizacionFD { get; set; }
        public string NumeroDocumentoCandidato { get; set; }
        public string TipoIdentificacionCandidato { get; set; }
        public string UsuarioCreacion { get; set; }
        public int idClientePlataforma { get; set; }
        public int IdCentro { get; set; }
        public long idProceso { get; set; }
        public string codigoProducto { get; set; }
    }
}

