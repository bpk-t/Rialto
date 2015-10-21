using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public class AllocatedTags : NotificationObject
    {
        private ObservableSynchronizedCollection<TagMasterInfo> ExistsTags_ = new ObservableSynchronizedCollection<TagMasterInfo>();
        public ObservableSynchronizedCollection<TagMasterInfo> ExistsTags
        {
            get
            {
                return ExistsTags_;
            }
            set
            {
                ExistsTags_ = value;
                RaisePropertyChanged(() => ExistsTags);
            }
        }

        public void GetAllocatedTags(long IMGINF_ID)
        {
            if (ExistsTags.Count > 0) { ExistsTags.Clear(); }
            
            ImageHasTags.FindByImgId(IMGINF_ID).ForEach(x =>
                ExistsTags.Add(new TagMasterInfo()
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
