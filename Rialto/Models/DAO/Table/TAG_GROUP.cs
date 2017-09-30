using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class TAG_GROUP : TableDefinition
    {
        public TAG_GROUP() : base(nameof(TAG_GROUP))
        {
        }

        public static readonly TAG_GROUP ThisTable = new TAG_GROUP();

        public static readonly ColumnDefinition ID = new ColumnDefinition(ThisTable, nameof(ID));
        public static readonly ColumnDefinition NAME = new ColumnDefinition(ThisTable, nameof(NAME));
        public static readonly ColumnDefinition CREATED_AT = new ColumnDefinition(ThisTable, nameof(CREATED_AT));
        public static readonly ColumnDefinition UPDATED_AT = new ColumnDefinition(ThisTable, nameof(UPDATED_AT));
    }
}
