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
using Rialto.Models.DataModel;
using System.IO;

namespace Rialto.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Private Members

        private static readonly Logger logger = LogManager.GetLogger("fileLogger");
        private ThumbnailImageService thumbnailService;
        private TagMasterService tagMasterService;
        private TagAllocateService tagAllocateService;
        private AllocatedTagsService allocatedTags;
        private DatabaseCreateService dbCreateService = new DatabaseCreateService();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            logger.Debug().Write();
            thumbnailService = new ThumbnailImageService();
            tagMasterService = new TagMasterService();
            tagAllocateService = new TagAllocateService(_SelectedThumbnailImgList);
            allocatedTags = new AllocatedTagsService();
            thumbnailService.OnChangePage += (value) => CurrentPageAllPage = value;
        }

        private async Task Refresh()
        {
            await thumbnailService.ShowThumbnailImage(TagConstant.ALL_TAG_ID);
            tagAllocateService.InitTabSettingPanel();
            await tagMasterService.InitTagTree();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public async void Initialize()
        {
            PageViewImageCountList = new ObservableCollection<PageViewImageCount>
            {
                new PageViewImageCount { ImageCount = 10 },
                new PageViewImageCount { ImageCount = 20 },
                new PageViewImageCount { ImageCount = 30 },
                new PageViewImageCount { ImageCount = 50 },
                new PageViewImageCount { ImageCount = 100 }
            };
            SelectedPageViewImageCount = PageViewImageCountList[2];
            thumbnailService.OnePageItemCount = SelectedPageViewImageCount.ImageCount;

            ThumbnailItemSizeHeight = 200.0;
            ThumbnailItemSizeWidth = 200.0;

            // DBファイルの存在チェック
            if (Properties.Settings.Default.LastOpenDbName.IsEmpty()
                || !File.Exists(Properties.Settings.Default.LastOpenDbName))
            {
                // TODO DBファイルが無い場合、新規でDBファイルを作成する
            }
            else
            {
                // TODO DBファイルが存在する
                await Refresh();
            }
        }

        #region Properties

        /// <summary>
        /// ページ番号表示（現在のページ/すべてのページ数）
        /// </summary>
        private string CurrentPageAllPage_ = string.Empty;
        public string CurrentPageAllPage
        {
            get
            {
                return CurrentPageAllPage_;
            }
            set
            {
                CurrentPageAllPage_ = value;
                RaisePropertyChanged(() => CurrentPageAllPage);
            }
        }

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
                        tagMasterService.TagTreeItems
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
                        thumbnailService.ThumbnailImgList
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
                        tagAllocateService.TabPanels
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
            await thumbnailService.ShowThumbnailImage(SelectedTagNode.ID);
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
                var currentIndex = Array.FindIndex(thumbnailService.ThumbnailImgList.ToArray(), (x) => x.ImgID == selectedImgId);

                Messenger.Raise(new TransitionMessage(
                    new FullScreenViewModel(currentIndex, thumbnailService), "ShowFullScreen"));
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
        public async void SearchTag()
        {
            if (SearchTagText.Count() > 0)
            {
                //await tagMasterService.InitTagTree((x) => x.TAG_NAME.IndexOf(SearchTagText) >= 0);
                await tagMasterService.InitTagTree((x) => x.Name.Contains(SearchTagText));
            }
            else
            {
                await tagMasterService.InitTagTree();
            }
        }
        #endregion

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

        public void CreateNewDB(SavingFileSelectionMessage param)
        {
            if (param.Response != null && !param.Response.IsEmpty())
            {
                var filePath = param.Response[0];

                if (File.Exists(filePath))
                {
                    // TODO 既に存在しているファイルのためエラー

                    ErrorDialog = new ErrorDialogViewModel("タイトル", "テスト");
                    ErrorDialogIsOpen = true;
                } else
                {

                    // TODO ファイル作成ダイアログ
                    // TODO 指定されたファイル名から新規DBファイルを作成する
                    //dbCreateService.CreateSchema();
                    Debug.WriteLine("Call CreateNewDB = " + param.Response[0]);
                }
            }

        }

        /// <summary>
        /// 既存のDBを開く
        /// </summary>
        /// <param name="param"></param>
        public async void OpenDB(OpeningFileSelectionMessage param)
        {
            if (param.Response != null)
            {
                Properties.Settings.Default.LastOpenDbName = param.Response[0];
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
            await thumbnailService.Shuffle();
        }

        /// <summary>
        /// サムネイルをリバースする
        /// </summary>
        public async void Reverse()
        {
            await thumbnailService.Reverse();
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
            await thumbnailService.GoToPrevPage();
        }

        public async void ShowNextPage()
        {
            await thumbnailService.GoToNextPage();
        }


        /// <summary>
        /// サムネイル上で選択した画像リスト
        /// </summary>
        private ObservableCollection<PageViewImageCount> PageViewImageCountList_ = new ObservableCollection<PageViewImageCount>();
        public ObservableCollection<PageViewImageCount> PageViewImageCountList
        {
            get
            {
                return PageViewImageCountList_;
            }
            set
            {
                PageViewImageCountList_ = value;
                RaisePropertyChanged(() => PageViewImageCountList);
            }
        }

        private PageViewImageCount SelectedPageViewImageCount_;
        public PageViewImageCount SelectedPageViewImageCount
        {
            get
            {
                return SelectedPageViewImageCount_;
            }
            set
            {
                SelectedPageViewImageCount_ = value;
                RaisePropertyChanged(() => SelectedPageViewImageCount);
            }
        }

        public async void PageViewImageCountSelectionChanged()
        {
            thumbnailService.OnePageItemCount = SelectedPageViewImageCount.ImageCount;
            await thumbnailService.Refresh();
        }

        private ErrorDialogViewModel ErrorDialog_ = null;
        public ErrorDialogViewModel ErrorDialog
        {
            get
            {
                return ErrorDialog_;
            }
            set
            {
                ErrorDialog_ = value;
                RaisePropertyChanged(() => ErrorDialog);
            }
        }

        private bool ErrorDialogIsOpen_ = false;
        public bool ErrorDialogIsOpen
        {
            get
            {
                return ErrorDialogIsOpen_;
            }
            set
            {
                ErrorDialogIsOpen_ = value;
                RaisePropertyChanged(() => ErrorDialogIsOpen);
            }
        }
    }
}
