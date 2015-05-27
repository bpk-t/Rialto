using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.ViewModels.Contents
{
    public class ButtonInfo
    {
        /// <summary>
        /// ボタン名称
        /// </summary>
        public string Name { get; set; }

        #region ClickCommand
        private Livet.Commands.ViewModelCommand _ClickCommand;
        public Livet.Commands.ViewModelCommand ClickCommand
        {
            get
            {
                if (_ClickCommand == null)
                {
                    _ClickCommand = new Livet.Commands.ViewModelCommand(Click);
                }
                return _ClickCommand;
            }
        }

        public void Click()
        {
            if (ClickEvent != null)
            {
                ClickEvent(this);
            }
        }
        #endregion

        /// <summary>
        /// ボタン押下時のイベント
        /// </summary>
        public Action<ButtonInfo> ClickEvent { get; set; }
    }
}
