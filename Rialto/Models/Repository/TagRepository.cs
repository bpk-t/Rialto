using Rialto.Models.DAO.Builder;
using Rialto.Models.DAO.Entity;
using Rialto.Models.DAO.Table;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LangExt;

namespace Rialto.Models.Repository
{
    public class TagRepository
    {
        public static IEnumerable<Tag> GetAllTag()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(TAG.ThisTable)
                    .OrderBy(TAG.ID, Order.Asc);
                return con.Query<Tag>(query.ToSqlString());
            }
        }

        public static Option<Tag> FindByName(string name)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                        .From(TAG.ThisTable)
                        .Where(TAG.NAME.Eq("@TAG_NAME"));

                return Option.Create(
                    con.Query<Tag>(query.ToSqlString(), new { TAG_NAME = name }).FirstOrDefault()
                );
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

        public static void Insert(Tag insertObj)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Insert().Into(TAG.ThisTable)
                    .Set(TAG.NAME, insertObj.Name)
                    .Set(TAG.RUBY, insertObj.Ruby)
                    .Set(TAG.SEARCH_COUNT, insertObj.SearchCount)
                    .Set(TAG.ASSIGN_IMAGE_COUNT, insertObj.AssignImageCount)
                    .Set(TAG.DESCRIPTION, insertObj.Description);

                var queryParam = new
                {
                    NAME = insertObj.Name,
                    RUBY = insertObj.Ruby,
                    SEARCH_COUNT = insertObj.SearchCount,
                    ASSIGN_IMAGE_COUNT = insertObj.AssignImageCount,
                    DESCRIPTION = insertObj.Description
                };

                con.Execute(query.ToSqlString(), queryParam);
            }
        }
    }
}
