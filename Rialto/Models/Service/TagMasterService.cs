using Livet;
using Rialto.Constant;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using Rialto.Util;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Rialto.Models.Service
{
    public class TagMasterService : NotificationObject
    {
        public Task<ObservableCollection<TagTreeNode>> GetAllTagAsync()
        {
            return GetAllTagAsync((_) => true);
        }

        public Task<ObservableCollection<TagTreeNode>> GetAllTagAsync(Func<Tag, bool> predicate)
        {
            using (var connection = DBHelper.Instance.GetDbConnection())
            {
                using (var tran = connection.BeginTransaction())
                {
                    var allTagsTask = TagRepository.GetAllTagAsync(connection);
                    var allCountTask = RegisterImageRepository.GetAllCountAsync(connection);
                    var noTagCount = RegisterImageRepository.GetNoTagCountAsync(connection);

                    return Task.WhenAll(
                        allTagsTask,
                        allCountTask,
                        noTagCount
                        ).ContinueWith(nouse =>
                    {
                        var tagTreeCollection = new ObservableCollection<TagTreeNode>();
                        tagTreeCollection.Add(new TagTreeNode()
                        {
                            ID = TagConstant.ALL_TAG_ID,
                            Name = "ALL",
                            ImageCount = (int)allCountTask.Result
                        });
                        tagTreeCollection.Add(new TagTreeNode()
                        {
                            ID = TagConstant.NOTAG_TAG_ID,
                            Name = "NoTag",
                            ImageCount = (int)noTagCount.Result
                        });
                        allTagsTask.Result.Where(predicate)
                            .Select((x) => new TagTreeNode()
                            {
                                ID = x.Id,
                                Name = x.Name,
                                ImageCount = x.AssignImageCount
                            })
                            .ForEach(x => tagTreeCollection.Add(x));
                        return tagTreeCollection;
                    });
                }
            }
        }
    }
}
