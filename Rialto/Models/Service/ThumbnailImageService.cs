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
using Rialto.Models.DAO.Builder;
using System.Windows.Media.Imaging;

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

            OnChangePage += (v) => { }; // null check不要にするための空実装
        }

        private ActorSystem system;
        private IActorRef thumbnailImageActor;

        private long currentTagId = TagConstant.ALL_TAG_ID;
        private Order currentImageOrder = Order.Desc;
        private int currentPage = 0;
        public int OnePageItemCount { get; set; }

        public event Action<string> OnChangePage;

        public Task<bool> ExistsPrevPage()
        {
            var message = new ThumbnailImageActor.ExistsPrevPage(currentTagId, currentPage * OnePageItemCount);
            return thumbnailImageActor.Ask<bool>(message);
        }

        public Task<bool> ExistsNextPage()
        {
            var message = new ThumbnailImageActor.ExistsNextPage(currentTagId, currentPage * OnePageItemCount);
            return thumbnailImageActor.Ask<bool>(message);
        }

        public async Task Refresh()
        {
            var message = new ThumbnailImageActor.GotToPage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
            (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
            SetThumbnailImages(allCount, images);
        }

        public async Task GoToNextPage()
        {
            if (await ExistsNextPage())
            {
                currentPage++;
                var message = new ThumbnailImageActor.GotToPage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
                (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
                SetThumbnailImages(allCount, images);
            } else
            {
                await GoToFirstPage();
            }
        }

        public async Task GoToPrevPage()
        {
            if (await ExistsPrevPage())
            {
                currentPage--;
                var message = new ThumbnailImageActor.GotToPage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
                (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
                SetThumbnailImages(allCount, images);
            }
        }

        public async Task GoToFirstPage()
        {
            currentPage = 0;
            var message = new ThumbnailImageActor.GotToPage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
            (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
            SetThumbnailImages(allCount, images);
        }

        public async Task ShowThumbnailImage(long tagId)
        {
            currentPage = 0;
            currentTagId = tagId;
            currentImageOrder = Order.Desc;

            var message = new ThumbnailImageActor.GotToPage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
            (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
            SetThumbnailImages(allCount, images);
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
            currentImageOrder = Order.Asc;
            currentPage = 0;

            var message = new ThumbnailImageActor.GotToPage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
            (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
            ThumbnailImgList.Clear();
            images.ForEach(x => ThumbnailImgList.Add(x));
        }

        private void SetThumbnailImages(long allCount, List<ImageInfo> images)
        {
            ThumbnailImgList.Clear();
            images.ForEach(x => ThumbnailImgList.Add(x));
            OnChangePage($"{currentPage}/{allCount / OnePageItemCount}");
        }

        class ThumbnailImageActor : ReceiveActor
        {
            public class GotToPage
            {
                public long TagId { get; }
                public long Offset { get; }
                public long Limit { get; }
                public Order ImageOrder { get; }
                public GotToPage(long tagId, int offset, int limit, Order imageOrder)
                {
                    TagId = tagId;
                    Offset = offset;
                    Limit = limit;
                    ImageOrder = imageOrder;
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
                    Sender.Tell(GetThumbnailImage(message.TagId, message.Offset, message.Limit, message.ImageOrder));
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
                    return RegisterImageRepository.GetAllCount();
                }
                else if (tagId == TagConstant.NOTAG_TAG_ID)
                {
                    return RegisterImageRepository.GetNoTagCount();
                }
                else
                {
                    return RegisterImageRepository.GetByTagCount(tagId);
                }
            }

            private (long allCount, List<ImageInfo> imgList) GetThumbnailImage(long tagId, long offset, long limit, Order imageOrder)
            {
                ImageInfo RegisterImageToImageInfo(RegisterImage image) => new ImageInfo()
                {
                    ImgID = image.Id,
                    ThumbnailImageFilePath = GetThumbnailImage(image.FilePath, image.Md5Hash),
                    SourceImageFilePath = new Uri(Path.Combine(Properties.Settings.Default.ImgDataDirectory, image.FilePath))
                };
                ImageInfo LoadImage(ImageInfo imgInfo) {
                    var tmpBitmapImage = new BitmapImage();
                    tmpBitmapImage.BeginInit();
                    tmpBitmapImage.UriSource = imgInfo.ThumbnailImageFilePath;
                    tmpBitmapImage.DecodePixelWidth = 200;
                    tmpBitmapImage.EndInit();
                    tmpBitmapImage.Freeze();
                    imgInfo.DispImage = tmpBitmapImage;
                    return imgInfo;
                };

                if (tagId == TagConstant.ALL_TAG_ID)
                {
                    var list = RegisterImageRepository.GetAll(offset, limit, imageOrder)
                        .Select(x => RegisterImageToImageInfo(x))
                        .ToList();

                    // 高速化のため、画像は並列読み込み
                    Parallel.For(0, list.Count, i => list[i] = LoadImage(list[i]));
                    var count = RegisterImageRepository.GetAllCount();
                    return (count, list);
                }
                else if (tagId == TagConstant.NOTAG_TAG_ID)
                {
                    var list = RegisterImageRepository.GetNoTag(offset, limit, imageOrder).Select(x => RegisterImageToImageInfo(x)).ToList();
                    // 高速化のため、画像は並列読み込み
                    Parallel.For(0, list.Count, i => list[i] = LoadImage(list[i]));
                    var count = RegisterImageRepository.GetNoTagCount();
                    return (count, list);
                }
                else
                {
                    var list = RegisterImageRepository.GetByTag(tagId, offset, limit, imageOrder).Select(x => RegisterImageToImageInfo(x)).ToList();
                    // 高速化のため、画像は並列読み込み
                    Parallel.For(0, list.Count, i => list[i] = LoadImage(list[i]));
                    var count = RegisterImageRepository.GetByTagCount(tagId);
                    return (count, list);
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
