using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Custom;
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
            
            ImageHasTags.FindByImgId(IMGINF_ID).ForEach(x =>
                ItemHaveTags.Add(new TagMasterInfo()
                {
                    ID = x.TAGINF_ID.Value,
                    Name = x.TAG_NAME,
                    Define = x.TAG_DEFINE,
                    CreatedAt = x.CREATE_LINE_DATE,
                    UpdatedAt = x.UPDATE_LINE_DATE
                }));
        }
    }
}
