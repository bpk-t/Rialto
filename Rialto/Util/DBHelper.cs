using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;

namespace Rialto.Util
{
    public class DBHelper
    {
        private static readonly Logger logger = LogManager.GetLogger("fileLogger");

        public static T Execute<T>(Func<DbConnection, DbTransaction, T> func, int retry = 0)
        {
            DbTransaction transaction = null;
            try
            {
                using (var connection = GetDbConnection())
                {
                    using (transaction = connection.BeginTransaction())
                    {
                        var result = func(connection, transaction);
                        transaction.Commit();
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ee)
                    {
                        logger.Error(ee);
                        throw ee;
                    }
                }
                if (retry <= 3)
                {
                    return Execute(func, retry + 1);
                } else
                {
                    throw e;
                }
            }
        }

        private static SQLiteConnection GetDbConnection()
        {
            var connection = new SQLiteConnection(GetConnectionString());
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            connection.Open();
            return connection;
        }

        private static string GetConnectionString()
        {
            return (new SQLiteConnectionStringBuilder
            {
                DataSource = Properties.Settings.Default.LastOpenDbName
            }).ToString();
        }
    }
}
