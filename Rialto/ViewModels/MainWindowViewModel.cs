using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using Rialto.Models;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Rialto.ViewModels.Contents;
using Rialto.Util;
using Rialto.Models.DBModel;
using System.Threading.Tasks;
using System.Windows;
using Rialto.Constant;
using Rialto.Model.DataModel;

namespace Rialto.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Private Members

        private ThumbnailImage ThumbnailModel;

        private Tagging TaggingModel;
        
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            ThumbnailModel = new ThumbnailImage();

            TaggingModel = new Tagging();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            ThumbnailItemSizeHeight = 200.0;
            ThumbnailItemSizeWidth = 200.0;

            ExistsTags.Add(new TagMasterInfo { Name = "AAA" });
            ExistsTags.Add(new TagMasterInfo { Name = "BBB" });
            ExistsTags.Add(new TagMasterInfo { Name = "CCC" });

            ThumbnailModel.ReadThumbnailImage();
            InitTabSettingPanel();
            TaggingModel.InitTagTree();
        }

        private void InitTabSettingPanel()
        {
            M_TAGADDTAB.GetAll().ForEach(tabPanel =>
            {
                var tabInfo = new TabInfo
                {
                    Header = tabPanel.TAB_NAME
                };
                M_TAGADDTAB.GetTabSettings(tabPanel).ForEach(tabSetting =>
                {
                    tabInfo.Buttons.Add(new ButtonInfo()
                    {
                        Name = tabSetting,
                        ClickEvent = (ev) =>
                        {
                            Debug.WriteLine(ev.Name);
                        }
                    });
                });
                TabPanels.Add(tabInfo);
            });
        }

        private double ThumbnailItemSizeHeight_;
        public double ThumbnailItemSizeHeight
        {
            get
            {
                return ThumbnailItemSizeHeight_;
            }
            set
            {
                ThumbnailItemSizeHeight_ = value;
                RaisePropertyChanged(() => ThumbnailItemSizeHeight);
            }
        }

        private double ThumbnailItemSizeWidth_;
        public double ThumbnailItemSizeWidth
        {
            get
            {
                return ThumbnailItemSizeWidth_;
            }
            set
            {
                ThumbnailItemSizeWidth_ = value;
                RaisePropertyChanged(() => ThumbnailItemSizeWidth);
            }
        }

        ReadOnlyDispatcherCollection<TagTreeNode> TagTreeItems_;
        public ReadOnlyDispatcherCollection<TagTreeNode> TagTreeItems
        {
            get
            {
                if (TagTreeItems_ == null)
                {
                    TagTreeItems_ = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                        TaggingModel.TagTreeItems
                        , m => m
                        , DispatcherHelper.UIDispatcher
                        );
                }
                return TagTreeItems_;
            }
        }

        private BitmapImage SideImage_;
        public BitmapImage SideImage
        {
            get
            {
                return SideImage_;
            }
            set
            {
                SideImage_ = value;
                RaisePropertyChanged(() => SideImage);
            }
        }

        ReadOnlyDispatcherCollection<ImageInfo> _ThumbnailImgList;
        public ReadOnlyDispatcherCollection<ImageInfo> ThumbnailImgList
        {
            get
            {
                if (_ThumbnailImgList == null)
                {
                    _ThumbnailImgList = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                        ThumbnailModel.ThumbnailImgList
                        , m => m
                        , DispatcherHelper.UIDispatcher
                        );
                }
                return _ThumbnailImgList;
            }
        }

        private TagTreeNode SelectedTagNode_;
        public TagTreeNode SelectedTagNode
        {
            get
            {
                return SelectedTagNode_;
            }
            set
            {
                SelectedTagNode_ = value;
                RaisePropertyChanged(() => SelectedTagNode);
            }
        }

        private ObservableCollection<ImageInfo> SelectedThumbnailImgList_ = new ObservableCollection<ImageInfo>();
        public IList SelectedThumbnailImgList
        {
            get
            {
                return SelectedThumbnailImgList_;
            }
            set
            {
                SelectedThumbnailImgList_.Clear();
                foreach (ImageInfo elem in value)
                {
                    SelectedThumbnailImgList_.Add(elem);
                }
            }
        }

        private ObservableCollection<TabInfo> TabPanels_ = new ObservableCollection<TabInfo>();
        public ObservableCollection<TabInfo> TabPanels
        {
            get
            {
                return TabPanels_;
            }
            set
            {
                TabPanels_ = value;
                RaisePropertyChanged(() => TabPanels);
            }
        }

        private ObservableCollection<TagMasterInfo> ExistsTags_ = new ObservableCollection<TagMasterInfo>();
        public ObservableCollection<TagMasterInfo> ExistsTags
        {
            get
            {
                return ExistsTags_;
            }
            set
            {
                ExistsTags_ = value;
                RaisePropertyChanged(() => ExistsTags);
            }
        }

        private string SearchTagText_ = string.Empty;
        public string SearchTagText
        {
            get
            {
                return SearchTagText_;
            }
            set
            {
                SearchTagText_ = value;
                RaisePropertyChanged(() => SearchTagText);
            }
        }

        #region Command

        public void ListViewChenged()
        {
            if (SelectedThumbnailImgList.Count > 0) { 
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = ((ImageInfo)SelectedThumbnailImgList[0]).SourceImageFilePath;
                image.EndInit();
                SideImage = image;
            }
            
            Debug.WriteLine("Call ListViewChenged : " + SelectedThumbnailImgList.Count);
        }

        public void TagTreeSelectionChanged()
        {
            Debug.WriteLine("Selected Tag Name : " + SelectedTagNode.Name);
        }

        public void OpenFullScreenView()
        {
            Messenger.Raise(new TransitionMessage(new FullScreenViewModel(), "ShowFullScreen"));
        }

        private ViewModelCommand _SearchTagCommand;

        public ViewModelCommand SearchTagCommand
        {
            get
            {
                if (_SearchTagCommand == null)
                {
                    _SearchTagCommand = new ViewModelCommand(SearchTag);
                }
                return _SearchTagCommand;
            }
        }

        public void SearchTag()
        {
            if (SearchTagText.Count() > 0)
            {
                TaggingModel.InitTagTree((x) => x.TAG_NAME.IndexOf(SearchTagText) >= 0);
            }
        }

        /// <summary>
        /// タグ検索テキストボックスが空になったら全タグを表示する
        /// </summary>
        public void SearchTagTextChanged()
        {
            if (SearchTagText.Count() == 0)
            {
                TaggingModel.InitTagTree();
            }
        }

        #region メニューコマンド
        
        public void CreateNewDB()
        {
            Debug.WriteLine("Call CreateNewDB");
        }
        
        #region OpenDBCommand
        private ListenerCommand<OpeningFileSelectionMessage> _OpenDBCommand;

        public ListenerCommand<OpeningFileSelectionMessage> OpenDBCommand
        {
            get
            {
                if (_OpenDBCommand == null)
                {
                    _OpenDBCommand = new ListenerCommand<OpeningFileSelectionMessage>(OpenDB);
                }
                return _OpenDBCommand;
            }
        }

        public void OpenDB(OpeningFileSelectionMessage parameter)
        {
            if (parameter.Response != null)
            {
                Debug.WriteLine("OpenFile : " + parameter.Response[0]);
            }
        }
        #endregion

        public void ShowTagSettingWindow()
        {
            Messenger.Raise(new TransitionMessage(new TagSettingWindowViewModel(), "ShowTagSetting"));
        }

        #endregion
        #endregion
    }
}
