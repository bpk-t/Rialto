using System;

namespace Rialto.DB.Entity
{
    public class M_IMAGE_REPOSITORY
    {
        public int IMGREPO_ID { get; set; }
        public string REPO_PATH { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }
    }
}
