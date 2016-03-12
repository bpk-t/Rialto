using System;

namespace Rialto.DB.Entity
{
    public class M_TABSETTING
    {
        public int TABSET_ID { get; set; }
        public long TAGINF_ID { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
