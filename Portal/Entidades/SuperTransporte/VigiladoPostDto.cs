using System.Collections.Generic;
using System.Globalization;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class VigiladoPostDto
    {
        public DataPost data { get; set; }

    }
    public class DataPost
    {
        public int id { get; set; }
        public AttributesPost attributes { get; set; }
    }

    public class AttributesPost
    {
        public string NIT { get; set; }
        public string razon_social { get; set; }
    }
}
