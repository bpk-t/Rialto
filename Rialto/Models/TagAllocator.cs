using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO;
using StatefulModel;
using System.Diagnostics;
using System.Linq;

namespace Rialto.Models
{
    public class TagAllocator : NotificationObject
    {
        private ReadOnlyNotifyChangedCollection<ImageInfo> SelectedThumbnailImgList = null;

        private Livet.ObservableSynchronizedCollection<TabInfo> TabPanels_ = new Livet.ObservableSynchronizedCollection<TabInfo>();
        public Livet.ObservableSynchronizedCollection<TabInfo> TabPanels
        {
            get
            {
                return TabPanels_;
            }
            set
            {
                TabPanels_ = value;
                RaisePropertyChanged(() => TabPanels);
            }
        }
        
        public TagAllocator()
        {

        }

        public void InitTabSettingPanel()
        {
            M_TAGADDTAB.GetAll().ForEach(tabPanel =>
            {
                var tabInfo = new TabInfo
                {
                    Header = tabPanel.TAB_NAME
                };
                M_TAGADDTAB.GetTabSettings(tabPanel).ForEach(tabSetting =>
                {
                    tabInfo.Buttons.Add(new TagAddButtonInfo()
                    {
                        TagInfId = tabSetting.TagInfID,
                        Name = tabSetting.TagName,
                        ClickEvent = (ev) =>
                        {
                            //TODO タグ割り当て処理
                            Debug.WriteLine(ev.Name);
                        }
                    });
                });
                TabPanels.Add(tabInfo);
            });
        }
    }
}
