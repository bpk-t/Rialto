using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class M_TAG_INFO_DEF : TableDefinition
    {
        private M_TAG_INFO_DEF() : base("M_TAG_INFO")
        {
        }
        public static M_TAG_INFO_DEF ThisTable = new M_TAG_INFO_DEF();

        public static ColumnDefinition TAGINF_ID = new ColumnDefinition(ThisTable, nameof(TAGINF_ID));
        public static ColumnDefinition TAG_NAME = new ColumnDefinition(ThisTable, nameof(TAG_NAME));
        public static ColumnDefinition TAG_RUBY = new ColumnDefinition(ThisTable, nameof(TAG_RUBY));
        public static ColumnDefinition SEARCH_COUNT = new ColumnDefinition(ThisTable, nameof(SEARCH_COUNT));
        public static ColumnDefinition IMG_COUNT = new ColumnDefinition(ThisTable, nameof(IMG_COUNT));
        public static ColumnDefinition TAG_DEFINE = new ColumnDefinition(ThisTable, nameof(TAG_DEFINE));
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(CREATE_LINE_DATE));
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(UPDATE_LINE_DATE));
    }
}
