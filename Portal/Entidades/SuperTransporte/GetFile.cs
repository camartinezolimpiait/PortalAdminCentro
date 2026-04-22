using System;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class GetFile
    {
        public DataFile data { get; set; }
    }

    public class DataFile
    {
        public int id { get; set; }
        public AttributesFile attributes { get; set; }

    }
    public class AttributesFile
    {
        public string name { get; set; }
        public DateTime createdAt { get; set; }
        public double size { get; set; }
        public string url { get; set; }
    }
}
