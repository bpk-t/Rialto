﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Rialto.ViewModels.Contents
{
    public class ImageInfo
    {
        public ImageInfo()
        {
        }

        /// <summary>
        /// サムネイルに表示する画像
        /// </summary>
        public BitmapImage DispImage { get; set; }

        /// <summary>
        /// サムネイルに表示する画像ファイル名
        /// </summary>
        public string DispImageName { get; set; }

        public void OpenImage(Uri uri)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;

            image.DecodePixelWidth = 100;
            //Image.DecodePixelHeight = 200;

            image.EndInit();
            DispImage = image; // ImageはProperty
        }
    }
}
