using Rialto.Util;
using System;
using System.Collections.Generic;
using Dapper;

namespace Rialto.Models.DAO
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

        public string AVEHASH { get; set; }

        public static IEnumerable<M_IMAGE_INFO> GetAll()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                return con.Query(
@"SELECT * FROM M_IMAGE_INFO IMGINF, T_AVEHASH AVEH
 WHERE IMGINF.DELETE_FLG='0'
 AND IMGINF.IMGINF_ID=AVEH.IMGINF_ID 
 ORDER BY IMGINF.IMGINF_ID DESC",
                         (M_IMAGE_INFO img, T_AVEHASH hash) => {
                             img.AVEHASH = hash.AVEHASH;
                             return img; 
                         }, splitOn: "IMGINF_ID,IMGINF_ID");
            }
        }
    }
}
