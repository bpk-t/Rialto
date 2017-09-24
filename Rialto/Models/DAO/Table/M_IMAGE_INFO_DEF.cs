using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    // TODO クラス名変更
    public class M_IMAGE_INFO_DEF : TableDefinition
    {
        private M_IMAGE_INFO_DEF() : base("M_IMAGE_INFO") { }

        // TODO read only
        public static M_IMAGE_INFO_DEF ThisTable = new M_IMAGE_INFO_DEF();
        public static ColumnDefinition IMGINF_ID = new ColumnDefinition(ThisTable, "IMGINF_ID");
        public static ColumnDefinition FILE_SIZE = new ColumnDefinition(ThisTable, "FILE_SIZE");
        public static ColumnDefinition FILE_NAME = new ColumnDefinition(ThisTable, "FILE_NAME");
        public static ColumnDefinition FILE_TYPE = new ColumnDefinition(ThisTable, "FILE_TYPE");
        public static ColumnDefinition HASH_VALUE = new ColumnDefinition(ThisTable, "HASH_VALUE");
        public static ColumnDefinition IMGREPO_ID = new ColumnDefinition(ThisTable, "IMGREPO_ID");
        public static ColumnDefinition FILE_PATH = new ColumnDefinition(ThisTable, "FILE_PATH");
        public static ColumnDefinition HEIGHT_PIX = new ColumnDefinition(ThisTable, "HEIGHT_PIX");
        public static ColumnDefinition WIDTH_PIX = new ColumnDefinition(ThisTable, "WIDTH_PIX");
        public static ColumnDefinition COLOR = new ColumnDefinition(ThisTable, "COLOR");
        public static ColumnDefinition DO_GET = new ColumnDefinition(ThisTable, "DO_GET");
        public static ColumnDefinition DELETE_FLG = new ColumnDefinition(ThisTable, "DELETE_FLG");
        public static ColumnDefinition DELETE_REASON_ID = new ColumnDefinition(ThisTable, "DELETE_REASON_ID");
        public static ColumnDefinition DELETE_DATE = new ColumnDefinition(ThisTable, "DELETE_DATE");
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, "CREATE_LINE_DATE");
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, "UPDATE_LINE_DATE");
    }
}
