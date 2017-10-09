using Rialto.Models.DAO.Builder;
using Rialto.Models.DAO.Table;
using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Rialto.Models.DAO.Entity;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Data.Common;

namespace Rialto.Models.Repository
{
    public class RegisterImageRepository
    {
        public static long GetAllCount()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull());

                return con.ExecuteScalar<long>(query.ToSqlString());
            }
        }

        public static Task<long> GetAllCountAsync(DbConnection connection)
        {
            var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                .From(REGISTER_IMAGE.ThisTable)
                .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull());

            return connection.ExecuteScalarAsync<long>(query.ToSqlString());
        }

        public static IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)> GetAll(long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                // TODO 
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

                var query = QueryBuilder.Select(REGISTER_IMAGE.Columns())
                    .Select(IMAGE_REPOSITORY.Columns())
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .OrderBy(REGISTER_IMAGE.ID, order)
                    .Limit(limit)
                    .Offset(offset);

                var reader = con.QueryMultiple(query.ToSqlString());
                return reader.Read((RegisterImage img, ImageRepository repo) => (Optional(img), Optional(repo)));
            }
        }

        public static long GetNoTagCount()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(ConditionBuilder.NotExists(
                        QueryBuilder.Select().From(TAG_ASSIGN.ThisTable).Where(REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))));

                return con.ExecuteScalar<long>(query.ToSqlString());
            }
        }

        public static Task<long> GetNoTagCountAsync(DbConnection connection)
        {
            var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                .From(REGISTER_IMAGE.ThisTable)
                .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                .Where(ConditionBuilder.NotExists(
                    QueryBuilder.Select().From(TAG_ASSIGN.ThisTable).Where(REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))));

            return connection.ExecuteScalarAsync<long>(query.ToSqlString());
        }

        public static IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)> GetNoTag(long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                // TODO 
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

                var query = QueryBuilder.Select(REGISTER_IMAGE.Columns())
                    .Select(IMAGE_REPOSITORY.Columns())
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(ConditionBuilder.NotExists(
                        QueryBuilder.Select().From(TAG_ASSIGN.ThisTable).Where(REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                        ))
                    .OrderBy(REGISTER_IMAGE.ID, order)
                    .Limit(limit)
                    .Offset(offset);

                var reader = con.QueryMultiple(query.ToSqlString());
                return reader.Read((RegisterImage img, ImageRepository repo) => (Optional(img), Optional(repo)));
            }
        }

        public static long GetByTagCount(long tagId)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                    .InnerJoin(TAG_ASSIGN.ThisTable, REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                    .InnerJoin(TAG.ThisTable, TAG_ASSIGN.TAG_ID.Eq(TAG.ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(TAG.ID.Eq("@TAG_ID"));

                return con.ExecuteScalar<long>(query.ToSqlString(), param: new { TAG_ID = tagId });
            }
        }

        public static IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)> GetByTag(long tagId, long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                // TODO 
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

                var query = QueryBuilder.Select(REGISTER_IMAGE.Columns())
                    .Select(IMAGE_REPOSITORY.Columns())
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                    .InnerJoin(TAG_ASSIGN.ThisTable, REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                    .InnerJoin(TAG.ThisTable, TAG_ASSIGN.TAG_ID.Eq(TAG.ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(TAG.ID.Eq("@TAG_ID"))
                    .OrderBy(REGISTER_IMAGE.ID, order)
                    .Limit(limit)
                    .Offset(offset);

                var reader = con.QueryMultiple(query.ToSqlString(), param: new { TAG_ID = tagId });
                return reader.Read((RegisterImage img, ImageRepository repo) => (Optional(img), Optional(repo)));
            }
        }

        public static Option<RegisterImage> FindByHash(string hash)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(REGISTER_IMAGE.ThisTable)
                    .Where(ConditionBuilder.Eq(SQLFunctionBuilder.Trim(REGISTER_IMAGE.MD5_HASH).ToSqlString(), "@HASH_VALUE"));

                return Some(con.Query(query.ToSqlString(), new { HASH_VALUE = hash }).FirstOrDefault());
            }
        }

        public static Option<RegisterImage> FindById(long id)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(REGISTER_IMAGE.ThisTable)
                    .Where(REGISTER_IMAGE.ID.Eq("@ID"));

                return Some(con.Query(query.ToSqlString(), new { ID = id }).FirstOrDefault());
            }
        }

        public static Option<RegisterImage> Insert(RegisterImage info)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                using (var tran = con.BeginTransaction())
                {
                    var query = QueryBuilder.Insert().Into(REGISTER_IMAGE.ThisTable)
                        .Set(REGISTER_IMAGE.FILE_SIZE, info.FileSize)
                        .Set(REGISTER_IMAGE.FILE_NAME, info.FileName)
                        .Set(REGISTER_IMAGE.FILE_EXTENSION, info.FileExtension)
                        .Set(REGISTER_IMAGE.FILE_PATH, info.FilePath)
                        .Set(REGISTER_IMAGE.MD5_HASH, info.Md5Hash)
                        .Set(REGISTER_IMAGE.AVE_HASH, info.AveHash)
                        .Set(REGISTER_IMAGE.HEIGHT_PIX, info.HeightPix)
                        .Set(REGISTER_IMAGE.WIDTH_PIX, info.WidthPix)
                        .Set(REGISTER_IMAGE.DO_GET, info.DoGet)
                        .Set(REGISTER_IMAGE.DELETE_TIMESTAMP, info.DeleteTimestamp);

                    var queryParams = new
                    {
                        FILE_SIZE = info.FileSize,
                        FILE_NAME = info.FileName,
                        FILE_EXTENSION = info.FileExtension,
                        FILE_PATH = info.FilePath,
                        MD5_HASH = info.Md5Hash,
                        AVE_HASH = info.AveHash,
                        HEIGHT_PIX = info.HeightPix,
                        WIDTH_PIX = info.WidthPix,
                        DO_GET = info.DoGet,
                        DELETE_TIMESTAMP = info.DeleteTimestamp
                    };

                    con.Execute(query.ToSqlString(), queryParams);

                    var selectQuery = QueryBuilder.Select()
                        .From(REGISTER_IMAGE.ThisTable)
                        .Where(ConditionBuilder.Eq("ROWID", SQLFunctionBuilder.LastInsertLowId().ToSqlString()));
                    var inserted = con.Query<RegisterImage>(selectQuery.ToSqlString()).FirstOrDefault();

                    tran.Commit();

                    return inserted;
                }

            }
        }
    }
}
