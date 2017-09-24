using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public class JoinTable : QueryParts
    {
        public string TableName { get; }
        public Condition OnCondition { get; }

        public JoinTable(string tableName, Condition onCondition)
        {
            this.TableName = tableName;
            this.OnCondition = onCondition;
        }

        public string ToSqlString() => $"JOIN {TableName} ON {OnCondition.ToSqlString()}";
    }
}
