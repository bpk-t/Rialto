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
        /// 子ノード
        /// </summary>
        public ObservableCollection<TagTreeNode> Children { get; set; }

        public TagTreeNode(string name)
        {
            Name = name;
            Children = new ObservableCollection<TagTreeNode>();
        }
    }
}
