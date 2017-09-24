using Rialto.Models.DAO.Entity;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LangExt;
using Rialto.Models.DAO.Builder;
using Rialto.Models.DAO.Table;

namespace Rialto.Models.Repository
{
    public class M_TAG_INFORepository
    {
        public static IEnumerable<M_TAG_INFO> GetAll()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(M_TAG_INFO_DEF.ThisTable)
                    .OrderBy(M_TAG_INFO_DEF.TAGINF_ID, Order.Asc);
                return con.Query<M_TAG_INFO>(query.ToSqlString());
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

        /// <summary>
        /// 引数で指定されたタグ情報を追加する、すでに存在する場合は上書きする
        /// </summary>
        /// <param name="upsertObj"></param>
        /// <param name="existTagAction"></param>
        public static void Upsert(M_TAG_INFO upsertObj, Func<bool> existTagAction)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                if (upsertObj.TAGINF_ID.HasValue)
                {
                    if (existTagAction())
                    {
                        Update(upsertObj);
                    }
                }
                else
                {
                    FindByName(upsertObj.TAG_NAME).Match(
                        (some) => {
                            if (existTagAction())
                            {
                                some.TAG_DEFINE = upsertObj.TAG_DEFINE;
                                Update(some);
                            }
                        },
                        () => {
                            Insert(upsertObj);
                        });
                }
            }
        }

        public static void Update(M_TAG_INFO updateObj)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                con.Execute(
@"UPDATE M_TAG_INFO 
 SET TAG_NAME = @TAG_NAME
 ,TAG_DEFINE = @TAG_DEFINE
 ,UPDATE_LINE_DATE = datetime('now', 'localtime') 
 WHERE TAGINF_ID = @TAGINF_ID", new
{
    TAG_NAME = updateObj.TAG_NAME,
    TAG_DEFINE = updateObj.TAG_DEFINE,
    TAGINF_ID = updateObj.TAGINF_ID
});
            }
        }

        public static void Insert(M_TAG_INFO insertObj)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                con.Execute(
@"INSERT INTO M_TAG_INFO (TAG_NAME,TAG_RUBY,SEARCH_COUNT,TAG_DEFINE)
 VALUES(@TAG_NAME,@TAG_RUBY,@SEARCH_COUNT,@TAG_DEFINE)", new
{
    TAG_NAME = insertObj.TAG_NAME,
    TAG_RUBY = " ",
    SEARCH_COUNT = 0,
    TAG_DEFINE = insertObj.TAG_DEFINE
});
            }
        }

        /// <summary>
        /// 指定された名前のタグを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Option<M_TAG_INFO> FindByName(string name)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                return Option.Create(
                    con.Query<M_TAG_INFO>("SELECT * FROM M_TAG_INFO WHERE TAG_NAME=@TAG_NAME"
                    , new { TAG_NAME = name }).FirstOrDefault()
                );
            }
        }

        public static void DeleteById(long id)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                con.Execute(@"DELETE FROM M_TAG_INFO WHERE TAGINF_ID=@TAGINF_ID", new { TAGINF_ID = id });
            }
        }
    }
}
