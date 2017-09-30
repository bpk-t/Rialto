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

        public static InsertQuery Insert()
        {
            return new InsertQuery();
        }
    }
}
