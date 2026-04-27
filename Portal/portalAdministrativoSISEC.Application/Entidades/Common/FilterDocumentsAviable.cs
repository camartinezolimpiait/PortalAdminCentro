using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System.Collections.Generic;
using System.Linq;

namespace portalAdministrativoSISEC.Entidades.Common
{
    public static class FilterDocumentsAviable
    {
       
    static int[] tiposAceptados = new[]
        {
            (int)EnumTipoDocumentoAceptados.CedulaCiudadania,           // 1
            (int)EnumTipoDocumentoAceptados.CédulaExtranjeria,         // 2
            (int)EnumTipoDocumentoAceptados.TarjetaIdentidad,          // 3
            (int)EnumTipoDocumentoAceptados.Pasaporte,                 // 5
            (int)EnumTipoDocumentoAceptados.PermisoPorProteccionTemporal // 13
        };
        public static List<TipoDocumentoPtesaDTO> FilterDocumentsTypes(List<TipoDocumentoPtesaDTO> ListaDocumentos)
        {
            return ListaDocumentos.Where(doc => tiposAceptados.Contains(int.Parse(doc.IdTipoSisec))).ToList();
        }

    }
}

