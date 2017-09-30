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
    }
}
