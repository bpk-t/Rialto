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
        public static Task<long> GetAllCountAsync(DbConnection connection)
        {
            var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                .From(REGISTER_IMAGE.ThisTable)
                .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull());

            return connection.ExecuteScalarAsync<long>(query.ToSqlString());
        }

        public static Task<IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)>> GetAllAsync(DbConnection connection, Option<long> offsetOpt, Option<long> limitOpt, Order order)
        {
            var query = QueryBuilder.Select(REGISTER_IMAGE.Columns())
                .Select(IMAGE_REPOSITORY.Columns())
                .From(REGISTER_IMAGE.ThisTable)
                .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                .OrderBy(REGISTER_IMAGE.ID, order);
            query = limitOpt.Fold(query, (nouse, limit) => query.Limit(limit));
            query = offsetOpt.Fold(query, (nouse, offset) => query.Offset(offset));

            return connection.QueryMultipleAsync(query.ToSqlString()).Select(reader =>
                        reader.Read((RegisterImage img, ImageRepository repo) => (Optional(img), Optional(repo)))
                    );
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

        public static Task<IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)>> GetNoTagAsync(DbConnection connection, Option<long> offsetOpt, Option<long> limitOpt, Order order)
        {
            var query = QueryBuilder.Select(REGISTER_IMAGE.Columns())
                .Select(IMAGE_REPOSITORY.Columns())
                .From(REGISTER_IMAGE.ThisTable)
                .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                .Where(ConditionBuilder.NotExists(
                    QueryBuilder.Select().From(TAG_ASSIGN.ThisTable).Where(REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                    ))
                .OrderBy(REGISTER_IMAGE.ID, order);
            query = limitOpt.Fold(query, (nouse, limit) => query.Limit(limit));
            query = offsetOpt.Fold(query, (nouse, offset) => query.Offset(offset));

            return connection.QueryMultipleAsync(query.ToSqlString()).Select(reader =>
                reader.Read((RegisterImage img, ImageRepository repo) => (Optional(img), Optional(repo)))
            );
        }

        public static Task<long> GetByTagCountAsync(DbConnection connection, long tagId)
        {
            var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                .From(REGISTER_IMAGE.ThisTable)
                .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                .InnerJoin(TAG_ASSIGN.ThisTable, REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                .InnerJoin(TAG.ThisTable, TAG_ASSIGN.TAG_ID.Eq(TAG.ID))
                .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                .Where(TAG.ID.Eq("@TAG_ID"));

            return connection.ExecuteScalarAsync<long>(query.ToSqlString(), param: new { TAG_ID = tagId });
        }

        public static Task<IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)>> GetByTagAsync(DbConnection connection, long tagId, Option<long> offsetOpt, Option<long> limitOpt, Order order)
        {
            var query = QueryBuilder.Select(REGISTER_IMAGE.Columns())
                .Select(IMAGE_REPOSITORY.Columns())
                .From(REGISTER_IMAGE.ThisTable)
                .InnerJoin(IMAGE_REPOSITORY.ThisTable, IMAGE_REPOSITORY.ID.Eq(REGISTER_IMAGE.IMAGE_REPOSITORY_ID))
                .InnerJoin(TAG_ASSIGN.ThisTable, REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                .InnerJoin(TAG.ThisTable, TAG_ASSIGN.TAG_ID.Eq(TAG.ID))
                .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                .Where(TAG.ID.Eq("@TAG_ID"))
                .OrderBy(REGISTER_IMAGE.ID, order);
            query = limitOpt.Fold(query, (nouse, limit) => query.Limit(limit));
            query = offsetOpt.Fold(query, (nouse, offset) => query.Offset(offset));

            return connection.QueryMultipleAsync(query.ToSqlString(), param: new { TAG_ID = tagId }).Select(reader =>
                reader.Read((RegisterImage img, ImageRepository repo) => (Optional(img), Optional(repo)))
            );
        }

        public static Task<Option<RegisterImage>> FindByHashAsync(DbConnection connection, string hash)
        {
            var query = QueryBuilder.Select()
                .From(REGISTER_IMAGE.ThisTable)
                .Where(ConditionBuilder.Eq(SQLFunctionBuilder.Trim(REGISTER_IMAGE.MD5_HASH).ToSqlString(), "@HASH_VALUE"));

            return connection.QueryAsync<RegisterImage>(query.ToSqlString(), new { HASH_VALUE = hash })
                .Select(x => x.ToOption());
        }

        public static Task<Option<RegisterImage>> FindByIdAsync(DbConnection connection, long id)
        {
            var query = QueryBuilder.Select()
                .From(REGISTER_IMAGE.ThisTable)
                .Where(REGISTER_IMAGE.ID.Eq("@ID"));

            return connection.QueryAsync<RegisterImage>(query.ToSqlString(), new { ID = id })
                 .Select(x => x.ToOption());
        }

        public static Task<Option<RegisterImage>> InsertAsync(DbConnection connection, RegisterImage info)
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

            return connection.ExecuteAsync(query.ToSqlString(), queryParams).SelectMany(nouse =>
            {
                var selectQuery = QueryBuilder.Select()
                    .From(REGISTER_IMAGE.ThisTable)
                    .Where(ConditionBuilder.Eq("ROWID", SQLFunctionBuilder.LastInsertLowId().ToSqlString()));

                return connection.QueryAsync<RegisterImage>(selectQuery.ToSqlString()).Select(x => x.ToOption());
            });
        }
    }
}
