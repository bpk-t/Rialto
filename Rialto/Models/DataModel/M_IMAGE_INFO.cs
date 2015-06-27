using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Rialto.Models.DataModel
{
    public class M_IMAGE_INFO
    {
        public long? IMGINF_ID { get; set; }
        public int FILE_SIZE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_TYPE { get; set; }
        public string HASH_VALUE { get; set; }
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

        public static IEnumerable<M_IMAGE_INFO> GetAll()
        {
            var db = DBHelper.GetInstance();
            using (var con = db.GetDbConnection())
            {
                return con.Query<M_IMAGE_INFO>("SELECT * FROM M_IMAGE_INFO WHERE DELETE_FLG='0' ORDER BY IMGINF_ID DESC");
            }
        }
    }
}
