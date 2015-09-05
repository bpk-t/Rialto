using Livet.Commands;
using System;

namespace Rialto.Model.DataModel
{
    public class TagAddButtonInfo
    {
        public long? TagInfId { get; set; }

        /// <summary>
        /// ボタン名称
        /// </summary>
        public string Name { get; set; }

        #region ClickCommand
        private ViewModelCommand _ClickCommand;
        public ViewModelCommand ClickCommand
        {
            get
            {
                if (_ClickCommand == null)
                {
                    _ClickCommand = new ViewModelCommand(Click);
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
        public Action<TagAddButtonInfo> ClickEvent { get; set; }
    }
}
