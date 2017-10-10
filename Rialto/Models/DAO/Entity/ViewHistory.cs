using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Entity
{
    public class ViewHistory
    {
        public long Id { get; set; }
        public long RegisterImageId { get; set; }

        public DateTime ViewTimestamp { get; set; }
        public int ViewTimeMs { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
