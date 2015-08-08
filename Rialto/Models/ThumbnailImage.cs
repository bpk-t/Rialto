﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DBModel;
using Rialto.Util;

namespace Rialto.Models
{
    public class ThumbnailImage : NotificationObject
    {
        private ObservableSynchronizedCollection<ImageInfo> ThumbnailImgList_ = new ObservableSynchronizedCollection<ImageInfo>();
        public ObservableSynchronizedCollection<ImageInfo> ThumbnailImgList
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

        public void ReadThumbnailImage()
        {
            Task.Run(() =>
            {
                var dispList = M_IMAGE_INFO.GetAll().ToList().GetRange(0, 50);
                dispList.Select(x => new ImageInfo()
                {
                    DispImageName = x.FILE_NAME,
                    ThumbnailImageFilePath = GetThumbnailImage(x.FILE_PATH, x.HASH_VALUE),
                    SourceImageFilePath = new Uri(x.FILE_PATH)
                }).ForEach(x => ThumbnailImgList.Add(x));
            });
        }

        /// <summary>
        /// サムネイル画像を返す
        /// </summary>
        /// <param name="imgPath">元画像のファイルパス</param>
        /// <param name="imgHash">元画像のハッシュ</param>
        /// <returns></returns>
        private Uri GetThumbnailImage(string imgPath, string imgHash)
        {
            var filePath = GetPath(imgHash);
            if (!File.Exists(filePath))
            {
                //CreateThumbnailImage(imgPath, imgHash);
            }
            return new Uri(filePath);
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(Properties.Settings.Default.ThumbnailImageDirectory, fileName);
        }

        /// <summary>
        /// サムネイル画像のディレクトリを作成する
        /// </summary>
        private void MakeThumbDir()
        {
            if (!Directory.Exists(Properties.Settings.Default.ThumbnailImageDirectory))
            {
                FileSystem.MkDir(Properties.Settings.Default.ThumbnailImageDirectory);
            }
        }
    }
}
