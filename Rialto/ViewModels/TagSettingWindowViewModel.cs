using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using Rialto.Models;
using System.Collections.ObjectModel;
using Rialto.ViewModels.Contents;
using Rialto.Models.DBModel;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Collections;

namespace Rialto.ViewModels
{
    public class TagSettingWindowViewModel : ViewModel
    {
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


        public void Initialize()
        {
            Task.Run(() =>
            {
                var list = M_TAG_INFO.GetAll();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    list.ForEach(x => AllTags.Add(x));
                });
            });
        }

        public void AddTag()
        {

        }

        public void ListViewChenged()
        {
            if (SelectedTags.Count > 0)
            {
                var selectedTag = SelectedTags[0] as M_TAG_INFO;
                TagNameText = selectedTag.TAG_NAME;
                TagDefineText = selectedTag.TAG_RUBY;
            }
        }
    }
}
