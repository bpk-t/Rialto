using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class T_ADD_TAG_DEF : TableDefinition
    {
        private T_ADD_TAG_DEF() : base("T_ADD_TAG")
        {
        }

        public static T_ADD_TAG_DEF ThisTable = new T_ADD_TAG_DEF();

        public static ColumnDefinition IMGINF_ID = new ColumnDefinition(ThisTable, "IMGINF_ID");
        public static ColumnDefinition TAGINF_ID = new ColumnDefinition(ThisTable, "TAGINF_ID");
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, "CREATE_LINE_DATE");
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, "UPDATE_LINE_DATE");
    }
}
