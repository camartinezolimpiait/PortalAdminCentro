using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class RespuestaPutCentro
    {
        public DataCentro data { get; set; }
    }
    public class DataCentro
    {
        public int id { get; set; }
        public AttributesCentro attributes { get; set; }
    }
    public class AttributesCentro
    {
        public string IDRUNT { get; set; }
        public string nombre { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool de_prueba { get; set; }
    }
}
