﻿using Livet;
using Rialto.Constant;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rialto.Models.Service
{
    public class TagMasterService : NotificationObject
    {
        private ObservableSynchronizedCollection<TagTreeNode> TagTreeItems_ = new ObservableSynchronizedCollection<TagTreeNode>();
        public ObservableSynchronizedCollection<TagTreeNode> TagTreeItems
        {
            get
            {
                return TagTreeItems_;
            }
            set
            {
                TagTreeItems_ = value;
                RaisePropertyChanged(() => TagTreeItems);
            }
        }

        public async Task InitTagTree()
        {
            await InitTagTree((_) => true);
        }

        public Task InitTagTree(Func<Tag, bool> predicate)
        {
            return Task.Run(() =>
            {
                //var list = M_TAG_INFORepository.GetAll();

                var list = TagRepository.GetAllTag();

                TagTreeItems.Clear();
                TagTreeItems.Add(new TagTreeNode() {
                    ID = TagConstant.ALL_TAG_ID,
                    Name = "ALL",
                    ImageCount = 0
                    //ImageCount = M_TAG_INFO.GetAllImgCount()
                });
                TagTreeItems.Add(new TagTreeNode() {
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
                    }).ForEach(x => TagTreeItems.Add(x));
            });
        }
    }
}
