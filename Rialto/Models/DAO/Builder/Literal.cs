using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public class Literal : QueryParts
    {
        public string Value { get; }
        public Literal(string value)
        {
            Value = value;
        }
        public string ToSqlString() => Value;
    }
}
