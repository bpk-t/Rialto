using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Entity
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ruby { get; set; }
        public int SearchCount { get; set; }
        public int AssignImageCount { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
