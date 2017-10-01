using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Data.Linq.Mapping;

namespace Rialto.Model.DataModel
{
    public class TagMasterInfo
    {
        /// <summary>
        /// タグ情報ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// タグ名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 検索回数
        /// </summary>
        public int SearchCount { get; set; }

        /// <summary>
        /// タグ定義（説明）
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// このタグに紐づく画像ファイル数を格納する
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
