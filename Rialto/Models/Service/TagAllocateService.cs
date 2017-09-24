using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using StatefulModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rialto.Models.Service
{
    public class TagAllocateService : NotificationObject
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
        
        public TagAllocateService(IList<ImageInfo> selectedThumbnailImgList)
        {
            this.selectedThumbnailImgList = selectedThumbnailImgList;
        }

        public void InitTabSettingPanel()
        {
            M_TAGADDTABRepository.GetAll().ForEach(tabPanel =>
            {
                var tabInfo = new TabInfo
                {
                    Header = tabPanel.TAB_NAME
                };
                M_TAGADDTABRepository.GetTabSettings(tabPanel).ForEach(tabSetting =>
                {
                    tabInfo.Buttons.Add(new TagAddButtonInfo()
                    {
                        TagInfId = tabSetting.TagInfID,
                        Name = tabSetting.TagName,
                        ClickEvent = (ev) =>
                        {
                            selectedThumbnailImgList.ForEach(x => 
                                T_ADD_TAGRepository.Insert(
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
