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
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull());

                return con.ExecuteScalar<long>(query.ToSqlString());
            }
        }

        public static IEnumerable<RegisterImage> GetAll(long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

                var query = QueryBuilder.Select()
                    .From(REGISTER_IMAGE.ThisTable)
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .OrderBy(REGISTER_IMAGE.ID, order)
                    .Limit(limit)
                    .Offset(offset);

                return con.Query<RegisterImage>(query.ToSqlString());
            }
        }

        public static long GetNoTagCount()
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(REGISTER_IMAGE.ThisTable)
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(ConditionBuilder.NotExists(
                        QueryBuilder.Select().From(TAG_ASSIGN.ThisTable).Where(REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))));

                return con.ExecuteScalar<long>(query.ToSqlString());
            }
        }

        public static IEnumerable<RegisterImage> GetNoTag(long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(REGISTER_IMAGE.ThisTable)
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(ConditionBuilder.NotExists(
                        QueryBuilder.Select().From(TAG_ASSIGN.ThisTable).Where(REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                        ))
                    .OrderBy(REGISTER_IMAGE.ID, order)
                    .Limit(limit)
                    .Offset(offset);

                return con.Query<RegisterImage>(query.ToSqlString());
            }
        }

        public static long GetByTagCount(long tagId)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select(SQLFunctionBuilder.Count("*").ToSqlString())
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(TAG_ASSIGN.ThisTable, REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                    .InnerJoin(TAG.ThisTable, TAG_ASSIGN.TAG_ID.Eq(TAG.ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(TAG.ID.Eq("@TAG_ID"));

                return con.ExecuteScalar<long>(query.ToSqlString(), param: new { TAG_ID = tagId });
            }
        }

        public static IEnumerable<RegisterImage> GetByTag(long tagId, long offset, long limit, Order order)
        {
            using (var con = DBHelper.Instance.GetDbConnection())
            {
                var query = QueryBuilder.Select()
                    .From(REGISTER_IMAGE.ThisTable)
                    .InnerJoin(TAG_ASSIGN.ThisTable, REGISTER_IMAGE.ID.Eq(TAG_ASSIGN.REGISTER_IMAGE_ID))
                    .InnerJoin(TAG.ThisTable, TAG_ASSIGN.TAG_ID.Eq(TAG.ID))
                    .Where(REGISTER_IMAGE.DELETE_TIMESTAMP.IsNull())
                    .Where(TAG.ID.Eq("@TAG_ID"))
                    .OrderBy(REGISTER_IMAGE.ID, order)
                    .Limit(limit)
                    .Offset(offset);

                return con.Query<RegisterImage>(query.ToSqlString(), param: new { TAG_ID = tagId });
            }
        }
    }
}
