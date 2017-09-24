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
        public static ColumnDefinition IMGINF_ID = new ColumnDefinition(ThisTable, nameof(IMGINF_ID));
        public static ColumnDefinition FILE_SIZE = new ColumnDefinition(ThisTable, nameof(FILE_SIZE));
        public static ColumnDefinition FILE_NAME = new ColumnDefinition(ThisTable, nameof(FILE_NAME));
        public static ColumnDefinition FILE_TYPE = new ColumnDefinition(ThisTable, nameof(FILE_TYPE));
        public static ColumnDefinition HASH_VALUE = new ColumnDefinition(ThisTable, nameof(HASH_VALUE));
        public static ColumnDefinition IMGREPO_ID = new ColumnDefinition(ThisTable, nameof(IMGREPO_ID));
        public static ColumnDefinition FILE_PATH = new ColumnDefinition(ThisTable, nameof(FILE_PATH));
        public static ColumnDefinition HEIGHT_PIX = new ColumnDefinition(ThisTable, nameof(HEIGHT_PIX));
        public static ColumnDefinition WIDTH_PIX = new ColumnDefinition(ThisTable, nameof(WIDTH_PIX));
        public static ColumnDefinition COLOR = new ColumnDefinition(ThisTable, nameof(COLOR));
        public static ColumnDefinition DO_GET = new ColumnDefinition(ThisTable, nameof(DO_GET));
        public static ColumnDefinition DELETE_FLG = new ColumnDefinition(ThisTable, nameof(DELETE_FLG));
        public static ColumnDefinition DELETE_REASON_ID = new ColumnDefinition(ThisTable, nameof(DELETE_REASON_ID));
        public static ColumnDefinition DELETE_DATE = new ColumnDefinition(ThisTable, nameof(DELETE_DATE));
        public static ColumnDefinition CREATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(CREATE_LINE_DATE));
        public static ColumnDefinition UPDATE_LINE_DATE = new ColumnDefinition(ThisTable, nameof(UPDATE_LINE_DATE));
    }
}
