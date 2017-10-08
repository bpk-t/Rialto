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

        public static LangExt.Option<Tag> FindByName(string name)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                        .From(TAG.ThisTable)
                        .Where(TAG.NAME.Eq("@TAG_NAME"));

                return LangExt.Option.Create(
                    con.Query<Tag>(query.ToSqlString(), new { TAG_NAME = name }).FirstOrDefault()
                );
            }
        }

        /// <summary>
        /// 引数で指定されたタグ情報を追加する、すでに存在する場合は上書きする
        /// </summary>
        /// <param name="upsertObj"></param>
        /// <param name="existTagAction"></param>
        public static void Upsert(Tag upsertObj, Func<bool> existTagAction)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                if (upsertObj.Id != 0)
                {
                    if (existTagAction())
                    {
                        Update(upsertObj);
                    }
                }
                else
                {
                    FindByName(upsertObj.Name).Match(
                        (some) =>
                        {
                            if (existTagAction())
                            {
                                some.Description = upsertObj.Description;
                                Update(some);
                            }
                        },
                        () =>
                        {
                            Insert(upsertObj);
                        });
                }
            }
        }

        public static void Update(Tag updateObj)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Update(TAG.ThisTable)
                    .Set(TAG.NAME, updateObj.Name)
                    .Set(TAG.DESCRIPTION, updateObj.Description)
                    .Set(TAG.UPDATED_AT, updateObj.UpdatedAt)
                    .Where(TAG.ID.Eq("@TAG_ID"));

                var queryParam = new
                {
                    NAME = updateObj.Name,
                    DESCRIPTION = updateObj.Description,
                    UPDATED_AT = "datetime('now', 'localtime')",
                    TAG_ID = updateObj.Id
                };
                con.Execute(query.ToSqlString(), queryParam);
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

        public static Dictionary<TagGroup, List<Tag>> GetAllTagGroup()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
                var query = QueryBuilder.Select(TAG_GROUP.Columns())
                    .Select(TAG.Columns())
                    .From(TAG_GROUP.ThisTable)
                    .InnerJoin(TAG_GROUP_ASSIGN.ThisTable, TAG_GROUP_ASSIGN.TAG_GROUP_ID.Eq(TAG_GROUP.ID))
                    .InnerJoin(TAG.ThisTable, TAG_GROUP_ASSIGN.TAG_ID.Eq(TAG.ID))
                    .OrderBy(TAG_GROUP.ID, Order.Asc)
                    .OrderBy(TAG.ID, Order.Asc);

                var result = con.QueryMultiple(query.ToSqlString());

                return result.Read((TagGroup tagGroup, Tag tag) => (tagGroup, tag))
                    .GroupBy(x => x.Item1, x => x.Item2, new CompareSelector<TagGroup, int>(x => x.Id))
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
        }

        public static void InsertTagAssign(TagAssign insertObj)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        // 本当は on duplicate keyみたいなことをやりたかった
                        var select = QueryBuilder.Select("1").From(TAG_ASSIGN.ThisTable)
                            .Where(TAG_ASSIGN.REGISTER_IMAGE_ID.Eq(insertObj.RegisterImageId.ToString()))
                            .Where(TAG_ASSIGN.TAG_ID.Eq(insertObj.TagId.ToString()));

                        if (con.Query(select.ToSqlString()).IsEmpty())
                        {
                            var query = QueryBuilder.Insert().Into(TAG_ASSIGN.ThisTable)
                                .Set(TAG_ASSIGN.REGISTER_IMAGE_ID, insertObj.RegisterImageId)
                                .Set(TAG_ASSIGN.TAG_ID, insertObj.TagId);

                            var queryParam = new
                            {
                                REGISTER_IMAGE_ID = insertObj.RegisterImageId,
                                TAG_ID = insertObj.TagId,
                            };

                            con.Execute(query.ToSqlString(), queryParam);
                            tran.Commit();
                        }

                    } catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        public static IEnumerable<Tag> GetTagByImageAssigned(long imgId)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

                var query = QueryBuilder.Select(TAG.Columns())
                    .From(TAG.ThisTable)
                    .InnerJoin(TAG_ASSIGN.ThisTable, TAG.ID.Eq(TAG_ASSIGN.TAG_ID))
                    .Where(TAG_ASSIGN.REGISTER_IMAGE_ID.Eq(imgId.ToString()))
                    .OrderBy(TAG_ASSIGN.CREATED_AT, Order.Asc);

                return con.Query<Tag>(query.ToSqlString());
            }
        }
    }
}
