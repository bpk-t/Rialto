using System.Linq;
using System.Diagnostics;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;

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
using System.Reactive.Linq;
using Akka.Actor;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Rialto.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Private Members

        private static readonly Logger logger = LogManager.GetLogger("fileLogger");
        private ThumbnailImageService thumbnailService;
        private TagMasterService tagMasterService;
        private TagAllocateService tagAllocateService;
        private DatabaseCreateService dbCreateService = new DatabaseCreateService();
        private ImageRegisterService imageRegisterService;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="system">ActorSystem</param>
        public MainWindowViewModel(ActorSystem system)
        {
            logger.Debug().Write();
            thumbnailService = new ThumbnailImageService(system);
            tagMasterService = new TagMasterService();
            tagAllocateService = new TagAllocateService(_SelectedThumbnailImgList, tags => SelectedItemHaveTags = tags);
            imageRegisterService = new ImageRegisterService(system);

            thumbnailService.OnChangePage += (value) =>
            {
                (int currentPage, int allPage, List<ImageInfo> images) = value;
                ThumbnailImgList.Clear();
                images.ForEach(img => ThumbnailImgList.Add(img));
                RefreshPageNumber(allPage, currentPage);
                CurrentPageAllPage = $"{currentPage}/{allPage}";
            };
            thumbnailService.OnChangeSelect += (value) =>
            {
                value.Match(
                    (some) =>
                    {
                        SideImage = some.Image;
                    },
                    () =>
                    {
                        SideImage = null;
                    }
                );
            
            };
        }

        private Task Refresh()
        {
            var thumbnailTask = thumbnailService.GetFirstPage(TagConstant.ALL_TAG_ID);
            var tagSettingTask = tagAllocateService.InitTabSettingPanelAsync();
            var initTagTask = tagMasterService.GetAllTagAsync()
                .ContinueWith(t => TagList = t.Result);

            return Task.WhenAll(thumbnailTask, tagSettingTask, initTagTask);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public async void Initialize()
        {
            ProgressBarVisible = true;
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

            ThumbnailImageSizeList = new ObservableCollection<ThumbnailImageSize>
            {
                new ThumbnailImageSize { ThumbnailSize = ThumbnailImageSize.Size.Large},
                new ThumbnailImageSize { ThumbnailSize = ThumbnailImageSize.Size.Middle},
                new ThumbnailImageSize { ThumbnailSize = ThumbnailImageSize.Size.Small},
            };
            SelectedThumbnailImageSize = ThumbnailImageSizeList[0];

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
                ProgressBarVisible = false;
            }
        }

        #region Properties


        /// <summary>
        /// ページ番号表示（現在のページ/すべてのページ数）
        /// </summary>
        private string CurrentPageAllPage_ = string.Empty;
        public string CurrentPageAllPage
        {
            get => CurrentPageAllPage_;
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
            get => ThumbnailItemSizeHeight_;
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
            get => ThumbnailItemSizeWidth_;
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
            get => SearchTagText_;
            set
            {
                SearchTagText_ = value;
                RaisePropertyChanged(() => SearchTagText);
            }
        }

        /// <summary>
        /// サイドペインに表示するタグの一覧
        /// </summary>
        private ObservableCollection<TagItem> TagList_;
        public ObservableCollection<TagItem> TagList
        {
            get => TagList_;
            set
            {
                TagList_ = value;
                RaisePropertyChanged(() => TagList);
            }
        }

        /// <summary>
        /// サイドペインに表示するタグの一覧から選択されたタグ
        /// </summary>
        private ObservableCollection<TagItem> SelectedTagItems_ = new ObservableCollection<TagItem>();
        public IList SelectedTagItems
        {
            get => SelectedTagItems_;
            set
            {
                SelectedTagItems_.Clear();
                foreach (TagItem elem in value)
                {
                    SelectedTagItems_.Add(elem);
                }
            }
        }

        private ObservableCollection<ImageInfo> ThumbnailImgList_ = new ObservableCollection<ImageInfo>();
        public ObservableCollection<ImageInfo> ThumbnailImgList
        {
            get => ThumbnailImgList_;
            set
            {
                ThumbnailImgList_ = value;
                RaisePropertyChanged(() => ThumbnailImgList);
            }
        }

        /// <summary>
        /// サムネイル上で選択した画像リスト
        /// </summary>
        private ObservableCollection<ImageInfo> _SelectedThumbnailImgList = new ObservableCollection<ImageInfo>();
        public IList SelectedThumbnailImgList
        {
            get => _SelectedThumbnailImgList;
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
            get => SideImage_;
            set
            {
                SideImage_ = value;
                RaisePropertyChanged(() => SideImage);
            }
        }

        private ObservableCollection<TagMasterInfo> SelectedItemHaveTags_ = new ObservableCollection<TagMasterInfo>();
        public ObservableCollection<TagMasterInfo> SelectedItemHaveTags
        {
            get => SelectedItemHaveTags_;
            set
            {
                SelectedItemHaveTags_ = value;
                RaisePropertyChanged(() => SelectedItemHaveTags);
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// サムネイルリストでアイテムを選択した場合に呼び出される
        /// </summary>
        public async void ThumbnailListSelectionChanged()
        {
            if (SelectedThumbnailImgList.Count > 0) {
                var selectedImg = SelectedThumbnailImgList[0] as ImageInfo;
                var getSelectedTask = tagAllocateService.GetAllocatedTags(selectedImg.ImgID);

                SelectedItemHaveTags = await getSelectedTask;
                await thumbnailService.SelectImage(selectedImg.ImgID);
            }
        }

        /// <summary>
        /// タグツリー情報でアイテムを選択した場合に呼び出される
        /// </summary>
        public async void TagTreeSelectionChanged()
        {
            if (SelectedTagItems.Count > 0)
            {
                ProgressBarVisible = true;
                var selected = SelectedTagItems[0] as TagItem;
                await thumbnailService.GetFirstPage(selected.ID);
                
                ProgressBarVisible = false;
            }
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
                Messenger.Raise(new TransitionMessage(
                    new FullScreenViewModel(selectedImgId, thumbnailService), "ShowFullScreen"));
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
                TagList = await tagMasterService.GetAllTagAsync((x) => x.Name.Contains(SearchTagText));
            }
            else
            {
                TagList = await tagMasterService.GetAllTagAsync();
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
        private async void AddItems(IList<Uri> uriList)
        {
            uriList.ForEach(x => Debug.WriteLine(x));

            var results = await imageRegisterService.RegisterImages(uriList, None);
            Refresh();
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
                }
                else
                {
                    Properties.Settings.Default.LastOpenDbName = filePath;
                    Properties.Settings.Default.Save();
                    dbCreateService.CreateSchema();
                    Refresh();
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
            get => PrevPageButtonIsEnable_;
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
            get => NextPageButtoIsEnable_;
            set
            {
                NextPageButtoIsEnable_ = value;
                RaisePropertyChanged(() => NextPageButtoIsEnable);
            }
        }

        /// <summary>
        /// ページ番号表示
        /// </summary>
        private string[] PageNumberList_ = null;
        public string[] PageNumberList
        {
            get => PageNumberList_;
            set
            {
                PageNumberList_ = value;
                RaisePropertyChanged(() => PageNumberList);
            }
        }

        /// <summary>
        /// ページ番号の現在位置
        /// </summary>
        private int[] PageNumberCurrentIndex_ = new int[] { 1, 0, 0, 0, 0 };
        public int[] PageNumberCurrentIndex
        {
            get => PageNumberCurrentIndex_;
            set
            {
                PageNumberCurrentIndex_ = value;
                RaisePropertyChanged(() => PageNumberCurrentIndex);
            }
        }

        public async void OnClickPageNumber(string number)
        {
            // 何故かstring型でしか取得できない
            ProgressBarVisible = true;
            var index = int.Parse(number);
            if (!string.IsNullOrEmpty(PageNumberList[index]))
            {
                var page = int.Parse(PageNumberList[index]);
                await thumbnailService.GoToPage(page);
            }
            ProgressBarVisible = false;
        }

        /// <summary>
        /// 前のページへ遷移する
        /// </summary>
        public async void ShowPrevPage()
        {
            ProgressBarVisible = true;
            await thumbnailService.GetPrevPage();
            ProgressBarVisible = false;
        }

        /// <summary>
        /// 次のページへ遷移する
        /// </summary>
        public async void ShowNextPage()
        {
            ProgressBarVisible = true;
            await thumbnailService.GoToNextPage();
            ProgressBarVisible = false;
        }

        private void RefreshPageNumber(int allPageCount, int currentPage)
        {
            if (PageNumberList == null 
                || currentPage == 1
                || !PageNumberList.HeadOrNone().Map(x => int.Parse(x)).Fold(false, (a, x) => x <= currentPage)
                || !PageNumberList.Reverse().HeadOrNone().Map(x => int.Parse(x)).Fold(false, (a, x) => x >= currentPage))
            {
                PageNumberList = Range(currentPage, Math.Max(allPageCount, 5))
                    .Take(5)
                    .Select(x => x <= allPageCount ? x.ToString() : "")
                    .ToArray();
            }
            PageNumberCurrentIndex = PageNumberList
                .Map(x => string.IsNullOrEmpty(x) ? None : Option<int>.Some(int.Parse(x)))
                .Map(x => x.Map(y => y == currentPage).IfNone(false) ? 1 : 0)
                .ToArray();
        }

        /// <summary>
        /// 1ページに表示するサムネイル画像の表示件数選択項目
        /// </summary>
        private ObservableCollection<PageViewImageCount> PageViewImageCountList_ = new ObservableCollection<PageViewImageCount>();
        public ObservableCollection<PageViewImageCount> PageViewImageCountList
        {
            get => PageViewImageCountList_;
            set
            {
                PageViewImageCountList_ = value;
                RaisePropertyChanged(() => PageViewImageCountList);
            }
        }

        /// <summary>
        /// 1ページに表示するサムネイル画像の表示件数（選択された項目）
        /// </summary>
        private PageViewImageCount SelectedPageViewImageCount_;
        public PageViewImageCount SelectedPageViewImageCount
        {
            get => SelectedPageViewImageCount_;
            set
            {
                SelectedPageViewImageCount_ = value;
                RaisePropertyChanged(() => SelectedPageViewImageCount);
            }
        }

        public async void PageViewImageCountSelectionChanged()
        {
            ProgressBarVisible = true;
            await thumbnailService.SetOnePageItemCountAndRefresh(SelectedPageViewImageCount.ImageCount);
            ProgressBarVisible = false;
        }

        /// <summary>
        /// サムネイル画像の大きさ選択項目
        /// </summary>
        private ObservableCollection<ThumbnailImageSize> ThumbnailImageSizeList_ = new ObservableCollection<ThumbnailImageSize>();
        public ObservableCollection<ThumbnailImageSize> ThumbnailImageSizeList
        {
            get => ThumbnailImageSizeList_;
            set
            {
                ThumbnailImageSizeList_ = value;
                RaisePropertyChanged(() => ThumbnailImageSizeList);
            }
        }

        /// <summary>
        /// サムネイル画像の大きさ（選択された項目）
        /// </summary>
        private ThumbnailImageSize SelectedThumbnailImageSize_;
        public ThumbnailImageSize SelectedThumbnailImageSize
        {
            get => SelectedThumbnailImageSize_;
            set
            {
                SelectedThumbnailImageSize_ = value;
                RaisePropertyChanged(() => SelectedThumbnailImageSize);

                switch (SelectedThumbnailImageSize_.ThumbnailSize)
                {
                    case ThumbnailImageSize.Size.Large:
                        ThumbnailItemSizeHeight = 200;
                        ThumbnailItemSizeWidth = 200;
                        break;
                    case ThumbnailImageSize.Size.Middle:
                        ThumbnailItemSizeHeight = 160;
                        ThumbnailItemSizeWidth = 160;
                        break;
                    case ThumbnailImageSize.Size.Small:
                        ThumbnailItemSizeHeight = 120;
                        ThumbnailItemSizeWidth = 120;
                        break;
                }
            }
        }

        private ErrorDialogViewModel ErrorDialog_ = null;
        public ErrorDialogViewModel ErrorDialog
        {
            get => ErrorDialog_;
            set
            {
                ErrorDialog_ = value;
                RaisePropertyChanged(() => ErrorDialog);
            }
        }

        private bool ErrorDialogIsOpen_ = false;
        public bool ErrorDialogIsOpen
        {
            get => ErrorDialogIsOpen_;
            set
            {
                ErrorDialogIsOpen_ = value;
                RaisePropertyChanged(() => ErrorDialogIsOpen);
            }
        }

        private Queue<int> processingQueue = new Queue<int>();
        public bool ProgressBarVisible
        {
            get => !processingQueue.IsEmpty();
            set
            {
                if (value)
                {
                    processingQueue.Enqueue(0);
                } else
                {
                    processingQueue.Dequeue();
                }
                RaisePropertyChanged(() => ProgressBarVisible);
            }
        }
    }
}
