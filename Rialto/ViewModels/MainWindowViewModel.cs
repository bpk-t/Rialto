﻿using System;
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

namespace Rialto.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /// <summary>
        /// すべての画像
        /// </summary>
        private static readonly long ALL_TAG_ID = 0;

        /// <summary>
        /// タグ付加されていない画像
        /// </summary>
        private static readonly long NOTAG_TAG_ID = -1;

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
            ThumbnailItemSizeHeight = 120.0;
            ThumbnailItemSizeWidth = 120.0;

            var images = new List<ImageInfo>();

            images.Add(new ImageInfo() { DispImageName = "001.jpg" });

            images[0].OpenImage(new Uri(@"C:\Users\kouichi\Pictures\cats\001.jpg"));

            for (int i = 0; i < 100; ++i)
            {
                images.ForEach(x => ThumbnailImgList.Add(x));
            }

            RaisePropertyChanged("ThumbnailImgList");

            M_TAGADDTAB.GetAll().ToList().ForEach(tabPanel =>
            {
                var tabInfo = new TabInfo
                {
                    Header = tabPanel.TAB_NAME
                };
                M_TAGADDTAB.GetTabSettings(tabPanel).ToList().ForEach(tabSetting =>
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

            ExistsTags.Add(new TagMasterInfo { Name = "AAA" });
            ExistsTags.Add(new TagMasterInfo { Name = "BBB" });
            ExistsTags.Add(new TagMasterInfo { Name = "CCC" });

            Task.Run(() =>
            {
                var list = M_TAG_INFO.GetAll().ToList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TagTreeItems.Add(new TagTreeNode() { ID = ALL_TAG_ID, Name = "ALL", ImageCount = M_TAG_INFO.GetAllImgCount() });
                    TagTreeItems.Add(new TagTreeNode() { ID = NOTAG_TAG_ID, Name = "NoTag", ImageCount = M_TAG_INFO.GetHasNotTagImgCount() });
                    list.ForEach(x => TagTreeItems.Add(
                                        new TagTreeNode() {
                                            ID=x.TAGINF_ID.GetValueOrDefault(0)
                                          , Name=x.TAG_NAME
                                          , ImageCount=x.IMG_COUNT}));
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

        private ObservableCollection<TagTreeNode> TagTreeItems_ = new ObservableCollection<TagTreeNode>();
        public ObservableCollection<TagTreeNode> TagTreeItems
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
                RaisePropertyChanged("SelectedTagNode");
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
                RaisePropertyChanged("TabPanels");
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
        #region ListViewChengedCommand
        private ViewModelCommand _ListViewChengedCommand;

        public ViewModelCommand ListViewChengedCommand
        {
            get
            {
                if (_ListViewChengedCommand == null)
                {
                    _ListViewChengedCommand = new ViewModelCommand(ListViewChenged);
                }
                return _ListViewChengedCommand;
            }
        }

        public void ListViewChenged()
        {
            Debug.WriteLine("Call ListViewChenged : " + SelectedThumbnailImgList.Count);
        }
        #endregion


        #region TagTreeSelectionChangedCommand
        private ViewModelCommand _TagTreeSelectionChangedCommand;

        public ViewModelCommand TagTreeSelectionChangedCommand
        {
            get
            {
                if (_TagTreeSelectionChangedCommand == null)
                {
                    _TagTreeSelectionChangedCommand = new ViewModelCommand(TagTreeSelectionChanged);
                }
                return _TagTreeSelectionChangedCommand;
            }
        }

        public void TagTreeSelectionChanged()
        {
            Debug.WriteLine("Selected Tag Name : " + SelectedTagNode.Name);
        }
        #endregion

        #region OpenFullScreenViewCommand
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

        public void OpenFullScreenView()
        {
            //TODO：MVVMに準拠
            var scr = new Rialto.Views.FullScreen();
            scr.ShowDialog();
        }
        #endregion


        #region メニューコマンド
        #region CreateNewDBCommand
        private ViewModelCommand _CreateNewDBCommand;

        public ViewModelCommand CreateNewDBCommand
        {
            get
            {
                if (_CreateNewDBCommand == null)
                {
                    _CreateNewDBCommand = new ViewModelCommand(CreateNewDB);
                }
                return _CreateNewDBCommand;
            }
        }

        public void CreateNewDB()
        {
            Debug.WriteLine("Call CreateNewDB");
        }
        #endregion


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


        #region ShowTagSettingWindowCommand
        private ViewModelCommand _ShowTagSettingWindowCommand;

        public ViewModelCommand ShowTagSettingWindowCommand
        {
            get
            {
                if (_ShowTagSettingWindowCommand == null)
                {
                    _ShowTagSettingWindowCommand = new ViewModelCommand(ShowTagSettingWindow);
                }
                return _ShowTagSettingWindowCommand;
            }
        }

        public void ShowTagSettingWindow()
        {
            //TODO：MVVMに準拠
            var scr = new Rialto.Views.TagSettingWindow();
            scr.ShowDialog();
        }
        #endregion


        #endregion
        #endregion
    }
}
