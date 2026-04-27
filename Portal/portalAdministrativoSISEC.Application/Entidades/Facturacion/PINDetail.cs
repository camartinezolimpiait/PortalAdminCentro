// Inicio código generado por GitHub Copilot
using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{

    public class DataWrapper<T> where T : class
    {
        public T Datos { get; set; }
        public object Errores { get; set; }
        public string Mensaje { get; set; }
        public bool SolicitudExitosa { get; set; }
        public int StatusCode { get; set; }
    }

    // Para la respuesta paginada (JSON 1 y 2)
    public class PaginatedData
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<PINDetail> Resultado { get; set; }
        public int TotalEncontrados { get; set; }
        public int TotalPaginados { get; set; }
    }

    // Para la respuesta de historial (JSON 2 del documento anterior)
    public class HistoricData
    {
        public List<HistoryStatusPin> HistorialEstados { get; set; }
        public RequestResume ResumenPeticion { get; set; }
    }
    public class PINDetail
    {
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaFacturacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdDetallePIN { get; set; }
        public int IdPtesaPIN { get; set; }
        public string Nombres { get; set; }
        public string NumeroFactura { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string OperadorRecaudo { get; set; }
        public string Pin { get; set; }
        public string TipoDocumento { get; set; }
        public int TotalRegistros { get; set; }
        public decimal ValorTotal { get; set; }
        public bool Seleccionado { get; set; } // Propiedad para selección múltiple en el QuickGrid
        public int IdEstado { get; set; }
        public int IdEstadoML { get; set; }
        public string EstadoML { get; set; }
        public string urlPdfFactura { get; set; }
    }

    public class HistoryStatusPin
    {
        public string CodigoError { get; set; }
        public string DescripcionError { get; set; }
        public bool EsActivo { get; set; }
        public string EstadoAsociado { get; set; }
        public DateTime? FechaEventoDetalle { get; set; }
        public DateTime FechaRegistroEstado { get; set; }
        public DateTime? FechaSolucion { get; set; }
        public int IdCentro { get; set; }
        public int IdDetallePIN { get; set; }
        public int IdEstado { get; set; }
        public int IdHistorialEstadosPin { get; set; }
        public string NivelEvento { get; set; }
        public bool TieneEventos { get; set; }
        public string TipoEvento { get; set; }
    }

    public class RequestResume
    {
        public string EstadoActual { get; set; }
        public DateTime FechaUltimoEstado { get; set; }
        public string NumeroFactura { get; set; }
        public string Pin { get; set; }
    }
}
// Fin código generado por GitHub Copilot
