using Rialto.Models.DAO.Builder;
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

        public static ColumnDefinition IMGINF_ID = new ColumnDefinition(ThisTable, nameof(IMGINF_ID));
        public static ColumnDefinition AVEHASH = new ColumnDefinition(ThisTable, nameof(AVEHASH));
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(CREATE_LINE_DATE));
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(UPDATE_LINE_DATE));
    }
}
