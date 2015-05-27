using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Rialto.ViewModels.Contents
{
    public class TabInfo
    {
        /// <summary>
        /// タブ名
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// タブに表示するボタン
        /// </summary>
        private ObservableCollection<ButtonInfo> Buttons_ = new ObservableCollection<ButtonInfo>();
        public ObservableCollection<ButtonInfo> Buttons
        {
            get
            {
                return Buttons_;
            }
            set
            {
                Buttons_ = value;
            }
        }
    }
}
