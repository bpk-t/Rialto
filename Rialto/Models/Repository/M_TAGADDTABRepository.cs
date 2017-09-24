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
    public class M_TAGADDTABRepository
    {
        public static IEnumerable<M_TAGADDTAB> GetAll()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                return con.Query<M_TAGADDTAB>("SELECT * FROM M_TAGADDTAB ORDER BY TABSET_ID");
            }
        }

        public static IEnumerable<dynamic> GetTabSettings(M_TAGADDTAB tab)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                return con.Query(
@"SELECT * FROM M_TABSETTING TABSET, M_TAG_INFO TAGM
 WHERE TABSET.TAGINF_ID=TAGM.TAGINF_ID
 AND TABSET.TABSET_ID=@TABSET_ID
 ORDER BY TABSET.UPDATE_LINE_DATE"
                    , (M_TABSETTING t, M_TAG_INFO tag) => new { TagName = tag.TAG_NAME, TagInfID = tag.TAGINF_ID }
                    , new { TABSET_ID = tab.TABSET_ID }, splitOn: "TABSET_ID,TAGINF_ID");

            }
        }
    }
}
