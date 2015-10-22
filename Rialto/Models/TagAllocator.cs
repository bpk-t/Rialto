using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Table;
using StatefulModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rialto.Models
{
    public class TagAllocator : NotificationObject
    {
        private IList<ImageInfo> selectedThumbnailImgList;

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
        
        public TagAllocator(IList<ImageInfo> selectedThumbnailImgList)
        {
            this.selectedThumbnailImgList = selectedThumbnailImgList;
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
                            selectedThumbnailImgList.ForEach(x => 
                                T_ADD_TAG.Insert(
                                    new T_ADD_TAG()
                                    {
                                        IMGINF_ID = x.ImgID,
                                        TAGINF_ID = ev.TagInfId
                                    }));
                        }
                    });
                });
                TabPanels.Add(tabInfo);
            });
        }
    }
}
