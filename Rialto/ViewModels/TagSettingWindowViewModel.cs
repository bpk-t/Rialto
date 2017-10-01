using System.Linq;

using Livet;
using System.Collections.ObjectModel;
using Rialto.Models.DAO.Table;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Diagnostics;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;

namespace Rialto.ViewModels
{
    public class TagSettingWindowViewModel : ViewModel
    {
        #region Properties
        private ObservableCollection<Tag> AllTags_ = new ObservableCollection<Tag>();
        public ObservableCollection<Tag> AllTags
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

        private ObservableCollection<Tag> _SelectedTags = new ObservableCollection<Tag>();
        public IList SelectedTags
        {
            get
            {
                return _SelectedTags;
            }
            set
            {
                _SelectedTags.Clear();
                foreach (Tag elem in value)
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
                var list = TagRepository.GetAllTag();

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
            // TODO
            /*
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

            M_TAG_INFORepository.Upsert(target, () => true);
            */
            Initialize();
        }

        /// <summary>
        /// タグリストで選択されたタグ情報を編集エリアに表示する
        /// </summary>
        public void TagListSelectionChanged()
        {
            if (SelectedTags.Count > 0)
            {
                var selectedTag = SelectedTags[0] as Tag;
                TagNameText = selectedTag.Name;
                TagDefineText = selectedTag.Description;
            }
        }

        public void DeleteTag()
        {
            // TODO ほんとに削除するのか
            var selectedTag = SelectedTags[0] as Tag;
            //M_TAG_INFORepository.DeleteById(selectedTag.Id);
            Initialize();
        }
    }
}
