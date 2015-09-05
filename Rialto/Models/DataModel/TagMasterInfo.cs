using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Data.Linq.Mapping;

namespace Rialto.Model.DataModel
{
    [Table(Name = "M_TAG_INFO")]
    public class TagMasterInfo
    {
        /// <summary>
        /// タグ情報ID
        /// </summary>
        [Column(Name = "TAGINF_ID", DbType = "INTEGER", IsPrimaryKey = true)]
        public int ID { get; set; }

        /// <summary>
        /// タグ名
        /// </summary>
        [Column(Name = "TAG_NAME", DbType = "TEXT")]
        public string Name { get; set; }

        /// <summary>
        /// 検索回数
        /// </summary>
        [Column(Name = "SEARCH_COUNT", DbType = "INTEGER")]
        public int SearchCount { get; set; }

        /// <summary>
        /// タグ定義（説明）
        /// </summary>
        [Column(Name = "TAG_DEFINE", DbType = "TEXT")]
        public string Define { get; set; }

        /// <summary>
        /// このタグに紐づく画像ファイル数を格納する
        /// </summary>
        [Column(Name = "IMG_COUNT", DbType = "INTEGER")]
        public int ImageCount { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        [Column(Name = "CREATE_LINE_DATE", DbType = "DATE")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Column(Name = "UPDATE_LINE_DATE", DbType = "DATE")]
        public DateTime UpdatedAt { get; set; }
    }
}
