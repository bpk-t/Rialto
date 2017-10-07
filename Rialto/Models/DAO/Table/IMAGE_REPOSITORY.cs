using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class IMAGE_REPOSITORY : TableDefinition
    {
        private IMAGE_REPOSITORY() : base(nameof(IMAGE_REPOSITORY))
        {
        }

        public static readonly IMAGE_REPOSITORY ThisTable = new IMAGE_REPOSITORY();

        public static readonly ColumnDefinition ID = new ColumnDefinition(ThisTable, nameof(ID));
        public static readonly ColumnDefinition PATH = new ColumnDefinition(ThisTable, nameof(PATH));
        public static readonly ColumnDefinition CREATED_AT = new ColumnDefinition(ThisTable, nameof(CREATED_AT));
        public static readonly ColumnDefinition UPDATED_AT = new ColumnDefinition(ThisTable, nameof(UPDATED_AT));

        public static string[] Columns()
        {
            return new string[]
            {
                ID.ToSqlString(),
                PATH.ToSqlString(),
                CREATED_AT.ToSqlString(),
                UPDATED_AT.ToSqlString()
            };
        }
    }
}
