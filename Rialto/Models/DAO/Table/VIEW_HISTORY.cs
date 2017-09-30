using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class VIEW_HISTORY : TableDefinition
    {
        private VIEW_HISTORY() : base(nameof(VIEW_HISTORY))
        {
        }

        public static readonly VIEW_HISTORY ThisTable = new VIEW_HISTORY();

        public static readonly ColumnDefinition ID = new ColumnDefinition(ThisTable, nameof(ID));
        public static readonly ColumnDefinition REGISTER_IMAGE_ID = new ColumnDefinition(ThisTable, nameof(REGISTER_IMAGE_ID));
        public static readonly ColumnDefinition VIEW_TIMESTAMP = new ColumnDefinition(ThisTable, nameof(VIEW_TIMESTAMP));
        public static readonly ColumnDefinition VIEW_TIME_MS = new ColumnDefinition(ThisTable, nameof(VIEW_TIME_MS));

        public static readonly ColumnDefinition CREATED_AT = new ColumnDefinition(ThisTable, nameof(CREATED_AT));
        public static readonly ColumnDefinition UPDATED_AT = new ColumnDefinition(ThisTable, nameof(UPDATED_AT));
    }
}
