using System.Data.Entity.Core.Metadata.Edm;

namespace FMS.BusinessObject.CustomEntityClass
{
    public class TableDetail
    {
        public string PropertyName { get; set; }
        public string TypeUsageName { get; set; }
        public Documentation Documentation { get; set; }

        public bool IsNullable { get; set; }

        public bool IsUniquePrimaryKey { get; set; }
    }
}
