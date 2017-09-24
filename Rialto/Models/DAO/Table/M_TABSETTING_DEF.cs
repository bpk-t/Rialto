using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class M_TABSETTING_DEF : TableDefinition
    {
        public M_TABSETTING_DEF() : base("M_TABSETTING")
        {
        }
        public static M_TABSETTING_DEF ThisTable = new M_TABSETTING_DEF();

        public static ColumnDefinition TABSET_ID = new ColumnDefinition(ThisTable, nameof(TABSET_ID));
        public static ColumnDefinition TAGINF_ID = new ColumnDefinition(ThisTable, nameof(TAGINF_ID));
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(CREATE_LINE_DATE));
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(UPDATE_LINE_DATE));
    }
}
