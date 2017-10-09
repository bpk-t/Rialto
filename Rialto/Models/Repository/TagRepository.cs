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
using LanguageExt;
using static LanguageExt.Prelude;
using System.Data.Common;

namespace Rialto.Models.Repository
{
    public class TagRepository
    {
        public static Task<IEnumerable<Tag>> GetAllTagAsync(DbConnection connection)
        {
            var query = QueryBuilder.Select()
                .From(TAG.ThisTable)
                .OrderBy(TAG.ID, Order.Asc);
            return connection.QueryAsync<Tag>(query.ToSqlString());
        }

        public static Task<Option<Tag>> FindByNameAsync(DbConnection connection, string name)
        {
            var query = QueryBuilder.Select()
                    .From(TAG.ThisTable)
                    .Where(TAG.NAME.Eq("@TAG_NAME"));

            return connection.QueryAsync<Tag>(query.ToSqlString(), new { TAG_NAME = name })
                .ContinueWith(x => x.Result.ToOption());
        }

        public static Task<Option<Tag>> FindByIdAsync(DbConnection connection, int tagId)
        {
            var query = QueryBuilder.Select()
                    .From(TAG.ThisTable)
                    .Where(TAG.ID.Eq("@TAG_ID"));

            return connection.QueryAsync<Tag>(query.ToSqlString(), new { TAG_ID = tagId })
                    .ContinueWith(x => x.Result.ToOption());
        }

        /// <summary>
        /// 引数で指定されたタグ情報を追加する、すでに存在する場合は上書きする
        /// </summary>
        /// <param name="upsertObj"></param>
        public static Task<int> UpsertAsync(DbConnection connection, Tag upsertObj)
        {
            return FindByIdAsync(connection, upsertObj.Id).SelectMany(x =>
            {
                if (x.IsSome)
                {
                    return UpdateAsync(connection, upsertObj);
                }
                else
                {
                    return InsertAsync(connection, upsertObj);
                }
            });
        }

        public static Task<int> UpdateAsync(DbConnection connection, Tag updateObj)
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
            return connection.ExecuteAsync(query.ToSqlString(), queryParam);
        }

        public static Task<int> InsertAsync(DbConnection connection, Tag insertObj)
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

            return connection.ExecuteAsync(query.ToSqlString(), queryParam);
        }

        public static Task<Dictionary<TagGroup, List<Tag>>> GetAllTagGroupAsync(DbConnection connection)
        {
            var query = QueryBuilder.Select(TAG_GROUP.Columns())
                .Select(TAG.Columns())
                .From(TAG_GROUP.ThisTable)
                .InnerJoin(TAG_GROUP_ASSIGN.ThisTable, TAG_GROUP_ASSIGN.TAG_GROUP_ID.Eq(TAG_GROUP.ID))
                .InnerJoin(TAG.ThisTable, TAG_GROUP_ASSIGN.TAG_ID.Eq(TAG.ID))
                .OrderBy(TAG_GROUP.ID, Order.Asc)
                .OrderBy(TAG.ID, Order.Asc);

            return connection.QueryMultipleAsync(query.ToSqlString()).Select(reader =>
            {
                return reader.Read((TagGroup tagGroup, Tag tag) => (tagGroup, tag))
                    .GroupBy(x => x.Item1, x => x.Item2, new CompareSelector<TagGroup, int>(x => x.Id))
                    .ToDictionary(x => x.Key, x => x.ToList());
            });
        }

        public static Task<int> InsertTagAssignAsync(DbConnection connection, TagAssign insertObj)
        {
            // 本当は on duplicate keyみたいなことをやりたかったけどSQLiteにはない
            var select = QueryBuilder.Select("1").From(TAG_ASSIGN.ThisTable)
                .Where(TAG_ASSIGN.REGISTER_IMAGE_ID.Eq(insertObj.RegisterImageId.ToString()))
                .Where(TAG_ASSIGN.TAG_ID.Eq(insertObj.TagId.ToString()));

            return connection.QueryAsync(select.ToSqlString()).SelectMany(x =>
            {
                if (x.IsEmpty())
                {
                    var query = QueryBuilder.Insert().Into(TAG_ASSIGN.ThisTable)
                        .Set(TAG_ASSIGN.REGISTER_IMAGE_ID, insertObj.RegisterImageId)
                        .Set(TAG_ASSIGN.TAG_ID, insertObj.TagId);

                    var queryParam = new
                    {
                        REGISTER_IMAGE_ID = insertObj.RegisterImageId,
                        TAG_ID = insertObj.TagId,
                    };

                    return connection.ExecuteAsync(query.ToSqlString(), queryParam);
                }
                return Task.FromResult(0);
            });
        }

        public static Task<IEnumerable<Tag>> GetTagByImageAssignedAsync(DbConnection connection, long imgId)
        {
            var query = QueryBuilder.Select(TAG.Columns())
                .From(TAG.ThisTable)
                .InnerJoin(TAG_ASSIGN.ThisTable, TAG.ID.Eq(TAG_ASSIGN.TAG_ID))
                .Where(TAG_ASSIGN.REGISTER_IMAGE_ID.Eq(imgId.ToString()))
                .OrderBy(TAG_ASSIGN.CREATED_AT, Order.Asc);

            return connection.QueryAsync<Tag>(query.ToSqlString());
        }
    }
}
