using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Util
{
    public enum Order
    {
        /// <summary>
        /// 照準
        /// </summary>
        Asc,

        /// <summary>
        /// 降順
        /// </summary>
        Desc
    }

    public static class OrderExt
    {
        public static string ToSqlString(this Order order)
        {
            switch (order)
            {
                case Order.Asc: return "ASC";
                case Order.Desc: return "DESC";
                default: return "";
            }
        }
    }

    public class OrderByItem : QueryParts
    {
        public string Colmun { get; }
        public Order Order { get; }

        public OrderByItem(string column, Order order)
        {
            this.Colmun = column;
            this.Order = order;
        }

        public string ToSqlString() => $"{Colmun} {Order.ToSqlString()}";
    }
}
