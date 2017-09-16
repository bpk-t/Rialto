using System;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using Microsoft.VisualBasic;
using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Table;
using System.Collections.Generic;
using Rialto.Constant;
using System.Drawing;

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
        public List<ImageInfo> CurrentImageFilePathList { get; set; }

        public async Task ShowThumbnailImage(long tagId)
        {
            await Task.Run(() =>
            {
                ThumbnailImgList.Clear();
                if (tagId == TagConstant.ALL_TAG_ID)
                {
                    LoadImage(M_IMAGE_INFO.GetAll());
                }
                else if (tagId == TagConstant.NOTAG_TAG_ID)
                {
                    LoadImage(M_IMAGE_INFO.GetNoTag());
                }
                else
                {
                    LoadImage(M_IMAGE_INFO.GetByTag(tagId));
                }
                ShowFirstPart();
            });
        }

        public async Task Shuffle()
        {
            await Task.Run(() =>
            {
                ThumbnailImgList.Clear();
                CurrentImageFilePathList = CurrentImageFilePathList.OrderBy(_ => Guid.NewGuid()).ToList();
                ShowFirstPart();
            });
        }

        public async Task Reverse()
        {
            await Task.Run(() =>
            {
                ThumbnailImgList.Clear();
                CurrentImageFilePathList.Reverse();
                ShowFirstPart();
            });
        }

        private void LoadImage(IEnumerable<M_IMAGE_INFO> images)
        {
            CurrentImageFilePathList = images.Select(x => new ImageInfo()
            {
                ImgID = x.IMGINF_ID.Value,
                ThumbnailImageFilePath = GetThumbnailImage(x.FILE_PATH, x.HASH_VALUE),
                SourceImageFilePath = new Uri(Path.Combine(Properties.Settings.Default.ImgDataDirectory, x.FILE_PATH))
            }).ToList();
        }

        private void ShowFirstPart()
        {
            CurrentImageFilePathList.Take(50).ForEach(x => ThumbnailImgList.Add(x));
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
                CreateThumbnailImage(imgPath, imgHash);
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

        /// <summary>
        /// サムネイル画像を作成する
        /// </summary>
        /// <param name="imgPath">元画像のファイルパス</param>
        /// <param name="imgHash">元画像のハッシュ</param>
        private void CreateThumbnailImage(string imgPath, string imgHash)
        {
            using (var image = Image.FromFile(imgPath))
            {
                var resizeH = image.Height;
                var resizeW = image.Width;

                // リサイズ後の縦横を計算
                if (image.Height > 220)
                {
                    resizeH = 220;
                    resizeW = (int)((double)resizeW * ((double)220 / (double)image.Height));
                }

                using (var canvas = new Bitmap(resizeW, resizeH))
                {
                    using (var g = Graphics.FromImage(canvas))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(image, new Rectangle(0, 0, resizeW, resizeH));
                        // サムネイル画像の保存
                        canvas.Save(Properties.Settings.Default.ThumbnailImageDirectory + "\\" + imgHash, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }
    }
}
