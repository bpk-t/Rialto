using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Rialto.Models.DataModel
{
    public class M_TAG_INFO
    {
        public long? TAGINF_ID { get; set; }
        public string TAG_NAME { get; set; }
        public string TAG_RUBY { get; set; }
        public int SEARCH_COUNT { get; set; }
        public int IMG_COUNT { get; set; }
        public string TAG_DEFINE { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }

        public static IEnumerable<M_TAG_INFO> GetAll()
        {
            DBHelper db = DBHelper.GetInstance();
            using (var con = db.GetDbConnection())
            {
                return con.Query<M_TAG_INFO>("SELECT * FROM M_TAG_INFO");
            }
        }

        /// <summary>
        /// 全画像の枚数を返す
        /// </summary>
        /// <returns></returns>
        public static int GetAllImgCount()
        {
            DBHelper db = DBHelper.GetInstance();
            return db.GetItemCount("SELECT COUNT(*) AS ITEM_COUNT FROM M_IMAGE_INFO WHERE DELETE_FLG='0'");
        }

        /// <summary>
        /// タグ付けされていない画像の枚数を返す
        /// </summary>
        /// <returns></returns>
        public static int GetHasNotTagImgCount()
        {
            DBHelper db = DBHelper.GetInstance();
            return db.GetItemCount(
@"SELECT COUNT(*) AS ITEM_COUNT FROM M_IMAGE_INFO
 WHERE DELETE_FLG='0' AND IMGINF_ID NOT IN (SELECT IMGINF_ID FROM T_ADD_TAG)");
        }
    }
}
