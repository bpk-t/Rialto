using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Rialto.Models.DAO.Entity;
using Rialto.Util;

namespace Rialto.Models.Repository
{
    public class T_ADD_TAGRepository
    {
        public static void Insert(T_ADD_TAG insertObj)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                con.Execute("INSERT INTO T_ADD_TAG(IMGINF_ID,TAGINF_ID) VALUES(@IMGINF_ID, @TAGINF_ID)",
                    new { IMGINF_ID = insertObj.IMGINF_ID, TAGINF_ID = insertObj.TAGINF_ID });
            }
        }
    }
}
