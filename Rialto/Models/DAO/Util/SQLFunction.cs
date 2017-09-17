using Rialto.Models.DAO.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Util
{
    public abstract class SQLFunction : QueryParts
    {
        public abstract string ToSqlString();
    }

    public class CountFunction : SQLFunction
    {
        private string operand;

        public CountFunction(string operand)
        {
            this.operand = operand;
        }

        public override string ToSqlString() => $"COUNT({operand})";
    }

    public class TrimFunction : SQLFunction
    {
        private string operand;

        public TrimFunction(string operand)
        {
            this.operand = operand;
        }

        public override string ToSqlString() => $"TRIM({operand})";
    }
    public class SQLFunctionBuilder
    {
        public static CountFunction Count(string operand) => new CountFunction(operand);
        public static TrimFunction Trim(string operand) => new TrimFunction(operand);
        public static TrimFunction Trim(ColumnDefinition operand) => new TrimFunction(operand.ToSqlString());
    }
}
