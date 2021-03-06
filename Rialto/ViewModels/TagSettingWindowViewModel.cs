﻿using System.Linq;

using Livet;
using System.Collections.ObjectModel;
using Rialto.Models.DAO.Table;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Diagnostics;
using Rialto.Models.DAO.Entity;
using Rialto.Models.Repository;
using Rialto.Models.Service;

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

        private TagMasterService tagMasterService;

        public TagSettingWindowViewModel()
        {
            tagMasterService = new TagMasterService();
        }

        public async void Initialize()
        {
            var tags = await tagMasterService.GetAllTagAsync();

            if (!AllTags.IsEmpty())
            {
                AllTags.Clear();
            }
            tags.ForEach(x => AllTags.Add(x));
        }
        
        /// <summary>
        /// 編集エリアのタグ情報を登録する
        /// </summary>
        public void AddTag()
        {
            tagMasterService.UpsertTag(TagNameText, TagNameText, TagDefineText);
            //Initialize();
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
