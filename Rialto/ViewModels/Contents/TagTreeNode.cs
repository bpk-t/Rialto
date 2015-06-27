using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Rialto.ViewModels.Contents
{
    public class TagTreeNode
    {
        /// <summary>
        /// ノード名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// タグID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// タグ割り当て画像数
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// 表示用
        /// </summary>
        public string DispName
        {
            private set { }
            get
            {
                return string.Format("{0}  ({1})", Name, ImageCount);
            }
        }

        /// <summary>
        /// 子ノード
        /// </summary>
        public ObservableCollection<TagTreeNode> Children { get; set; }

        public TagTreeNode()
        {
            Children = new ObservableCollection<TagTreeNode>();
        }
    }
}
