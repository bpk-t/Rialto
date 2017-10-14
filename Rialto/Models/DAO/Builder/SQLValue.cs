using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public class SQLValue<T> : QueryParts
    {
        public T Value { get; }
        public string UUID { get; }

        public SQLValue(T value)
        {
            Value = value;
            UUID = Guid.NewGuid().ToString().Replace("-", "").Take(6).ToString();
        }

        public string ToSqlString()
        {
            return $"@{UUID}";
        }
    }
}
