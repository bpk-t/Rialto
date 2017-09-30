using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public class InsertQuery : QueryParts
    {
        private static readonly object NullObject = "";
        private string tableName;
        private dynamic queryParam = new ExpandoObject();
        private LangExt.Option<SelectQuery> selectQuery = LangExt.Option.None;

        public InsertQuery Into(string tableName)
        {
            this.tableName = tableName;
            return this;
        }

        public InsertQuery Into(TableDefinition table)
        {
            this.tableName = table.TableName;
            return this;
        }

        public InsertQuery Into(string tableName, params string[] columns)
        {
            this.tableName = tableName;
            columns.ForEach(x => Set(x, NullObject));
            return this;
        }

        public InsertQuery Into(TableDefinition table, params ColumnDefinition[] columns)
        {
            this.tableName = table.TableName;
            columns.ForEach(x => Set(x, NullObject));
            return this;
        }

        public InsertQuery Set<T>(string column, T value)
        {
            (queryParam as IDictionary<string, T>).Add(column, value);
            return this;
        }

        public InsertQuery Set<T>(ColumnDefinition column, T value)
        {
            (queryParam as IDictionary<string, T>).Add(column.ColumnName, value);
            return this;
        }

        public InsertQuery Select(SelectQuery query)
        {
            selectQuery = LangExt.Option.Create(query);
            return this;
        }

        public string ToSqlString()
        {
            return $"INSERT INTO {tableName}"
                + "(" + (queryParam as IDictionary<string, object>).Select(x => x.Key).Aggregate((acc, x) => acc + "," + x) + ") "
                + selectQuery.Match<string>(
                    (query) => query.ToSqlString(),
                    () => "VALUES(" + (queryParam as IDictionary<string, object>).Select(x => x.Key).Aggregate((acc, x) => acc + ",@" + x) + ") "
                    );
        }
    }
}
