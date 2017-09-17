using Rialto.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using LangExt;
using Rialto.Models.DAO.Util;
using Rialto.Models.DAO.Table;

namespace Rialto.Models.DAO.Entity
{
    public class M_IMAGE_INFO
    {
        public long? IMGINF_ID { get; set; }
        public int FILE_SIZE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_TYPE { get; set; }
        public string HASH_VALUE { get; set; }
        public int IMGREPO_ID { get; set; }
        public string FILE_PATH { get; set; }
        public int HEIGHT_PIX { get; set; }
        public int WIDTH_PIX { get; set; }
        public int COLOR { get; set; }
        public int DO_GET { get; set; }
        public int DELETE_FLG { get; set; }
        public int DELETE_REASON_ID { get; set; }
        public DateTime DELETE_DATE { get; set; }
        public DateTime CREATE_LINE_DATE { get; set; }
        public DateTime UPDATE_LINE_DATE { get; set; }

        public string AVEHASH { get; set; }

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

        public static IEnumerable<M_IMAGE_INFO> GetAll(long offset, long limit)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"))
                    .OrderBy(M_IMAGE_INFO_DEF.IMGINF_ID, Order.Desc)
                    .Limit(limit)
                    .Offset(offset);

                return con.Query(query.ToSqlString(), (M_IMAGE_INFO img, T_AVEHASH hash) => { img.AVEHASH = hash.AVEHASH; return img;}, splitOn: "IMGINF_ID,IMGINF_ID");
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
                        QueryBuilder.Select().From("T_ADD_TAG").Where(M_IMAGE_INFO_DEF.IMGINF_ID.Eq("T_ADD_TAG.IMGINF_ID"))));

                return con.ExecuteScalar<long>(query.ToSqlString());
            }
        }

        public static IEnumerable<M_IMAGE_INFO> GetNoTag(long offset, long limit)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(M_IMAGE_INFO_DEF.ThisTable)
                    .InnerJoin(T_AVEHASH_DEF.ThisTable, M_IMAGE_INFO_DEF.IMGINF_ID.Eq(T_AVEHASH_DEF.IMGINF_ID))
                    .Where(M_IMAGE_INFO_DEF.DELETE_FLG.Eq("'0'"))
                    .Where(ConditionBuilder.NotExists(
                        QueryBuilder.Select().From("T_ADD_TAG").Where(M_IMAGE_INFO_DEF.IMGINF_ID.Eq("T_ADD_TAG.IMGINF_ID"))
                        ))
                    .OrderBy(M_IMAGE_INFO_DEF.IMGINF_ID, Order.Desc)
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

        public static IEnumerable<M_IMAGE_INFO> GetByTag(long tagId, long offset, long limit)
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
                    .OrderBy(M_IMAGE_INFO_DEF.IMGINF_ID, Order.Desc)
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
                return Option.Create(
                con.Query("SELECT * FROM M_IMAGE_INFO WHERE IMGINF_ID=@IMGINF_ID"
                    , new { IMGINF_ID = id }).FirstOrDefault());
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

                var inserted = con.Query<M_IMAGE_INFO>("SELECT * FROM M_IMAGE_INFO WHERE ROWID = last_insert_rowid()").First();

                tran.Commit();

                return inserted;
            }
        }
    }
}
