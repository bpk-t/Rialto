using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class TAG_GROUP_ASSIGN : TableDefinition
    {
        public TAG_GROUP_ASSIGN() : base(nameof(TAG_GROUP_ASSIGN))
        {
        }

        public static readonly TAG_GROUP_ASSIGN ThisTable = new TAG_GROUP_ASSIGN();

        public static readonly ColumnDefinition TAG_GROUP_ID = new ColumnDefinition(ThisTable, nameof(TAG_GROUP_ID));
        public static readonly ColumnDefinition TAG_ID = new ColumnDefinition(ThisTable, nameof(TAG_ID));

        public static readonly ColumnDefinition CREATED_AT = new ColumnDefinition(ThisTable, nameof(CREATED_AT));
        public static readonly ColumnDefinition UPDATED_AT = new ColumnDefinition(ThisTable, nameof(UPDATED_AT));
    }
}
