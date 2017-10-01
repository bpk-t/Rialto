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
            TagRepository.GetAllTagGroup().ForEach(group =>
            {
                var tabInfo = new TabInfo
                {
                    Header = group.Key.Name,
                };

                group.Value.Select(tag =>
                    new TagAddButtonInfo()
                    {
                        TagInfId = tag.Id,
                        Name = tag.Name,
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
                    })
                    .ForEach(x => tabInfo.Buttons.Add(x));
                TabPanels.Add(tabInfo);
            });
        }
    }
}
