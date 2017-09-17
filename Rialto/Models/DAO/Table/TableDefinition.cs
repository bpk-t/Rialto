using Rialto.Models.DAO.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class TableDefinition : QueryParts
    {
        public string TableName { get; }

        public TableDefinition(string tableName)
        {
            TableName = tableName;
        }

        public string ToSqlString() => TableName;
    }
}
