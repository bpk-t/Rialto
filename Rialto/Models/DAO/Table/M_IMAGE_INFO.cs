using Rialto.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using LangExt;

namespace Rialto.Models.DAO.Table
{
    public class M_IMAGE_INFO
    {
        public long? IMGINF_ID { get; set; }
        public int FILE_SIZE { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_TYPE { get; set; }
        public string HASH_VALUE { get; set; }
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

        public static IEnumerable<M_IMAGE_INFO> GetAll()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                return con.Query(
@"SELECT * FROM M_IMAGE_INFO IMGINF, T_AVEHASH AVEH
 WHERE IMGINF.DELETE_FLG='0'
 AND IMGINF.IMGINF_ID=AVEH.IMGINF_ID 
 ORDER BY IMGINF.IMGINF_ID DESC",
                         (M_IMAGE_INFO img, T_AVEHASH hash) => {
                             img.AVEHASH = hash.AVEHASH;
                             return img; 
                         }, splitOn: "IMGINF_ID,IMGINF_ID");
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

        public static Option<M_IMAGE_INFO> FindByHash(string hash)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                return Option.Create(
                con.Query("SELECT * FROM M_IMAGE_INFO WHERE TRIM(HASH_VALUE)=@HASH_VALUE"
                    , new { HASH_VALUE = hash }).FirstOrDefault());
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
    }
}
