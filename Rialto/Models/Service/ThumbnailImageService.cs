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
using Rialto.Models.Repository;
using Akka.Actor;

namespace Rialto.Models.Service
{
    public class ThumbnailImageService : NotificationObject
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

        public ThumbnailImageService()
        {
            system = ActorSystem.Create("ThumbnailImageServiceSystem");
            thumbnailImageActor = system.ActorOf<ThumbnailImageActor>("ThumbnailImageActor");
        }

        private ActorSystem system;
        private IActorRef thumbnailImageActor;

        private long CurrentTagId = TagConstant.ALL_TAG_ID;
        private int CurrentPage = 0;
        public int OnePageItemCount { get; } = 30;

        public Task<bool> ExistsPrevPage()
        {
            var message = new ThumbnailImageActor.ExistsPrevPage(CurrentTagId, CurrentPage * OnePageItemCount);
            return thumbnailImageActor.Ask<bool>(message);
        }

        public Task<bool> ExistsNextPage()
        {
            var message = new ThumbnailImageActor.ExistsNextPage(CurrentTagId, CurrentPage * OnePageItemCount);
            return thumbnailImageActor.Ask<bool>(message);
        }

        public async Task GoToNextPage()
        {
            if (await ExistsNextPage())
            {
                CurrentPage++;
                var message = new ThumbnailImageActor.GotToPage(CurrentTagId, CurrentPage * OnePageItemCount, OnePageItemCount);
                var images = await thumbnailImageActor.Ask<List<ImageInfo>>(message);

                ThumbnailImgList.Clear();
                images.ForEach(x => ThumbnailImgList.Add(x));
            }
        }

        public async Task GoToPrevPage()
        {
            if (await ExistsPrevPage())
            {
                CurrentPage--;
                var message = new ThumbnailImageActor.GotToPage(CurrentTagId, CurrentPage * OnePageItemCount, OnePageItemCount);
                var images = await thumbnailImageActor.Ask<List<ImageInfo>>(message);

                ThumbnailImgList.Clear();
                images.ForEach(x => ThumbnailImgList.Add(x));
            }
        }

        public async Task GoToFirstPage()
        {
            CurrentPage = 0;
            var message = new ThumbnailImageActor.GotToPage(CurrentTagId, CurrentPage * OnePageItemCount, OnePageItemCount);
            var images = await thumbnailImageActor.Ask<List<ImageInfo>>(message);
            ThumbnailImgList.Clear();
            images.ForEach(x => ThumbnailImgList.Add(x));
        }

        public async Task ShowThumbnailImage(long tagId)
        {
            CurrentPage = 0;
            CurrentTagId = tagId;

            var message = new ThumbnailImageActor.GotToPage(CurrentTagId, CurrentPage * OnePageItemCount, OnePageItemCount);
            var images = await thumbnailImageActor.Ask<List<ImageInfo>>(message);
            ThumbnailImgList.Clear();
            images.ForEach(x => ThumbnailImgList.Add(x));
        }

        public async Task Shuffle()
        {
            await Task.Run(() =>
            {
                // TODO 直す
                ThumbnailImgList.Clear();
                ThumbnailImgList.OrderBy(_ => Guid.NewGuid()).ForEach(x => ThumbnailImgList.Add(x));
            });
        }

        public async Task Reverse()
        {
            await Task.Run(() =>
            {
                // TODO 直す
                ThumbnailImgList.Clear();
                ThumbnailImgList.Reverse();
            });
        }

        class ThumbnailImageActor : ReceiveActor
        {
            public class GotToPage
            {
                public long TagId { get; }
                public long Offset { get; }
                public long Limit { get; }
                public GotToPage(long tagId, int offset, int limit)
                {
                    TagId = tagId;
                    Offset = offset;
                    Limit = limit;
                }
            }

            // TODO トランザクション分けない
            public class ExistsNextPage
            {
                public long TagId { get; }
                public long Offset { get; }
                public ExistsNextPage(long tagId, int offset)
                {
                    TagId = tagId;
                    Offset = offset;
                }
            }

            // TODO トランザクション分けない
            public class ExistsPrevPage
            {
                public long TagId { get; }
                public long Offset { get; }
                public ExistsPrevPage(long tagId, int offset)
                {
                    TagId = tagId;
                    Offset = offset;
                }
            }

            public ThumbnailImageActor()
            {
                Receive<GotToPage>((message) =>
                {
                    Sender.Tell(GetThumbnailImage(message.TagId, message.Offset, message.Limit));
                });
                Receive<ExistsNextPage>((message) =>
                {
                    Sender.Tell(GetImageCount(message.TagId) > message.Offset);
                });
                Receive<ExistsPrevPage>((message) =>
                {
                    Sender.Tell(GetImageCount(message.TagId) > 0 && message.Offset >= 0);
                });
            }

            private long GetImageCount(long tagId)
            {
                if (tagId == TagConstant.ALL_TAG_ID)
                {
                    return M_IMAGE_INFORepository.GetAllCount();
                }
                else if (tagId == TagConstant.NOTAG_TAG_ID)
                {
                    return M_IMAGE_INFORepository.GetNoTagCount();
                }
                else
                {
                    return M_IMAGE_INFORepository.GetByTagCount(tagId);
                }
            }

            private List<ImageInfo> GetThumbnailImage(long tagId, long offset, long limit)
            {
                ImageInfo ToImageInfo(M_IMAGE_INFO image) => new ImageInfo()
                {
                    ImgID = image.IMGINF_ID.Value,
                    ThumbnailImageFilePath = GetThumbnailImage(image.FILE_PATH, image.HASH_VALUE),
                    SourceImageFilePath = new Uri(Path.Combine(Properties.Settings.Default.ImgDataDirectory, image.FILE_PATH))
                };
                
                if (tagId == TagConstant.ALL_TAG_ID)
                {
                    return M_IMAGE_INFORepository.GetAll(offset, limit).Select(x => ToImageInfo(x)).ToList();
                }
                else if (tagId == TagConstant.NOTAG_TAG_ID)
                {
                    return M_IMAGE_INFORepository.GetNoTag(offset, limit).Select(x => ToImageInfo(x)).ToList();
                }
                else
                {
                    return M_IMAGE_INFORepository.GetByTag(tagId, offset, limit).Select(x => ToImageInfo(x)).ToList();
                }
            }

            /// <summary>
            /// サムネイル画像を返す
            /// </summary>
            /// <param name="imgPath">元画像のファイルパス</param>
            /// <param name="imgHash">元画像のハッシュ</param>
            /// <returns></returns>
            private Uri GetThumbnailImage(string imgPath, string imgHash)
            {
                string GetPath(string fileName)
                {
                    return Path.Combine(Properties.Settings.Default.ThumbnailImageDirectory, fileName);
                }
                var filePath = GetPath(imgHash);
                if (!File.Exists(filePath))
                {
                    CreateThumbnailImage(imgPath, imgHash);
                }
                return new Uri(Path.GetFullPath(filePath));
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
                            canvas.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
            }
        }

    }
}
