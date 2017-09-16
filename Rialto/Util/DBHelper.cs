using System;
using System.Collections.Generic;

using System.Data.SQLite;

namespace Rialto.Util
{
    public class DBHelper
    {

        /// <summary>
        /// 日付型データの初期値
        /// </summary>
        //public static readonly string DATETIME_DEFAULT_VALUE = "datetime('1900-01-01 00:00:00')";
        public static readonly DateTime DATETIME_DEFAULT_VALUE = new DateTime(1900, 1, 1, 0, 0, 0);
        
        /// <summary>
        /// SQLite 接続DB名
        /// </summary>
        public string DB_NAME
        {
            get
            {
                return Properties.Settings.Default.LastOpenDbName;
            }
        }

        private DBHelper()
        {
            //DB_NAME = Properties.Settings.Default.LastOpenDbName;
        }

        private static readonly Lazy<DBHelper> helper = new Lazy<DBHelper>(() => new DBHelper());

        public static DBHelper Instance
        {
            get
            {
                return helper.Value;
            }
        }

        /// <summary>
        /// SELECT文を実行する
        /// </summary>
        /// <param name="sql">実行するSQL文を指定</param>
        /// <returns></returns>
        public IEnumerable<SQLiteDataReader> ExecuteReader(string sql)
        {
            using (var conn = new SQLiteConnection("Data Source=" + DB_NAME))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = sql;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return reader;
                        }
                    }
                }
                conn.Close();
            }
        }

        /// <summary>
        /// UPDATE, INSERT文を実行する
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="replaceNames"></param>
        /// <param name="replaces"></param>
        public void ExecuteNonQuery(string sql, Dictionary<string, System.Data.DbType> replaceNames = null, List<Dictionary<string, object>> replaces = null)
        {
            using (var conn = new SQLiteConnection("Data Source=" + DB_NAME))
            {
                conn.Open();
                using (SQLiteTransaction sqlt = conn.BeginTransaction())
                {
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = sql;

                        if (replaceNames != null)
                        {
                            foreach (var r in replaceNames)
                            {
                                command.Parameters.Add(r.Key, r.Value);
                            }
                        }

                        if (replaces != null)
                        {
                            foreach (var r in replaces)
                            {
                                foreach (var p in r)
                                {
                                    command.Parameters[p.Key].Value = p.Value;
                                }
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    sqlt.Commit();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// DBの現在時間を取得する
        /// DBから取得できなかった場合、アプリケーション実行環境の現在時間を取得する
        /// </summary>
        /// <returns></returns>
        public DateTime GetCurrentDateTimeDb()
        {
            foreach (var current in ExecuteReader("SELECT datetime('now','localtime') AS CURRENTTIME"))
            {
                return DateTime.Parse(current["CURRENTTIME"].ToString());
            }
            return DateTime.Now;
        }

        /// <summary>
        /// COUNTを返す
        /// 結果のレコードが必ず1件になること
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int GetItemCount(string sql, string columnName = "ITEM_COUNT")
        {
            foreach (var r in ExecuteReader(sql))
            {
                return Convert.ToInt32(r[columnName].ToString());
            }
            return 0;
        }

        public SQLiteConnection GetDbConnection()
        {
            return new SQLiteConnection(GetConnectionString());
        }

        private string GetConnectionString()
        {
            return (new SQLiteConnectionStringBuilder
            {
                DataSource = DB_NAME,
            }).ToString();
        }
    }
}
