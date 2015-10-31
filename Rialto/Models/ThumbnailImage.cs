﻿using System;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using Microsoft.VisualBasic;
using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Table;
using System.Collections.Generic;
using Rialto.Constant;

namespace Rialto.Models
{
    public class ThumbnailImage : NotificationObject
    {
        /// <summary>
        /// サムネイルに表示している画像リスト
        /// </summary>
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

        /// <summary>
        /// サムネイルに表示する予定含むすべての画像リスト
        /// </summary>
        public List<Uri> CurrentImageFilePathList { get; set; }

        public void ShowThumbnailImage(long tagId)
        {
            Task.Run(() =>
            {
                ThumbnailImgList.Clear();
                if (tagId == TagConstant.ALL_TAG_ID)
                {
                    ShowFirstPart(M_IMAGE_INFO.GetAll());
                }
                else if (tagId == TagConstant.NOTAG_TAG_ID)
                {
                    ShowFirstPart(M_IMAGE_INFO.GetNoTag());
                }
                else
                {
                    ShowFirstPart(M_IMAGE_INFO.GetByTag(tagId));
                }
            });
        }

        private void ShowFirstPart(IEnumerable<M_IMAGE_INFO> images)
        {
            images.Take(50).Select(x => new ImageInfo()
            {
                ImgID = x.IMGINF_ID.Value,
                DispImageName = x.FILE_NAME,
                ThumbnailImageFilePath = GetThumbnailImage(x.FILE_PATH, x.HASH_VALUE),
                SourceImageFilePath = new Uri(Path.GetFullPath(x.FILE_PATH))
            }).ForEach(x => ThumbnailImgList.Add(x));

            CurrentImageFilePathList = images.Select(x => new Uri(Path.GetFullPath(x.FILE_PATH))).ToList();
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
            return new Uri(Path.GetFullPath(filePath));
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
