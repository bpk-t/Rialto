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
using Rialto.Models.DataModel;
using System.Threading.Tasks;
using System.Windows;
using Rialto.Constant;

namespace Rialto.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public ThumbnailImage ThumbnailModel = new ThumbnailImage();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {

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

            InitThumbnailImage();
            InitTabSettingPanel();
            InitTagTree();
        }

        private void InitTagTree()
        {
            InitTagTree((_) => true);
        }
        private void InitTagTree(Func<M_TAG_INFO,bool> predicate)
        {
            Task.Run(() =>
            {
                var list = M_TAG_INFO.GetAll();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TagTreeItems.Clear();
                    TagTreeItems.Add(new TagTreeNode() { ID = TagConstant.ALL_TAG_ID, Name = "ALL", ImageCount = M_TAG_INFO.GetAllImgCount() });
                    TagTreeItems.Add(new TagTreeNode() { ID = TagConstant.NOTAG_TAG_ID, Name = "NoTag", ImageCount = M_TAG_INFO.GetHasNotTagImgCount() });
                    TagTreeItems.AddRange(
                        list.Where(predicate)
                            .Select((x) => new TagTreeNode()
                                        {
                                            ID = x.TAGINF_ID.GetValueOrDefault(0),
                                            Name = x.TAG_NAME,
                                            ImageCount = x.IMG_COUNT
                                        }));
                });
            });
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

        private void InitThumbnailImage()
        {
            Task.Run(() =>
            {
                var dispList = M_IMAGE_INFO.GetAll().ToList().GetRange(0, 50);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    dispList.Select(x => new ImageInfo()
                    {
                        DispImageName = x.FILE_NAME,
                        ThumbnailImageFilePath = ThumbnailModel.GetThumbnailImage(x.FILE_PATH, x.HASH_VALUE),
                        SourceImageFilePath = new Uri(x.FILE_PATH)
                    }).ForEach(x => ThumbnailImgList.Add(x));
                });
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

        private RangeObservableCollection<TagTreeNode> TagTreeItems_ = new RangeObservableCollection<TagTreeNode>();
        public RangeObservableCollection<TagTreeNode> TagTreeItems
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

        private ObservableCollection<ImageInfo> ThumbnailImgList_ = new ObservableCollection<ImageInfo>();
        public ObservableCollection<ImageInfo> ThumbnailImgList
        {
            get
            {
                return ThumbnailImgList_;
            }
            set
            {
                ThumbnailImgList_ = value;
                RaisePropertyChanged(() => ThumbnailImgList);
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
                InitTagTree((x) => x.TAG_NAME.IndexOf(SearchTagText) >= 0);
            }
        }

        /// <summary>
        /// タグ検索テキストボックスが空になったら全タグを表示する
        /// </summary>
        public void SearchTagTextChanged()
        {
            if (SearchTagText.Count() == 0)
            {
                InitTagTree();
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
