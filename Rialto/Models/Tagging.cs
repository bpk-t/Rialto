using Livet;
using Rialto.Constant;
using Rialto.Model.DataModel;
using Rialto.Models.DAO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rialto.Models
{
    public class Tagging : NotificationObject
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

        public void InitTagTree()
        {
            InitTagTree((_) => true);
        }

        public void InitTagTree(Func<M_TAG_INFO, bool> predicate)
        {
            Task.Run(() =>
            {
                var list = M_TAG_INFO.GetAll();
                TagTreeItems.Clear();
                TagTreeItems.Add(new TagTreeNode() { ID = TagConstant.ALL_TAG_ID, Name = "ALL", ImageCount = M_TAG_INFO.GetAllImgCount() });
                TagTreeItems.Add(new TagTreeNode() { ID = TagConstant.NOTAG_TAG_ID, Name = "NoTag", ImageCount = M_TAG_INFO.GetHasNotTagImgCount() });
                list.Where(predicate)
                    .Select((x) => new TagTreeNode()
                    {
                        ID = x.TAGINF_ID.GetValueOrDefault(0),
                        Name = x.TAG_NAME,
                        ImageCount = x.IMG_COUNT
                    }).ForEach(x => TagTreeItems.Add(x));
            });
        }
    }
}
