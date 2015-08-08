using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DBModel
{
    public class M_TABSETTING
    {
        public long? TABSET_ID { get; set; }
        public long TAGINF_ID { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
