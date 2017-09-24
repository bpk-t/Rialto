using System.Linq;
using System.Diagnostics;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;

using Rialto.Models;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Media.Imaging;
using Rialto.Model.DataModel;
using System;
using System.Collections.Generic;
using Rialto.Constant;
using NLog;
using NLog.Fluent;
using System.Threading.Tasks;
using Rialto.Models.Service;

namespace Rialto.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Private Members

        private static readonly Logger logger = LogManager.GetLogger("fileLogger");
        private ThumbnailImageService ThumbnailModel;
        private TagMasterService TaggingModel;
        private TagAllocateService tagAllocator;
        private AllocatedTagsService allocatedTags;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            logger.Debug().Write();
            ThumbnailModel = new ThumbnailImageService();
            TaggingModel = new TagMasterService();
            tagAllocator = new TagAllocateService(_SelectedThumbnailImgList);
            allocatedTags = new AllocatedTagsService();
        }

        private async Task Refresh()
        {
            ThumbnailItemSizeHeight = 200.0;
            ThumbnailItemSizeWidth = 200.0;

            await ThumbnailModel.ShowThumbnailImage(TagConstant.ALL_TAG_ID);
            tagAllocator.InitTabSettingPanel();
            await TaggingModel.InitTagTree();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public async void Initialize()
        {
            Properties.Settings.Default.Reset();
            await Refresh();
        }

        #region Properties

        /// <summary>
        /// 1つのサムネイル表示アイテムの高さ
        /// </summary>
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

        /// <summary>
        /// 1つのサムネイル表示アイテムの幅
        /// </summary>
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

        /// <summary>
        /// タグ検索文字列
        /// </summary>
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

        /// <summary>
        /// タグツリー上に表示するリスト
        /// </summary>
        private ReadOnlyDispatcherCollection<TagTreeNode> TagTreeItems_;
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

        /// <summary>
        /// タグツリー上で選択したタグ
        /// </summary>
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

        /// <summary>
        /// サムネイルに表示する画像リスト
        /// </summary>
        private ReadOnlyDispatcherCollection<ImageInfo> _ThumbnailImgList;
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

        /// <summary>
        /// サムネイル上で選択した画像リスト
        /// </summary>
        private ObservableCollection<ImageInfo> _SelectedThumbnailImgList = new ObservableCollection<ImageInfo>();
        public IList SelectedThumbnailImgList
        {
            get
            {
                return _SelectedThumbnailImgList;
            }
            set
            {
                _SelectedThumbnailImgList.Clear();
                foreach (ImageInfo elem in value)
                {
                    _SelectedThumbnailImgList.Add(elem);
                }
            }
        }

        /// <summary>
        /// タグ付加用パネル情報
        /// </summary>
        private ReadOnlyDispatcherCollection<TabInfo> TabPanels_;
        public ReadOnlyDispatcherCollection<TabInfo> TabPanels
        {
            get
            {
                if (TabPanels_ == null)
                {
                    TabPanels_ = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                        tagAllocator.TabPanels
                        , m => m
                        , DispatcherHelper.UIDispatcher
                        );
                }
                return TabPanels_;
            }
        }

        /// <summary>
        /// サイドペインに表示する画像（選択された画像）
        /// </summary>
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

        /// <summary>
        /// 選択された画像に付与されたタグ
        /// </summary>
        private ReadOnlyDispatcherCollection<TagMasterInfo> SelectedItemHaveTags_;
        public ReadOnlyDispatcherCollection<TagMasterInfo> SelectedItemHaveTags
        {
            get
            {
                if (SelectedItemHaveTags_ == null)
                {
                    SelectedItemHaveTags_ = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                        allocatedTags.ItemHaveTags
                        , m => m
                        , DispatcherHelper.UIDispatcher
                        );
                }
                return SelectedItemHaveTags_;
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// サムネイルリストでアイテムを選択した場合に呼び出される
        /// </summary>
        public void ThumbnailListSelectionChanged()
        {
            if (SelectedThumbnailImgList.Count > 0) {
                var selectedImg = SelectedThumbnailImgList[0] as ImageInfo;

                // TODO 読み込み処理
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = selectedImg.SourceImageFilePath;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                SideImage = image;

                allocatedTags.GetAllocatedTags(selectedImg.ImgID);
            }
        }

        /// <summary>
        /// タグツリー情報でアイテムを選択した場合に呼び出される
        /// </summary>
        public async void TagTreeSelectionChanged()
        {
            if (SelectedTagNode == null) return;
            await ThumbnailModel.ShowThumbnailImage(SelectedTagNode.ID);
        }

        #region SearchTagCommand
        private ViewModelCommand _OpenFullScreenViewCommand;

        public ViewModelCommand OpenFullScreenViewCommand
        {
            get
            {
                if (_OpenFullScreenViewCommand == null)
                {
                    _OpenFullScreenViewCommand = new ViewModelCommand(OpenFullScreenView);
                }
                return _OpenFullScreenViewCommand;
            }
        }

        /// <summary>
        /// 選択した画像をフルスクリーンで表示する
        /// </summary>
        public void OpenFullScreenView()
        {
            if (SelectedThumbnailImgList.Count > 0)
            {
                var selectedImgId = ((ImageInfo)SelectedThumbnailImgList[0]).ImgID;
                var currentIndex = Array.FindIndex(ThumbnailModel.ThumbnailImgList.ToArray(), (x) => x.ImgID == selectedImgId);

                Messenger.Raise(new TransitionMessage(
                    new FullScreenViewModel(currentIndex, ThumbnailModel), "ShowFullScreen"));
                //new FullScreenViewModel(currentIndex, ThumbnailModel.CurrentImageFilePathList), "ShowFullScreen"));
            }   
        }

        #endregion
        
        #region SearchTagCommand
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

        /// <summary>
        /// タグツリーから検索する
        /// </summary>
        public void SearchTag()
        {
            if (SearchTagText.Count() > 0)
            {
                TaggingModel.InitTagTree((x) => x.TAG_NAME.IndexOf(SearchTagText) >= 0);
            }
        }
        #endregion

        /// <summary>
        /// タグ検索テキストボックスが空になったら全タグを表示する
        /// </summary>
        public async void SearchTagTextChanged()
        {
            if (SearchTagText.Count() == 0)
            {
                await TaggingModel.InitTagTree();
            }
        }

        #region AddItemsCommand
        private ListenerCommand<IList<Uri>> _AddItemsCommand;
        public ListenerCommand<IList<Uri>> AddItemsCommand
        {
            get
            {
                if (_AddItemsCommand == null)
                {
                    _AddItemsCommand = new ListenerCommand<IList<Uri>>(AddItems);
                }
                return _AddItemsCommand;
            }
        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        /// <param name="uriList">ファイルリスト</param>
        private void AddItems(IList<Uri> uriList)
        {
            uriList.ForEach(x => Debug.WriteLine(x));
        }

        #endregion

        #region メニューコマンド

        public void CreateNewDB()
        {
            Debug.WriteLine("Call CreateNewDB");
        }

        /// <summary>
        /// 既存のDBを開く
        /// </summary>
        /// <param name="parameter"></param>
        public async void OpenDB(OpeningFileSelectionMessage parameter)
        {
            if (parameter.Response != null)
            {
                Properties.Settings.Default.LastOpenDbName = parameter.Response[0];
                Properties.Settings.Default.Save();
                await Refresh();
            }
        }

        /// <summary>
        /// タグ設定ダイアログを開く
        /// </summary>
        public void ShowTagSettingWindow()
        {
            Messenger.Raise(new TransitionMessage(new TagSettingWindowViewModel(), "ShowTagSetting"));
        }

        /// <summary>
        /// サムネイルをシャッフルする
        /// </summary>
        public async void Shuffle()
        {
            await ThumbnailModel.Shuffle();
        }

        /// <summary>
        /// サムネイルをリバースする
        /// </summary>
        public async void Reverse()
        {
            await ThumbnailModel.Reverse();
        }

        #endregion
        #endregion

        /// <summary>
        /// 前のページボタン有効
        /// </summary>
        private bool PrevPageButtonIsEnable_ = true;
        public bool PrevPageButtonIsEnable
        {
            get
            {
                return PrevPageButtonIsEnable_;
            }
            set
            {
                PrevPageButtonIsEnable_ = value;
                RaisePropertyChanged(() => PrevPageButtonIsEnable);
            }
        }

        /// <summary>
        /// 次のページボタン有効
        /// </summary>
        private bool NextPageButtoIsEnable_ = true;
        public bool NextPageButtoIsEnable
        {
            get
            {
                return NextPageButtoIsEnable_;
            }
            set
            {
                NextPageButtoIsEnable_ = value;
                RaisePropertyChanged(() => NextPageButtoIsEnable);
            }
        }

        public async void ShowPrevPage()
        {
            await ThumbnailModel.PrevPage();
        }

        public async void ShowNextPage()
        {
            await ThumbnailModel.NextPage();
        }
    }
}
