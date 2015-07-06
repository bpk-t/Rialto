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
using Rialto.Models.DataModel;
using System.Threading.Tasks;
using System.Windows;

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
                RaisePropertyChanged("AllTags");
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
    }
}
