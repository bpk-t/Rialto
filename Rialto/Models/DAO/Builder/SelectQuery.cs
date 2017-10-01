using Rialto.Models.DAO.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Builder
{
    public class SelectQuery : QueryParts
    {
        private List<string> selectColumns = new List<string>();
        private string tableName;
        private List<OrderByItem> orderByItems = new List<OrderByItem>();
        private List<Condition> whereConditions = new List<Condition>();
        private LangExt.Option<long> limit = LangExt.Option.None;
        private LangExt.Option<long> offset = LangExt.Option.None;

        private List<JoinTable> innerJoinTables = new List<JoinTable>();
        private List<JoinTable> leftOuterJoinTables = new List<JoinTable>();

        public SelectQuery Select(string column)
        {
            selectColumns.Add(column);
            return this;
        }

        public SelectQuery Select(params string[] columns)
        {
            this.selectColumns.AddRange(columns);
            return this;
        }

        public SelectQuery Select(params ColumnDefinition[] columns)
        {
            this.selectColumns.AddRange(columns.Select(x => x.ToSqlString()));
            return this;
        }

        public SelectQuery From(string table)
        {
            tableName = table;
            return this;
        }

        public SelectQuery From(TableDefinition tableDef)
        {
            tableName = tableDef.ToSqlString();
            return this;
        }

        public SelectQuery InnerJoin(TableDefinition table, Condition onCondition)
        {
            innerJoinTables.Add(new JoinTable(table.ToSqlString(), onCondition));
            return this;
        }

        public SelectQuery InnerJoin(string tableName, Condition onCondition)
        {
            innerJoinTables.Add(new JoinTable(tableName, onCondition));
            return this;
        }

        public SelectQuery LeftOuterJoin(string tableName, Condition onCondition)
        {
            leftOuterJoinTables.Add(new JoinTable(tableName, onCondition));
            return this;
        }

        public SelectQuery LeftOuterJoin(TableDefinition table, Condition onCondition)
        {
            leftOuterJoinTables.Add(new JoinTable(table.ToSqlString(), onCondition));
            return this;
        }

        public SelectQuery OrderBy(string column, Order order)
        {
            orderByItems.Add(new OrderByItem(column, order));
            return this;
        }

        public SelectQuery OrderBy(ColumnDefinition column, Order order)
        {
            orderByItems.Add(new OrderByItem(column.ToSqlString(), order));
            return this;
        }

        public SelectQuery Where(Condition condition)
        {
            whereConditions.Add(condition);
            return this;
        }

        public SelectQuery Where(params Condition[] conditions)
        {
            whereConditions.AddRange(conditions);
            return this;
        }

        public SelectQuery Limit(long limit)
        {
            this.limit = LangExt.Option.Some(limit);
            return this;
        }

        public SelectQuery Offset(long offset)
        {
            this.offset = LangExt.Option.Some(offset);
            return this;
        }

        public string ToSqlString()
        {
            return "SELECT " + (selectColumns.IsEmpty() ? "*" : selectColumns.Aggregate((acc, x) => acc + ", " + x))
                + " FROM " + tableName
                + (innerJoinTables.IsEmpty() ? string.Empty : innerJoinTables.Select(x => $" INNER {x.ToSqlString()}").Aggregate((acc, x) => acc + " " + x))
                + (leftOuterJoinTables.IsEmpty() ? string.Empty : leftOuterJoinTables.Select(x => $" LEFT OUTER {x.ToSqlString()}").Aggregate((acc, x) => acc + " " + x))
                + (whereConditions.IsEmpty() ? string.Empty : (" WHERE " + (new And(whereConditions.ToArray())).ToSqlString()))
                + (orderByItems.IsEmpty() ? string.Empty : " ORDER BY " + orderByItems.Select(x => x.ToSqlString()).Aggregate((acc, x) => acc + ", " + x))
                + limit.Match((x) => $" LIMIT {x}", () => string.Empty)
                + offset.Match((x) => $" OFFSET {x}", () => string.Empty);
        }
    }
}
