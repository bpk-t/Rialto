using System;

namespace Rialto.DB.Entity
{
    public class M_TAG_INFO
    {
        public int TAGINF_ID { get; set; }
        public string TAG_NAME { get; set; }
        public string TAG_RUBY { get; set; }
        public int SEARCH_COUNT { get; set; }
        public int IMG_COUNT { get; set; }
        public string TAG_DEFINE { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
