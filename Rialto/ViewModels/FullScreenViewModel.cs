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

        private double _ImageWidth;
        public double ImageWidth
        {
            get { return _ImageWidth; }
            set
            {
                _ImageWidth = value;
                RaisePropertyChanged(nameof(ImageWidth));
            }
        }

        private double _ImageHeight;
        public double ImageHeight
        {
            get { return _ImageHeight; }
            set
            {
                _ImageHeight = value;
                RaisePropertyChanged(nameof(ImageHeight));
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
                        var screenHeight = SystemParameters.WorkArea.Height;
                        var screenWidth = SystemParameters.WorkArea.Width;
                        
                        PageNumberView = $"{img.Index}/{img.AllCount}";

                        var currentImage = img.Image.IfFailThrow(); // TODO
                        if (currentImage.PixelWidth <= screenWidth && currentImage.PixelHeight <= screenHeight)
                        {
                            ImageHeight = currentImage.PixelHeight;
                            ImageWidth = currentImage.PixelWidth;
                        }
                        else if (currentImage.PixelWidth > screenWidth && currentImage.PixelHeight > screenHeight)
                        {
                            var resizeHeight = screenHeight;
                            var resizeWidth = currentImage.PixelWidth * (screenHeight / currentImage.PixelHeight);

                            if (resizeWidth > screenWidth)
                            {
                                resizeHeight = resizeHeight * (screenWidth / resizeWidth);
                                resizeWidth = screenWidth;
                            }
                            ImageWidth = resizeWidth;
                            ImageHeight = resizeHeight;
                        }
                        else if (currentImage.PixelWidth > screenWidth)
                        {
                            ImageWidth = screenWidth;
                            ImageHeight = currentImage.PixelHeight * (screenWidth / currentImage.PixelWidth);
                        }
                        else if (currentImage.PixelHeight > screenHeight)
                        {
                            ImageHeight = screenHeight;
                            ImageWidth = currentImage.PixelWidth * (screenHeight / currentImage.PixelHeight);
                        }

                        this.CurrentImage = currentImage;
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
