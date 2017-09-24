using Rialto.Models.DAO.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rialto.Models.DAO.Builder.QueryBuilder;

namespace Rialto.Models.DAO.Builder
{
    public class ConditionBuilder
    {
        private ConditionBuilder() { }

        public static Condition Eq(string operand1, string operand2) => new ConditionEq(operand1, operand2);
        public static Condition NotEq(string operand1, string operand2) => new ConditionNotEq(operand1, operand2);

        /// <summary>
        /// operand1 > operand2
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <returns></returns>
        public static Condition Gt(string operand1, string operand2) => new ConditionGt(operand1, operand2);

        /// <summary>
        /// operand1 >= operand2
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <returns></returns>
        public static Condition Ge(string operand1, string operand2) => new ConditionGe(operand1, operand2);

        /// <summary>
        /// operand1 < operand2
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <returns></returns>
        public static Condition Lt(string operand1, string operand2) => new ConditionLt(operand1, operand2);

        /// <summary>
        /// operand1 <= operand2
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <returns></returns>
        public static Condition Le(string operand1, string operand2) => new ConditionLe(operand1, operand2);

        public static Condition Like(string operand1, string operand2) => new ConditionLike(operand1, operand2);


        public static Condition Exists(SelectQuery query) => new ExistsCondition(query);
        public static Condition NotExists(SelectQuery query) => new NotExistsCondition(query);
    }

    public abstract class Condition : QueryParts
    {
        public abstract string ToSqlString();
    }

    public abstract class TwoOperandCondition : Condition
    {
        public string Operand1 { get; }
        public string Operand2 { get; }
        public TwoOperandCondition(string operand1, string operand2)
        {
            Operand1 = operand1;
            Operand2 = operand2;
        }
    }

    // TODO エスケープ処理
    public class ConditionEq : TwoOperandCondition
    {
        public ConditionEq(string operand1, string operand2) : base(operand1, operand2) { }
        public override string ToSqlString() => $"({Operand1} = {Operand2})";
    }

    // TODO エスケープ処理
    public class ConditionNotEq : TwoOperandCondition
    {
        public ConditionNotEq(string operand1, string operand2) : base(operand1, operand2) { }
        public override string ToSqlString() => $"({Operand1} <> {Operand2})";
    }

    // TODO エスケープ処理
    public class ConditionGt : TwoOperandCondition
    {
        public ConditionGt(string operand1, string operand2) : base(operand1, operand2) { }
        public override string ToSqlString() => $"({Operand1} > {Operand2})";
    }

    // TODO エスケープ処理
    public class ConditionGe : TwoOperandCondition
    {
        public ConditionGe(string operand1, string operand2) : base(operand1, operand2) { }
        public override string ToSqlString() => $"({Operand1} >= {Operand2})";
    }

    // TODO エスケープ処理
    public class ConditionLt : TwoOperandCondition
    {
        public ConditionLt(string operand1, string operand2) : base(operand1, operand2) { }
        public override string ToSqlString() => $"({Operand1} < {Operand2})";
    }

    public class ConditionLe : TwoOperandCondition
    {
        public ConditionLe(string operand1, string operand2) : base(operand1, operand2) { }
        public override string ToSqlString() => $"({Operand1} <= {Operand2})";
    }

    public class ConditionLike : TwoOperandCondition
    {
        public ConditionLike(string operand1, string operand2) : base(operand1, operand2) { }
        public override string ToSqlString() => $"({Operand1} LIKE {Operand2})";
    }

    public class And : Condition
    {
        private List<Condition> conditions = new List<Condition>();
        public And(params Condition[] conditions)
        {
            this.conditions.AddRange(conditions);
        }
        public override string ToSqlString() => $"({conditions.Select(x => x.ToSqlString()).Aggregate((acc, x) => acc + " AND " + x)})";
    }
    public class Or : Condition
    {
        private List<Condition> conditions = new List<Condition>();
        public Or(params Condition[] conditions)
        {
            this.conditions.AddRange(conditions);
        }
        public override string ToSqlString() => $"({conditions.Select(x => x.ToSqlString()).Aggregate((acc, x) => acc + " OR " + x)})";
    }

    public class ExistsCondition : Condition
    {
        public SelectQuery SelectQuery { get; }
        public ExistsCondition(SelectQuery query)
        {
            SelectQuery = query;
        }

        public override string ToSqlString() => $"(EXISTS ({SelectQuery.ToSqlString()}))";
    }

    public class NotExistsCondition : Condition
    {
        public SelectQuery SelectQuery { get; }
        public NotExistsCondition(SelectQuery query)
        {
            SelectQuery = query;
        }

        public override string ToSqlString() => $"(NOT EXISTS ({SelectQuery.ToSqlString()}))";
    }
}
