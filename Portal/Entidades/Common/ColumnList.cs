using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Common
{
    public class ColumnList
    {
        public string NameColumn { get; set; } = "";
        public string Value { get; set; } = "";
        public bool Sortable { get; set; } = true;

    }

    public class RowList
    {
        public List<ColumnList> Row { get; set; } = [];
    }
}