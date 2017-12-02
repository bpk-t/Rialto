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
    public class TagMasterService
    {
        public Task<ObservableCollection<TagItem>> GetAllTagItemAsync()
        {
            return GetAllTagItemAsync((_) => true);
        }

        public Task<ObservableCollection<TagItem>> GetAllTagItemAsync(Func<Tag, bool> predicate)
        {
            return DBHelper.Execute((connection, tran) =>
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
                        var tagTreeCollection = new ObservableCollection<TagItem>();
                        tagTreeCollection.Add(new TagItem()
                        {
                            ID = TagConstant.ALL_TAG_ID,
                            Name = "ALL",
                            ImageCount = (int)allCountTask.Result
                        });
                        tagTreeCollection.Add(new TagItem()
                        {
                            ID = TagConstant.NOTAG_TAG_ID,
                            Name = "NoTag",
                            ImageCount = (int)noTagCount.Result
                        });
                        allTagsTask.Result.Where(predicate)
                                .Select((x) => new TagItem()
                        {
                            ID = x.Id,
                            Name = x.Name,
                            ImageCount = x.AssignImageCount
                        })
                                .ForEach(x => tagTreeCollection.Add(x));
                        return tagTreeCollection;
                    });
            });
        }

        public Task<ObservableCollection<Tag>> GetAllTagAsync()
        {
            return DBHelper.Execute((connection, tran) =>
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
                        var tagTreeCollection = new ObservableCollection<Tag>();
                        allTagsTask.Result.ForEach(x => tagTreeCollection.Add(x));
                        return tagTreeCollection;
                    });
            });
        }

        public Task UpsertTag(string name, string ruby, string description)
        {
            return DBHelper.Execute((connection, tran) =>
            {
                var tag = new Tag()
                {
                    Name = name,
                    Ruby = ruby,
                    Description = description
                };
                return TagRepository.UpsertAsync(connection, tran, tag);
            });
        } 
    }
}
