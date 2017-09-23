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
using Rialto.Model.DataModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;

namespace Rialto.ViewModels
{

    public class FullScreenViewModel : ViewModel
    {
        #region Fields
        private int currentIndex;

        private ThumbnailImage thumbnailImageService;

        private LangExt.Option<Task<BitmapImage>> nextImageTask = LangExt.Option.None;

        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        /// <param name="imageFilePathList"></param>
        public FullScreenViewModel(int index, ThumbnailImage thumbnailImage)
        {
            currentIndex = index;
            thumbnailImageService = thumbnailImage;
        }

        public void Initialize()
        {
            var path = thumbnailImageService.CurrentImageFilePathList[currentIndex].SourceImageFilePath;
            CurrentImage = new BitmapImage(path);
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
        public async void NextPicture(object parameter)
        {
            currentIndex++;
            if (currentIndex >= thumbnailImageService.CurrentImageFilePathList.Count)
            {
                currentIndex = 0;
                await thumbnailImageService.NextPage();
            }
            var path = thumbnailImageService.CurrentImageFilePathList[currentIndex].SourceImageFilePath;
            CurrentImage = new BitmapImage(path);

            /*
            if (nextImageTask.IsSome)
            {
                CurrentImage = await nextImageTask.GetOr(null);
                nextImageTask = LangExt.Option.Some(
                    Task.Run<BitmapImage>(() =>
                    {
                        var nextImgPath = thumbnailImageService.CurrentImageFilePathList[currentIndex + 1].SourceImageFilePath;

                        using (var memStream = new MemoryStream())
                        {
                            using (var fileStream = File.OpenRead(nextImgPath.AbsolutePath))
                            {
                                memStream.SetLength(fileStream.Length);
                                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);

                                var image = new BitmapImage();
                                image.BeginInit();
                                image.StreamSource = memStream;
                                image.EndInit();
                                image.Freeze();
                                return image;
                            }
                        }

                    })
                );
            }
            */
            
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
        public async void PrevPicture(object parameter)
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                if (thumbnailImageService.ExistsPrevPage())
                {
                    await thumbnailImageService.PrevPage();
                    currentIndex = thumbnailImageService.CurrentImageFilePathList.Count - 1;
                }
            }
            var path = thumbnailImageService.CurrentImageFilePathList[currentIndex].SourceImageFilePath;
            CurrentImage = new BitmapImage(path);
        }
        #endregion

    }
}
