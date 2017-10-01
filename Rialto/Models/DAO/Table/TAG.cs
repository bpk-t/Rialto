using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class TAG : TableDefinition
    {
        private TAG() : base(nameof(TAG))
        {
        }

        public readonly static TAG ThisTable = new TAG();

        public static readonly ColumnDefinition ID = new ColumnDefinition(ThisTable, nameof(ID));
        public static readonly ColumnDefinition NAME = new ColumnDefinition(ThisTable, nameof(NAME));
        public static readonly ColumnDefinition RUBY = new ColumnDefinition(ThisTable, nameof(RUBY));
        public static readonly ColumnDefinition SEARCH_COUNT = new ColumnDefinition(ThisTable, nameof(SEARCH_COUNT));
        public static readonly ColumnDefinition ASSIGN_IMAGE_COUNT = new ColumnDefinition(ThisTable, nameof(ASSIGN_IMAGE_COUNT));
        public static readonly ColumnDefinition DESCRIPTION = new ColumnDefinition(ThisTable, nameof(DESCRIPTION));

        public static readonly ColumnDefinition CREATED_AT = new ColumnDefinition(ThisTable, nameof(CREATED_AT));
        public static readonly ColumnDefinition UPDATED_AT = new ColumnDefinition(ThisTable, nameof(UPDATED_AT));

        public static ColumnDefinition[] Columns()
        {
            return new ColumnDefinition[]
            {
                ID,
                NAME,
                RUBY,
                SEARCH_COUNT,
                ASSIGN_IMAGE_COUNT,
                DESCRIPTION,
                CREATED_AT,
                UPDATED_AT
            };
        }
    }
}
