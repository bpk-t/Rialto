using Rialto.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Rialto.Models.Service
{
    public class DatabaseCreateService
    {
        public void CreateSchema()
        {
            DBHelper.Execute((connection, tran) =>
            {
                connection.Execute(
                    @"
                    CREATE TABLE ""register_image"" ( 
                      `id` INTEGER PRIMARY KEY AUTOINCREMENT
                      , `file_size` INTEGER NOT NULL
                      , `file_name` TEXT NOT NULL
                      , `file_extension` TEXT
                      , `file_path` TEXT NOT NULL
                      , `md5_hash` TEXT NOT NULL
                      , `ave_hash` TEXT NOT NULL
                      , `image_repository_id` INTEGER
                      , `height_pix` INTEGER NOT NULL
                      , `width_pix` INTEGER NOT NULL
                      , `do_get` INTEGER
                      , `delete_timestamp` TIMESTAMP
                      , `created_at` TIMESTAMP DEFAULT(datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT(datetime('now', 'localtime'))
                    )
                    ");

                connection.Execute(
                    @"
                    CREATE TABLE ""image_repository"" ( 
                      `id` INTEGER PRIMARY KEY AUTOINCREMENT
                      , `path` TEXT
                      , `created_at` TIMESTAMP DEFAULT(datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT(datetime('now', 'localtime'))
                    )
                    ");

                connection.Execute(
                    @"
                    CREATE TABLE ""tag"" ( 
                      `id` INTEGER PRIMARY KEY AUTOINCREMENT
                      , `name` TEXT NOT NULL
                      , `ruby` TEXT
                      , `search_count` INTEGER NOT NULL
                      , `assign_image_count` INTEGER NOT NULL
                      , `description` TEXT
                      , `created_at` TIMESTAMP DEFAULT(datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT(datetime('now', 'localtime'))
                    )
                ");

                connection.Execute(
                    @"
                    CREATE TABLE tag_group( 
                      id INTEGER PRIMARY KEY AUTOINCREMENT
                      , name TEXT NOT NULL
                      , `created_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                    )
                ");

                connection.Execute(
                    @"
                    CREATE TABLE tag_group_assign( 
                      tag_group_id INTEGER
                      , tag_id INTEGER
                      , `created_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , PRIMARY KEY (tag_group_id, tag_id)
                    )
                ");

                connection.Execute(
                    @"
                    CREATE TABLE tag_assign( 
                      register_image_id INTEGER
                      , tag_id INTEGER
                      , `created_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , PRIMARY KEY (register_image_id, tag_id)
                    )
                ");

                connection.Execute(
                    @"
                    CREATE TABLE image_score( 
                      register_image_id INTEGER
                      , tag_id INTEGER
                      , score INTEGER NOT NULL
                      , `created_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , PRIMARY KEY (register_image_id, tag_id)
                    )
                ");

                connection.Execute(
                    @"
                    CREATE TABLE view_history( 
                      id INTEGER PRIMARY KEY AUTOINCREMENT
                      , register_image_id INTEGER
                      , view_timestamp TIMESTAMP
                      , view_time_ms INTEGER NOT NULL
                      , `created_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                      , `updated_at` TIMESTAMP DEFAULT (datetime('now', 'localtime'))
                    );
                ");
                return unit;
            });
        }
    }
}
