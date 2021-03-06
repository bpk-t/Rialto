﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public class ColumnDefinition : QueryParts
    {
        public TableDefinition Table { get; }
        public string ColumnName { get; }
        public string FullName
        {
            get => Table.TableName + "." + ColumnName;
        }

        public ColumnDefinition(TableDefinition table, string column)
        {
            Table = table;
            ColumnName = column;
        }

        public string ToSqlString() => FullName;

        public Condition Eq(string operand2) => new ConditionEq(this.ToSqlString(), operand2);
        public Condition Eq(Literal operand2) => new ConditionEq(this.ToSqlString(), operand2.ToSqlString());
        public Condition Eq(ColumnDefinition operand2) => new ConditionEq(this.ToSqlString(), operand2.ToSqlString());

        public Condition NotEq(string operand2) => new ConditionNotEq(this.ToSqlString(), operand2);
        public Condition NotEq(Literal operand2) => new ConditionNotEq(this.ToSqlString(), operand2.ToSqlString());
        public Condition NotEq(ColumnDefinition operand2) => new ConditionNotEq(this.ToSqlString(), operand2.ToSqlString());

        public Condition IsNull() => new ConditionIsNull(this.ToSqlString());
        public Condition IsNotNull() => new ConditionIsNotNull(this.ToSqlString());

        public OrderByItem Asc() => new OrderByItem(FullName, Order.Asc);
        public OrderByItem Desc() => new OrderByItem(FullName, Order.Desc);
    }
}
