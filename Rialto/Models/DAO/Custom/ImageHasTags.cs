using Rialto.Models.DAO.Table;
using Rialto.Util;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.DAO.Custom
{
    public class ImageHasTags
    {
        public static IEnumerable<M_TAG_INFO> FindByImgId(long id)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                return con.Query<M_TAG_INFO, T_ADD_TAG, M_TAG_INFO>(
@"SELECT * FROM M_TAG_INFO MTAG, T_ADD_TAG ADDT 
 WHERE ADDT.IMGINF_ID = @IMGINF_ID 
  AND MTAG.TAGINF_ID = ADDT.TAGINF_ID", (tag, add) => { return tag; }
                , splitOn: "TAGINF_ID"
                , param: new { IMGINF_ID = id});
            }
        }
    }
}
