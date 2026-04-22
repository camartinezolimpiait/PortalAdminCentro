namespace portalAdministrativoSISEC.Data
{
    public class Municipios
    {
        public int IdMunicipio { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int IdDepartamento { get; set; }
        public string Municipio { get; set; } = string.Empty;
    }
}
