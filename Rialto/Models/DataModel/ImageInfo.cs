﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Rialto.Model.DataModel
{
    public class ImageInfo
    {
        public ImageInfo()
        {
        }

        public int ImgID { get; set; }

        private BitmapImage _DispImage = null;

        /// <summary>
        /// サムネイルに表示する画像
        /// </summary>
        public BitmapImage DispImage
        {
            get
            {
                if (_DispImage == null)
                {
                    _DispImage = new BitmapImage();
                    _DispImage.BeginInit();
                    _DispImage.UriSource = ThumbnailImageFilePath;
                    _DispImage.DecodePixelWidth = 200;
                    _DispImage.EndInit();
                    _DispImage.Freeze();
                }
                return _DispImage;
            }
        }

        /// <summary>
        /// サムネイル用画像のファイルパス
        /// </summary>
        private Uri ThumbnailImageFilePath_;
        public Uri ThumbnailImageFilePath
        {
            get
            {
                return ThumbnailImageFilePath_;
            }
            set
            {
                ThumbnailImageFilePath_ = value;
            }
        }

        /// <summary>
        /// 元画像のファイルパス
        /// </summary>
        public Uri SourceImageFilePath { get; set; }
    }
}
