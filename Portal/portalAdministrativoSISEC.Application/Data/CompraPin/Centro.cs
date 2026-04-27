using System.Collections.Generic;

namespace portalAdministrativoSISEC.Application.Data.CompraPin
{
    public class Centro
    {
        public long IdCentro { get; set; }
        public string? Nombre { get; set; } = "";
        public long IdComercio { get; set; }
        public int IdDepartamento { get; set; }
        public int IdMunicipio { get; set; }
        public int IdZona { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Fijo { get; set; }
        public string? Movil { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public long? CodigoRUNT { get; set; }
        public string? Busqueda { get; set; }
        public List<string>? Categorias { get; set; }

        public string? ValorSugerido { get; set; }
    }
}

