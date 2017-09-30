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
using LangExt;
using System.Diagnostics;

namespace Rialto.Models.Repository
{
    public class M_IMAGE_INFORepository
    {

        public static long GetAllCount()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"));

                return con.ExecuteScalar<long>(query.ToSqlString());
            }
        }

        public static IEnumerable<M_IMAGE_INFO> GetAll(long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
                var list = con.Query<RegisterImage>("select * from register_image limit 1");
                Debug.WriteLine("************************************************");
                Debug.WriteLine(list.ToList()[0].Id);
                Debug.WriteLine(list.ToList()[0].FileName);
                Debug.WriteLine(list.ToList()[0].FilePath);
                Debug.WriteLine(list.ToList()[0].CreatedAt);
                Debug.WriteLine(list.ToList()[0].UpdatedAt);
                Debug.WriteLine("************************************************");

                var query = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"))
                    .OrderBy(M_IMAGE_INFO_DEF.IMGINF_ID, order)
                    .Limit(limit)
                    .Offset(offset);

                return con.Query(query.ToSqlString(), (M_IMAGE_INFO img, T_AVEHASH hash) => { img.AVEHASH = hash.AVEHASH; return img; }, splitOn: "IMGINF_ID,IMGINF_ID");
            }
        }

        public static long GetNoTagCount()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"))
                    .Where(ConditionBuilder.NotExists(
                        QueryBuilder.Select().From(T_ADD_TAG_DEF.ThisTable).Where(M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_ADD_TAG_DEF.IMGINF_ID))));

                return con.ExecuteScalar<long>(query.ToSqlString());
            }
        }

        public static IEnumerable<M_IMAGE_INFO> GetNoTag(long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"))
                    .Where(ConditionBuilder.NotExists(
                        QueryBuilder.Select().From(T_ADD_TAG_DEF.ThisTable).Where(M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_ADD_TAG_DEF.IMGINF_ID))
                        ))
                    .OrderBy(M_IMAGE_INFO_DEF.IMGINF_ID, order)
                    .Limit(limit)
                    .Offset(offset);

                return con.Query(query.ToSqlString(), (M_IMAGE_INFO img, T_AVEHASH hash) => {
                    img.AVEHASH = hash.AVEHASH;
                    return img;
                }, splitOn: "IMGINF_ID,IMGINF_ID");
            }
        }

        public static long GetByTagCount(long tagId)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .InnerJoin(T_ADD_TAG_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_ADD_TAG_DEF.IMGINF_ID))
                    .InnerJoin(M_TAG_INFO_DEF.ThisTable, T_ADD_TAG_DEF.TAGINF_ID.Eq(M_TAG_INFO_DEF.TAGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"))
                    .Where(M_TAG_INFO_DEF.TAGINF_ID.Eq("@TAGINF_ID"));

                return con.ExecuteScalar<long>(query.ToSqlString(), param: new { TAGINF_ID = tagId });
            }
        }

        public static IEnumerable<M_IMAGE_INFO> GetByTag(long tagId, long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .InnerJoin(T_ADD_TAG_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_ADD_TAG_DEF.IMGINF_ID))
                    .InnerJoin(M_TAG_INFO_DEF.ThisTable, T_ADD_TAG_DEF.TAGINF_ID.Eq(M_TAG_INFO_DEF.TAGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"))
                    .Where(M_TAG_INFO_DEF.TAGINF_ID.Eq("@TAGINF_ID"))
                    .OrderBy(M_IMAGE_INFO_DEF.IMGINF_ID, order)
                    .Limit(limit)
                    .Offset(offset);

                return con.Query(query.ToSqlString(),
         (M_IMAGE_INFO img, T_AVEHASH hash) => {
             img.AVEHASH = hash.AVEHASH;
             return img;
         }, splitOn: "IMGINF_ID,IMGINF_ID"
         , param: new { TAGINF_ID = tagId });
            }
        }

        public static Option<M_IMAGE_INFO> FindByHash(string hash)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .Where(ConditionBuilder.Eq(SQLFunctionBuilder.Trim(M_IMAGE_INFO_DEF.HASH_VALUE).ToSqlString(), "@HASH_VALUE"));

                return Option.Create(con.Query(query.ToSqlString(), new { HASH_VALUE = hash }).FirstOrDefault());
            }
        }

        public static Option<M_IMAGE_INFO> FindById(long id)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .Where(M_IMAGE_INFO_DEF.IMGINF_ID.Eq("@IMGINF_ID"));

                return Option.Create(con.Query(query.ToSqlString(), new { IMGINF_ID = id }).FirstOrDefault());
            }
        }

        public static M_IMAGE_INFO Insert(M_IMAGE_INFO info)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var tran = con.BeginTransaction();
                con.Execute(
                   @"INSERT INTO M_IMAGE_INFO(
FILE_SIZE,FILE_NAME,FILE_TYPE,HASH_VALUE,FILE_PATH,HEIGHT_PIX,WIDTH_PIX,COLOR,DO_GET,DELETE_FLG,DELETE_REASON_ID,DELETE_DATE)
 VALUES(@FILE_SIZE,@FILE_NAME,@FILE_TYPE,@HASH_VALUE,@FILE_PATH,@HEIGHT_PIX,@WIDTH_PIX,@COLOR,@DO_GET,@DELETE_FLG,@DELETE_REASON_ID,@DELETE_DATE)",
                   new
                   {
                       FILE_SIZE = info.FILE_SIZE,
                       FILE_NAME = info.FILE_NAME,
                       FILE_TYPE = info.FILE_TYPE,
                       HASH_VALUE = info.HASH_VALUE,
                       FILE_PATH = info.FILE_PATH,
                       HEIGHT_PIX = info.HEIGHT_PIX,
                       WIDTH_PIX = info.WIDTH_PIX,
                       COLOR = info.COLOR,
                       DO_GET = info.DO_GET,
                       DELETE_FLG = info.DELETE_FLG,
                       DELETE_REASON_ID = info.DELETE_REASON_ID,
                       DELETE_DATE = info.DELETE_DATE
                   });

                var selectQuery = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .Where(ConditionBuilder.Eq("ROWID", SQLFunctionBuilder.LastInsertLowId().ToSqlString()));
                var inserted = con.Query<M_IMAGE_INFO>(selectQuery.ToSqlString()).First();

                tran.Commit();

                return inserted;
            }
        }
    }
}
