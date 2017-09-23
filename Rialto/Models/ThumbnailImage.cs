using System;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using Microsoft.VisualBasic;
using Livet;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Entity;
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

        private long CurrentTagId = TagConstant.ALL_TAG_ID;
        private int CurrentPage = 0;
        private long ImgCount = 0;
        public int PageCount { get; } = 30;

        public bool ExistsPrevPage()
        {
            return CurrentPage > 1;
        }

        public bool ExistsNextPage()
        {
            return (CurrentPage + 1) * PageCount <= ImgCount;
        }

        public async Task NextPage()
        {
            if (ExistsNextPage())
            {
                CurrentPage++;
                await Task.Run(() =>
                {
                    ShowThumbnailImage_(CurrentTagId);
                });
            }
        }

        public async Task PrevPage()
        {
            if (ExistsPrevPage())
            {
                CurrentPage--;
                await Task.Run(() =>
                {
                    ShowThumbnailImage_(CurrentTagId);
                });
            }
        }

        /// <summary>
        /// サムネイルに表示する予定含むすべての画像リスト
        /// </summary>
        public List<ImageInfo> CurrentImageFilePathList { get; set; }

        public async Task ShowThumbnailImage(long tagId)
        {
            CurrentPage = 0;
            CurrentTagId = tagId;
            await Task.Run(() =>
            {
                ShowThumbnailImage_(tagId);
            });
        }

        public async Task Shuffle()
        {
            await Task.Run(() =>
            {
                ThumbnailImgList.Clear();
                CurrentImageFilePathList = CurrentImageFilePathList.OrderBy(_ => Guid.NewGuid()).ToList();

                CurrentImageFilePathList.ForEach(x => ThumbnailImgList.Add(x));
            });
        }

        public async Task Reverse()
        {
            await Task.Run(() =>
            {
                ThumbnailImgList.Clear();
                CurrentImageFilePathList.Reverse();

                CurrentImageFilePathList.ForEach(x => ThumbnailImgList.Add(x));
            });
        }

        private void ShowThumbnailImage_(long tagId)
        {
            ThumbnailImgList.Clear();
            if (tagId == TagConstant.ALL_TAG_ID)
            {
                ImgCount = M_IMAGE_INFO.GetAllCount();
                LoadImage(M_IMAGE_INFO.GetAll(CurrentPage * PageCount, PageCount));
            }
            else if (tagId == TagConstant.NOTAG_TAG_ID)
            {
                ImgCount = M_IMAGE_INFO.GetNoTagCount();
                LoadImage(M_IMAGE_INFO.GetNoTag(CurrentPage * PageCount, PageCount));
            }
            else
            {
                ImgCount = M_IMAGE_INFO.GetByTagCount(tagId);
                LoadImage(M_IMAGE_INFO.GetByTag(tagId, CurrentPage * PageCount, PageCount));
            }

            CurrentImageFilePathList.ForEach(x => ThumbnailImgList.Add(x));
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

                        var savePath = Path.Combine(Properties.Settings.Default.ThumbnailImageDirectory, imgHash);
                        // サムネイル画像の保存
                        canvas.Save(savePath , System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }
    }
}
