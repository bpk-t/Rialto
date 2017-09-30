using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Table
{
    public class IMAGE_SCORE : TableDefinition
    {
        private IMAGE_SCORE() : base(nameof(IMAGE_SCORE))
        {
        }

        public static readonly IMAGE_SCORE ThisTable = new IMAGE_SCORE();

        public static readonly ColumnDefinition REGISTER_IMAGE_ID = new ColumnDefinition(ThisTable, nameof(REGISTER_IMAGE_ID));
        public static readonly ColumnDefinition TAG_ID = new ColumnDefinition(ThisTable, nameof(TAG_ID));
        public static readonly ColumnDefinition SCORE = new ColumnDefinition(ThisTable, nameof(SCORE));

        public static readonly ColumnDefinition CREATED_AT = new ColumnDefinition(ThisTable, nameof(CREATED_AT));
        public static readonly ColumnDefinition UPDATED_AT = new ColumnDefinition(ThisTable, nameof(UPDATED_AT));
    }
}
