using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class REGISTER_IMAGE : TableDefinition
    {
        private REGISTER_IMAGE() : base(nameof(REGISTER_IMAGE))
        {
        }

        public static readonly REGISTER_IMAGE ThisTable = new REGISTER_IMAGE();

        public static readonly ColumnDefinition ID = new ColumnDefinition(ThisTable, nameof(ID));
        public static readonly ColumnDefinition FILE_SIZE = new ColumnDefinition(ThisTable, nameof(FILE_SIZE));
        public static readonly ColumnDefinition FILE_NAME = new ColumnDefinition(ThisTable, nameof(FILE_NAME));
        public static readonly ColumnDefinition FILE_EXTENSION = new ColumnDefinition(ThisTable, nameof(FILE_EXTENSION));
        public static readonly ColumnDefinition FILE_PATH = new ColumnDefinition(ThisTable, nameof(FILE_PATH));
        public static readonly ColumnDefinition MD5_HASH = new ColumnDefinition(ThisTable, nameof(MD5_HASH));
        public static readonly ColumnDefinition AVE_HASH = new ColumnDefinition(ThisTable, nameof(AVE_HASH));
        public static readonly ColumnDefinition IMAGE_REPOSITORY_ID = new ColumnDefinition(ThisTable, nameof(IMAGE_REPOSITORY_ID));
        public static readonly ColumnDefinition HEIGHT_PIX = new ColumnDefinition(ThisTable, nameof(HEIGHT_PIX));
        public static readonly ColumnDefinition WIDTH_PIX = new ColumnDefinition(ThisTable, nameof(WIDTH_PIX));
        public static readonly ColumnDefinition DO_GET = new ColumnDefinition(ThisTable, nameof(DO_GET));
        public static readonly ColumnDefinition DELETE_TIMESTAMP = new ColumnDefinition(ThisTable, nameof(DELETE_TIMESTAMP));
        
        public static readonly ColumnDefinition CREATED_AT = new ColumnDefinition(ThisTable, nameof(CREATED_AT));
        public static readonly ColumnDefinition UPDATED_AT = new ColumnDefinition(ThisTable, nameof(UPDATED_AT));

        public static string[] Columns()
        {
            return new string[]
            {
                ID.ToSqlString(),
                FILE_SIZE.ToSqlString(),
                FILE_NAME.ToSqlString(),
                FILE_EXTENSION.ToSqlString(),
                FILE_PATH.ToSqlString(),
                MD5_HASH.ToSqlString(),
                AVE_HASH.ToSqlString(),
                IMAGE_REPOSITORY_ID.ToSqlString(),
                HEIGHT_PIX.ToSqlString(),
                WIDTH_PIX.ToSqlString(),
                DO_GET.ToSqlString(),
                DELETE_TIMESTAMP.ToSqlString(),
                CREATED_AT.ToSqlString(),
                UPDATED_AT.ToSqlString(),
            };
        }
    }
}
