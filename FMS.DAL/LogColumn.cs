using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.EntitiesDAL
{
    public class LogColumn
    {
        public List<string> getColumn(string tableName)
        {
            List<string> columns = new List<string>();

            if (tableName.Equals("MasterList"))
            {
                columns.Add("List Name");
                columns.Add("List Value");
                columns.Add("List Status");
                columns.Add("FieldName");
                columns.Add("FieldValue");
                columns.Add("IsActive");
            }

            return columns;
        }
    }
}
