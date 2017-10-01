using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public class UpdateQuery : QueryParts
    {
        private string tableName;
        private dynamic queryParam = new ExpandoObject();
        private List<Condition> whereConditions = new List<Condition>();

        public UpdateQuery(string tableName)
        {
            this.tableName = tableName;
        }

        public UpdateQuery(TableDefinition table)
        {
            this.tableName = table.TableName;
        }

        public UpdateQuery Set<T>(string column, T value)
        {
            (queryParam as IDictionary<string, T>).Add(column, value);
            return this;
        }

        public UpdateQuery Set<T>(ColumnDefinition column, T value)
        {
            (queryParam as IDictionary<string, T>).Add(column.ColumnName, value);
            return this;
        }

        public UpdateQuery Where(Condition condition)
        {
            whereConditions.Add(condition);
            return this;
        }

        public UpdateQuery Where(params Condition[] conditions)
        {
            whereConditions.AddRange(conditions);
            return this;
        }

        public string ToSqlString()
        {
            return $"UPDATE {tableName} "
                + "SET " + (queryParam as IDictionary<string, object>).Select(x => x.Key).Aggregate((acc, x) => acc + $",{x}=@{x}")
                + (whereConditions.IsEmpty() ? string.Empty : (" WHERE " + (new And(whereConditions.ToArray())).ToSqlString()));
        }
    }
}
