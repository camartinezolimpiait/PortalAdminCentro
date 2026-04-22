using System.Collections.Generic;
using System.Globalization;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class VigiladoFilterDto
    {
        public List<DataFilter> data { get; set; }

    }
    public class DataFilter
    {
        public int id { get; set; }
        public AttributesVig attributes { get; set; }
    }

    public class AttributesVig
    {
        public string NIT { get; set; }
        public string razon_social { get; set; }
    }
}
