using System;

namespace Rialto.DB.Entity
{
    public class M_TAGADDTAB
    {
        public int TABSET_ID { get; set; }
        public string TAB_NAME { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
