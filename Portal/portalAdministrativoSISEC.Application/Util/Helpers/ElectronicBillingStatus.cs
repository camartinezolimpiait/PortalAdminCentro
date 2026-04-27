// Inicio código generado por GitHub Copilot
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System;

namespace portalAdministrativoSISEC.Util.Helpers
{
    /// <summary>
    /// Helper para manejo de estados de facturación electrónica
    /// Centraliza la lógica de mapeo entre estados del backend y la UI
    /// </summary>
    public class ElectronicBillingStatus
    {
        /// <summary>
        /// Mapea un estado del backend (string) al enum correspondiente
        /// </summary>
        /// <param name="estadoBackend">Estado que viene del API</param>
        /// <returns>Valor del enum correspondiente</returns>
        public static EnumEstadoFacturacionElectronica MapearDesdeEstadoBackend(string estadoBackend)
        {
            // Método generado por GitHub Copilot
            return estadoBackend?.ToLower() switch
            {
                // Estados que mapean a "Por revisar"
                "en error de configuración" => EnumEstadoFacturacionElectronica.PorRevisar,
                "en error de conexión" => EnumEstadoFacturacionElectronica.PorRevisar,

                // Estados que mapean a "En cola"
                "registrado" => EnumEstadoFacturacionElectronica.EnCola,
                "listo para procesar" => EnumEstadoFacturacionElectronica.EnCola,
                "encolados" => EnumEstadoFacturacionElectronica.EnCola,
                "procesando" => EnumEstadoFacturacionElectronica.EnCola,

                // Estados que mapean a "Facturadas"
                "facturada" => EnumEstadoFacturacionElectronica.Facturadas,

                // Estados que mapean a "Anuladas"
                "anulado" => EnumEstadoFacturacionElectronica.Anuladas,

                // Por defecto: Por revisar
                _ => EnumEstadoFacturacionElectronica.PorRevisar
            };
        }

        /// <summary>
        /// Obtiene el nombre para mostrar en las pestañas según el enum
        /// </summary>
        /// <param name="estado">Estado del enum</param>
        /// <returns>Nombre formateado para mostrar en UI</returns>
        public static string GetEstadoDisplayName(EnumEstadoFacturacionElectronica estado)
        {
            // Método generado por GitHub Copilot
            return estado switch
            {
                EnumEstadoFacturacionElectronica.PorRevisar => "Por revisar",
                EnumEstadoFacturacionElectronica.Facturadas => "Facturadas",
                EnumEstadoFacturacionElectronica.Anuladas => "Anuladas",
                EnumEstadoFacturacionElectronica.EnCola => "En cola",
                _ => estado.ToString()
            };
        }

        /// <summary>
        /// Obtiene la clase CSS del badge según el estado mapeado a pestaña
        /// Este es el método para obtener la clase CSS según la pestaña agrupada
        /// </summary>
        /// <param name="estadoBackend">Estado original del backend</param>
        /// <returns>Clase CSS del badge según la pestaña correspondiente</returns>
        public string GetEstadoBadgeClassPorPestana(string estadoBackend)
        {
            // Método generado por GitHub Copilot
            if (string.IsNullOrWhiteSpace(estadoBackend))
                return "badge badge-warning";

            var estadoEnum = MapearDesdeEstadoBackend(estadoBackend);
            return ObtenerClaseBadge(estadoEnum);
        }

        /// <summary>
        /// Obtiene la clase CSS del badge según el estado del enum
        /// Retorna la clase base "badge" seguida del modificador de color
        /// </summary>
        /// <param name="estado">Estado del enum</param>
        /// <returns>Clase CSS de DaisyUI para el badge</returns>
        public string ObtenerClaseBadge(EnumEstadoFacturacionElectronica estado)
        {
            // Método generado por GitHub Copilot
            return estado switch
            {
                EnumEstadoFacturacionElectronica.PorRevisar => "badge badge-warning",
                EnumEstadoFacturacionElectronica.EnCola => "badge badge-info",
                EnumEstadoFacturacionElectronica.Facturadas => "badge badge-success",
                EnumEstadoFacturacionElectronica.Anuladas => "badge badge-neutral",
                _ => "badge"
            };
        }
    }
}
// Fin código generado por GitHub Copilot