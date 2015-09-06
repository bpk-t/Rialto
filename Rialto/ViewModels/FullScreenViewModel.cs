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

namespace Rialto.ViewModels
{
    public class FullScreenViewModel : ViewModel
    {
        private int currentIndex;
        private List<Uri> currentImageFilePathList;

        public FullScreenViewModel(int index, List<Uri> imageFilePathList)
        {
            currentIndex = index;
            currentImageFilePathList = imageFilePathList;
        }

        public void Initialize()
        {
            CurrentImage = new BitmapImage(currentImageFilePathList[currentIndex]);
        }

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
            currentIndex++;
            if (currentIndex >= currentImageFilePathList.Count)
            {
                currentIndex = 0;
            }
            CurrentImage = new BitmapImage(currentImageFilePathList[currentIndex]);
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
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = currentImageFilePathList.Count - 1;
            }
            CurrentImage = new BitmapImage(currentImageFilePathList[currentIndex]);
        }
        #endregion

    }
}
