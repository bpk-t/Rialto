using System.Linq;

using Livet;
using System.Collections.ObjectModel;
using Rialto.Models.DAO;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Diagnostics;

namespace Rialto.ViewModels
{
    public class TagSettingWindowViewModel : ViewModel
    {
        #region Properties
        private ObservableCollection<M_TAG_INFO> AllTags_ = new ObservableCollection<M_TAG_INFO>();
        public ObservableCollection<M_TAG_INFO> AllTags
        {
            get
            {
                return AllTags_;
            }
            set
            {
                AllTags_ = value;
                RaisePropertyChanged(() => AllTags);
            }
        }

        private ObservableCollection<M_TAG_INFO> _SelectedTags = new ObservableCollection<M_TAG_INFO>();
        public IList SelectedTags
        {
            get
            {
                return _SelectedTags;
            }
            set
            {
                _SelectedTags.Clear();
                foreach (M_TAG_INFO elem in value)
                {
                    _SelectedTags.Add(elem);
                }
            }
        }

        private string _TagNameText;
        public string TagNameText
        {
            get
            { return _TagNameText; }
            set
            {
                if (_TagNameText == value)
                    return;
                _TagNameText = value;
                RaisePropertyChanged(() => TagNameText);
            }
        }

        private string _TagDefineText;
        public string TagDefineText
        {
            get
            { return _TagDefineText; }
            set
            { 
                if (_TagDefineText == value)
                    return;
                _TagDefineText = value;
                RaisePropertyChanged(()=>TagDefineText);
            }
        }

        #endregion

        public void Initialize()
        {
            Task.Run(() =>
            {
                var list = M_TAG_INFO.GetAll();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!AllTags.IsEmpty())
                    {
                        AllTags.Clear();
                    }
                    list.ForEach(x => AllTags.Add(x));
                });
            });
        }
        
        /// <summary>
        /// 編集エリアのタグ情報を登録する
        /// </summary>
        public void AddTag()
        {
            M_TAG_INFO target = null;
            if (SelectedTags.Count > 0)
            {
                target = SelectedTags[0] as M_TAG_INFO;
            }
            else
            {
                target = new M_TAG_INFO
                {
                    TAG_NAME = TagNameText,
                    TAG_DEFINE = TagDefineText
                };
            }
            
            M_TAG_INFO.Upsert(target, () => true);
            Initialize();
        }

        /// <summary>
        /// タグリストで選択されたタグ情報を編集エリアに表示する
        /// </summary>
        public void TagListSelectionChanged()
        {
            if (SelectedTags.Count > 0)
            {
                var selectedTag = SelectedTags[0] as M_TAG_INFO;
                TagNameText = selectedTag.TAG_NAME;
                TagDefineText = selectedTag.TAG_DEFINE;
            }
        }

        public void DeleteTag()
        {
            Debug.WriteLine("Delete Tag");
        }
    }
}
