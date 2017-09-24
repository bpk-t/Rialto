using Rialto.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using LangExt;
using Rialto.Models.DAO.Builder;
using Rialto.Models.DAO.Table;

namespace Rialto.Models.DAO.Entity
{
    public class M_IMAGE_INFO
    {
        public long? IMGINF_ID { get; set; }
        public int FILE_SIZE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_TYPE { get; set; }
        public string HASH_VALUE { get; set; }
        public int IMGREPO_ID { get; set; }
        public string FILE_PATH { get; set; }
        public int HEIGHT_PIX { get; set; }
        public int WIDTH_PIX { get; set; }
        public int COLOR { get; set; }
        public int DO_GET { get; set; }
        public int DELETE_FLG { get; set; }
        public int DELETE_REASON_ID { get; set; }
        public DateTime DELETE_DATE { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }

        public string AVEHASH { get; set; }
    }
}
