using Rialto.Util;
using System;
using System.Data.SQLite;
using Dapper;

namespace Rialto.Models.DAO.Table
{
    public class T_AVEHASH
    {
        public long? IMGINF_ID { get; set; }
        public string AVEHASH { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }

        public static void Insert(T_AVEHASH insertObj)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                con.Execute("INSERT INTO T_AVEHASH(IMGINF_ID,AVEHASH) VALUES(@IMGINF_ID, @AVEHASH)",
                    new { IMGINF_ID = insertObj.IMGINF_ID, AVEHASH = insertObj.AVEHASH });
            }
        }
    }
}
