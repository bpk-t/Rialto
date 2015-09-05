using Rialto.Util;
using System;
using System.Collections.Generic;
using Dapper;

namespace Rialto.Models.DAO
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
            using (var con = DBHelper.Instance.GetDbConnection())
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
            return DBHelper.Instance.GetItemCount("SELECT COUNT(*) AS ITEM_COUNT FROM M_IMAGE_INFO WHERE DELETE_FLG='0'");
        }

        /// <summary>
        /// タグ付けされていない画像の枚数を返す
        /// </summary>
        /// <returns></returns>
        public static int GetHasNotTagImgCount()
        {
            return DBHelper.Instance.GetItemCount(
@"SELECT COUNT(*) AS ITEM_COUNT FROM M_IMAGE_INFO
 WHERE DELETE_FLG='0' AND IMGINF_ID NOT IN (SELECT IMGINF_ID FROM T_ADD_TAG)");
        }

        public static void UpsertTag()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                
            }
        }
    }
}
