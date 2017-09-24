using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class M_TAGADDTAB_DEF : TableDefinition
    {
        public M_TAGADDTAB_DEF() : base("M_TAGADDTAB"){}
        public static M_TAGADDTAB_DEF ThisTable = new M_TAGADDTAB_DEF();

        public static ColumnDefinition TABSET_ID = new ColumnDefinition(ThisTable, nameof(TABSET_ID));
        public static ColumnDefinition TAB_NAME = new ColumnDefinition(ThisTable, nameof(TAB_NAME));
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(CREATE_LINE_DATE));
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(UPDATE_LINE_DATE));
    }
}
