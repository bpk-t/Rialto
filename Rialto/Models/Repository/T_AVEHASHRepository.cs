using Rialto.Models.DAO.Entity;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Rialto.Models.Repository
{
    public class T_AVEHASHRepository
    {
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
