using Livet;
using Rialto.Constant;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Rialto.Models.Service
{
    public class TagMasterService : NotificationObject
    {
        public Task<ObservableCollection<TagTreeNode>> GetAllTag()
        {
            return GetAllTag((_) => true);
        }

        public Task<ObservableCollection<TagTreeNode>> GetAllTag(Func<Tag, bool> predicate)
        {
            return Task.Run(() =>
            {
                var tagTreeCollection = new ObservableCollection<TagTreeNode>();
                var list = TagRepository.GetAllTag();

                tagTreeCollection.Add(new TagTreeNode() {
                    ID = TagConstant.ALL_TAG_ID,
                    Name = "ALL",
                    ImageCount = 0
                    //ImageCount = M_TAG_INFO.GetAllImgCount()
                });
                tagTreeCollection.Add(new TagTreeNode() {
                    ID = TagConstant.NOTAG_TAG_ID,
                    Name = "NoTag",
                    ImageCount = 0
                    //ImageCount = M_TAG_INFO.GetHasNotTagImgCount()
                });
                list.Where(predicate)
                    .Select((x) => new TagTreeNode()
                    {
                        ID = x.Id,
                        Name = x.Name,
                        ImageCount = x.AssignImageCount
                    }).ForEach(x => tagTreeCollection.Add(x));

                return tagTreeCollection;
            });
        }
    }
}
