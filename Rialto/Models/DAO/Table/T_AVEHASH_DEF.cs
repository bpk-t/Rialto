using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    // TODO クラス名変更
    public class T_AVEHASH_DEF : TableDefinition
    {
        public static T_AVEHASH_DEF ThisTable = new T_AVEHASH_DEF();

        public T_AVEHASH_DEF() : base("T_AVEHASH") { }

        public static ColumnDefinition IMGINF_ID = new ColumnDefinition(ThisTable, "IMGINF_ID");
        public static ColumnDefinition AVEHASH = new ColumnDefinition(ThisTable, "AVEHASH");
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, "CREATE_LINE_DATE");
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, "UPDATE_LINE_DATE");
    }
}
