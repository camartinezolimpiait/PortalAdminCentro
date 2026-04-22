// Inicio código generado por GitHub Copilot
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Facturacion
{
    /// <summary>
    /// Wrapper para manejar resultados paginados de DetallePIN
    /// </summary>
    public class ResultadoPaginadoDetalleFacturacion
    {
        public List<PINDetail> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
    }
}
// Fin código generado por GitHub Copilot