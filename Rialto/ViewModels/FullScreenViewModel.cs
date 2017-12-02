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

using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Rialto.Models.Service;

namespace Rialto.ViewModels
{

    public class FullScreenViewModel : ViewModel
    {
        #region Fields
        private ThumbnailImageService thumbnailImageService;

        private long selectedImgId;
        private LangExt.Option<Task<BitmapImage>> nextImageTask = LangExt.Option.None;

        #endregion

        private string _PageNumberView;
        public string PageNumberView
        {
            get { return _PageNumberView; }
            set {
                _PageNumberView = value;
                RaisePropertyChanged(nameof(PageNumberView));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        /// <param name="imageFilePathList"></param>
        public FullScreenViewModel(long selectedImgId, ThumbnailImageService thumbnailImage)
        {
            thumbnailImageService = thumbnailImage;
            thumbnailImageService.OnChangeSelect += (imgOptTry) =>
            {
                imgOptTry.ForEach(imgTry => 
                    imgTry.IfSucc(img =>
                    {
                        PageNumberView = $"{img.Index}/{img.AllCount}";
                        this.CurrentImage = img.Image.IfFailThrow(); // TODO
                        this.selectedImgId = img.ImageId;
                    })
                );
            };
            this.selectedImgId = selectedImgId;
        }

        public void Initialize()
        {
            thumbnailImageService.SelectImage(selectedImgId);
        }

        /// <summary>
        /// 現在表示している画像
        /// </summary>
        private BitmapImage _CurrentImage;
        public BitmapImage CurrentImage
        {
            get
            {
                return _CurrentImage;
            }
            set
            {
                _CurrentImage = value;
                RaisePropertyChanged(nameof(CurrentImage));
            }
        }

        #region ESCKeyDownCommand
        private ListenerCommand<object> _ESCKeyDownCommand;

        public ListenerCommand<object> ESCKeyDownCommand
        {
            get
            {
                if (_ESCKeyDownCommand == null)
                {
                    _ESCKeyDownCommand = new ListenerCommand<object>(ESCKeyDown);
                }
                return _ESCKeyDownCommand;
            }
        }

        /// <summary>
        /// Windowを閉じる
        /// </summary>
        /// <param name="param"></param>
        public void ESCKeyDown(object param)
        {
            if (param != null)
            {
                var window = param as Window;
                window.Close();
            }
        }
        #endregion

        #region NextPictureCommand
        private ListenerCommand<object> _NextPictureCommand;
        public ListenerCommand<object> NextPictureCommand
        {
            get
            {
                if (_NextPictureCommand == null)
                {
                    _NextPictureCommand = new ListenerCommand<object>(NextPicture);
                }
                return _NextPictureCommand;
            }
        }

        /// <summary>
        /// 次の画像を表示する
        /// </summary>
        /// <param name="parameter"></param>
        public void NextPicture(object parameter)
        {
            thumbnailImageService.NextImage();
        }
        #endregion

        #region PrevPictureCommand
        private ListenerCommand<object> _PrevPictureCommand;
        public ListenerCommand<object> PrevPictureCommand
        {
            get
            {
                if (_PrevPictureCommand == null)
                {
                    _PrevPictureCommand = new ListenerCommand<object>(PrevPicture);
                }
                return _PrevPictureCommand;
            }
        }

        /// <summary>
        /// 前の画像を表示する
        /// </summary>
        /// <param name="parameter"></param>
        public void PrevPicture(object parameter)
        {
            thumbnailImageService.PrevImageImage();
        }
        #endregion

    }
}
