using Rialto.Models.DAO.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public interface QueryParts
    {
        string ToSqlString();
    }

    public class QueryBuilder
    {
        private QueryBuilder() { }

        public static SelectQuery Select()
        {
            return new SelectQuery();
        }

        public static SelectQuery Select(string column)
        {
            return new SelectQuery().Select(column);
        }

        public static SelectQuery Select(params string[] columns)
        {
            return new SelectQuery().Select(columns);
        }
    }

    public class InsertQuery : QueryParts
    {
        private string tableName;

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

        public InsertQuery Column(string column, string value)
        {
            return this;
        }

        public InsertQuery Column(ColumnDefinition column, string value)
        {
            return this;
        }

        public string ToSqlString()
        {
            throw new NotImplementedException();
        }
    }
}
