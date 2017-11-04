using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LanguageExt;
using static LanguageExt.Prelude;
using Livet;

namespace Rialto.Model.DataModel
{
    public class ImageInfo : NotificationObject
    {
        public ImageInfo()
        {
            ImageVisible = false;
            LoadingVisible = true;
        }

        public long ImgID { get; set; }

        private ImageSource _DispImage = null;

        /// <summary>
        /// サムネイルに表示する画像
        /// </summary>
        public ImageSource DispImage
        {
            get
            {
                return _DispImage;
            }
            set
            {
                _DispImage = value;
                RaisePropertyChanged(() => DispImage);
            }
        }

        private Task<Try<BitmapImage>> imageTask;
        public void SetImage(Task<Try<BitmapImage>> loadingImageTask)
        {
            this.imageTask = loadingImageTask;
            imageTask.Select((Try<BitmapImage> imageTry) =>
            {
                ImageVisible = true;
                LoadingVisible = false;
                imageTry.Match(
                    succ => DispImage = succ,
                    fail => DispImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Spinner, System.Windows.Media.Brushes.Black, 200)
                    );
                return unit;
            });
        }

        /// <summary>
        /// 元画像のファイルパス
        /// </summary>
        public Uri SourceImageFilePath { get; set; }

        private bool _ImageVisible;
        public bool ImageVisible
        {
            get => _ImageVisible;
            set
            {
                _ImageVisible = value;
                RaisePropertyChanged(() => ImageVisible);
            }
        }
        private bool _LoadingVisible;
        public bool LoadingVisible
        {
            get => _LoadingVisible;
            set
            {
                _LoadingVisible = value;
                RaisePropertyChanged(() => LoadingVisible);
            }
        }
    }
}
