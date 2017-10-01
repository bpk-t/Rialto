using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models.Service
{
    public class AllocatedTagsService : NotificationObject
    {
        private ObservableSynchronizedCollection<TagMasterInfo> ItemHaveTags_ = new ObservableSynchronizedCollection<TagMasterInfo>();
        public ObservableSynchronizedCollection<TagMasterInfo> ItemHaveTags
        {
            get
            {
                return ItemHaveTags_;
            }
            set
            {
                ItemHaveTags_ = value;
                RaisePropertyChanged(() => ItemHaveTags);
            }
        }

        public void GetAllocatedTags(long IMGINF_ID)
        {
            if (ItemHaveTags.Count > 0) { ItemHaveTags.Clear(); }

            TagRepository.GetTagByImageAssigned(IMGINF_ID).ForEach(x =>
            {
                ItemHaveTags.Add(new TagMasterInfo()
                {
                    ID = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                });
            });
        }
    }
}
