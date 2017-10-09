using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using Rialto.Util;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rialto.Models.Service
{
    /// <summary>
    /// タグ割り当て
    /// </summary>
    public class TagAllocateService : NotificationObject
    {
        private IList<ImageInfo> selectedThumbnailImgList;

        // TODO 消す（メソッドの戻り値で返却するように修正）
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

        public event Action<ObservableCollection<TagMasterInfo>> OnChange;

        public TagAllocateService(IList<ImageInfo> selectedThumbnailImgList, Action<ObservableCollection<TagMasterInfo>> onChange)
        {
            this.selectedThumbnailImgList = selectedThumbnailImgList;
            this.OnChange += onChange;
        }

        public Task InitTabSettingPanelAsync()
        {
            return Task.Run(() =>
            {
                using (var connection = DBHelper.Instance.GetDbConnection())
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        TabPanels.Clear();
                        TagRepository.GetAllTagGroup(connection).ForEach(group =>
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
                                        TagRepository.InsertTagAssign(connection,
                                            new TagAssign
                                            {
                                                RegisterImageId = x.ImgID,
                                                TagId = ev.TagInfId
                                            }));

                                        if (selectedThumbnailImgList.Count == 1)
                                        {
                                            var imgId = selectedThumbnailImgList[0].ImgID;
                                            OnChange(GetAllocatedTags(imgId));
                                        }
                                    }
                                })
                                .ForEach(x => tabInfo.Buttons.Add(x));
                            TabPanels.Add(tabInfo);
                        });
                    }
                }
            });
        }

        public ObservableCollection<TagMasterInfo> GetAllocatedTags(long imgId)
        {
            using (var connection = DBHelper.Instance.GetDbConnection())
            {
                using (var tran = connection.BeginTransaction())
                {
                    var tags = new ObservableCollection<TagMasterInfo>();
                    TagRepository.GetTagByImageAssigned(connection, imgId).ForEach(x => tags.Add(TagToTagMaster(x)));
                    return tags;
                }
            }
        }

        private TagMasterInfo TagToTagMaster(Tag x)
        {
            return new TagMasterInfo()
            {
                ID = x.Id,
                Name = x.Name,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            };
        }
    }
}
