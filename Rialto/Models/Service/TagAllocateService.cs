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
using LanguageExt;
using static LanguageExt.Prelude;

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

        public Task<Unit> InitTabSettingPanelAsync()
        {
            using (var connection = DBHelper.Instance.GetDbConnection())
            {
                using (var tran = connection.BeginTransaction())
                {
                    TabPanels.Clear();
                    return TagRepository.GetAllTagGroupAsync(connection).SelectMany(results =>
                    {
                        results.ForEach(group =>
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
                                        TagRepository.InsertTagAssignAsync(connection,
                                            new TagAssign
                                            {
                                                RegisterImageId = x.ImgID,
                                                TagId = ev.TagInfId
                                            }));

                                        if (selectedThumbnailImgList.Count == 1)
                                        {
                                            var imgId = selectedThumbnailImgList[0].ImgID;

                                            GetAllocatedTags(imgId).Select(tags => {
                                                OnChange(tags);
                                                return unit;
                                            });
                                        }
                                    }
                                })
                                .ForEach(x => tabInfo.Buttons.Add(x));
                            TabPanels.Add(tabInfo);
                        });

                        return Task.FromResult(unit);
                    });
                }
            }
        }

        public Task<ObservableCollection<TagMasterInfo>> GetAllocatedTags(long imgId)
        {
            using (var connection = DBHelper.Instance.GetDbConnection())
            {
                using (var tran = connection.BeginTransaction())
                {
                    return TagRepository.GetTagByImageAssignedAsync(connection, imgId).Select(results =>
                    {
                        var tags = new ObservableCollection<TagMasterInfo>();
                        results.ForEach(x => tags.Add(TagToTagMaster(x)));
                        return tags;
                    });
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
