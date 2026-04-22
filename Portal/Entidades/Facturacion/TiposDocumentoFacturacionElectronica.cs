// Inicio código generado por GitHub Copilot
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    /// <summary>
    /// Representa un tipo de documento para facturación electrónica
    /// Obtenido del endpoint GetIdDocumentType
    /// </summary>
    public class TipoDocumentoFacturacionElectronica
    {
        /// <summary>
        /// ID del tipo de documento en SISEC
        /// </summary>
        public string IdTipoSisec { get; set; }

        /// <summary>
        /// Nombre completo del tipo de documento (ej: "Cédula de ciudadanía")
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Código del tipo de documento en RUNT
        /// </summary>
        public string TipoDocRunt { get; set; }

        /// <summary>
        /// Indica si se visualiza en CRC
        /// </summary>
        public int VisualizarCrc { get; set; }

        /// <summary>
        /// Indica si se visualiza en CEA
        /// </summary>
        public int VisualizarCea { get; set; }

        /// <summary>
        /// Edad mínima permitida para este tipo de documento
        /// </summary>
        public int EdadMinima { get; set; }

        /// <summary>
        /// Edad máxima permitida para este tipo de documento
        /// </summary>
        public int EdadMaxima { get; set; }

        /// <summary>
        /// Código ACH del tipo de documento (ej: "CC", "CE", "PP")
        /// Este es el valor que se usa para facturación electrónica
        /// </summary>
        public string CodigoACH { get; set; }

        /// <summary>
        /// ID del tipo en Bancolombia
        /// </summary>
        public string IdTipoBancolombia { get; set; }

        /// <summary>
        /// Código del tipo de documento en Daviplata
        /// </summary>
        public string TipoDocDaviplata { get; set; }

        /// <summary>
        /// Código del tipo de documento en Colpatria
        /// </summary>
        public string TipoDocColpatria { get; set; }

        /// <summary>
        /// Indica si este tipo de documento se visualiza en facturación electrónica
        /// </summary>
        public bool VisualizarFacturacionElectronica { get; set; }
    }

    /// <summary>
    /// Respuesta del endpoint GetIdDocumentType
    /// </summary>
    public class RespuestaTipoDocumento
    {
        /// <summary>
        /// Código de respuesta
        /// </summary>
        public int Codigo { get; set; }

        /// <summary>
        /// Mensaje de respuesta
        /// </summary>
        public string Respuesta { get; set; }

        /// <summary>
        /// Número de auditoría
        /// </summary>
        public string NumeroAuditoria { get; set; }

        /// <summary>
        /// Lista de tipos de documento
        /// </summary>
        public List<TipoDocumentoFacturacionElectronica> Entidad { get; set; }
    }
}
// Fin código generado por GitHub Copilot
