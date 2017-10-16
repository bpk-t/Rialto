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
using LanguageExt;
using static LanguageExt.Prelude;
using Rialto.Util;

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

        public ThumbnailImageService(ActorSystem system)
        {
            thumbnailImageActor = system.ActorOf<ThumbnailImageActor>(nameof(ThumbnailImageActor));

            // TODO 戻り値で返却するように修正する
            OnChangePage += (v) => { }; // null check不要にするための空実装
        }

        private IActorRef thumbnailImageActor;

        private long currentTagId = TagConstant.ALL_TAG_ID;
        private Order currentImageOrder = Order.Desc;
        private int currentPage = 0;
        public int OnePageItemCount { get; set; }

        public event Action<string> OnChangePage;

        public Task<bool> ExistsPrevPage()
        {
            var message = new ThumbnailImageActor.ExistsPrevPageMessage(currentTagId, currentPage * OnePageItemCount);
            return thumbnailImageActor.Ask<bool>(message);
        }

        public Task<bool> ExistsNextPage()
        {
            var message = new ThumbnailImageActor.ExistsNextPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount);
            return thumbnailImageActor.Ask<bool>(message);
        }

        /// <summary>
        /// 1ページの表示件数を変更
        /// </summary>
        /// <param name="onePageItemCount">1ページの表示件数</param>
        /// <returns></returns>
        public async Task SetOnePageItemCountAndRefresh(int onePageItemCount)
        {
            OnePageItemCount = onePageItemCount;
            var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
            (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
            SetThumbnailImages(allCount, images);
        }

        public async Task GoToNextPage()
        {
            if (await ExistsNextPage())
            {
                currentPage++;
                var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
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
                var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
                (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
                SetThumbnailImages(allCount, images);
            }
        }

        public async Task GoToFirstPage()
        {
            currentPage = 0;
            var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
            (long allCount, List<ImageInfo> images) = await thumbnailImageActor.Ask<(long allCount, List<ImageInfo> images)>(message);
            SetThumbnailImages(allCount, images);
        }

        public async Task ShowThumbnailImage(long tagId)
        {
            currentPage = 0;
            currentTagId = tagId;
            currentImageOrder = Order.Desc;

            var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
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

            var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
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
            public class GotToPageMessage
            {
                public long TagId { get; }
                public long Offset { get; }
                public long Limit { get; }
                public Order ImageOrder { get; }
                public GotToPageMessage(long tagId, int offset, int limit, Order imageOrder)
                {
                    TagId = tagId;
                    Offset = offset;
                    Limit = limit;
                    ImageOrder = imageOrder;
                }
            }

            // TODO トランザクション分けない
            public class ExistsNextPageMessage
            {
                public long TagId { get; }
                public long Offset { get; }
                public long Limit { get; }
                public ExistsNextPageMessage(long tagId, int offset, int limit)
                {
                    TagId = tagId;
                    Offset = offset;
                    Limit = limit;
                }
            }

            // TODO トランザクション分けない
            public class ExistsPrevPageMessage
            {
                public long TagId { get; }
                public long Offset { get; }
                public ExistsPrevPageMessage(long tagId, int offset)
                {
                    TagId = tagId;
                    Offset = offset;
                }
            }

            public ThumbnailImageActor()
            {
                Receive<GotToPageMessage>((message) =>
                {
                    var result = GetThumbnailImage(message.TagId, message.Offset, message.Limit, message.ImageOrder);
                    result.Wait();
                    Sender.Tell(result.Result);
                });
                Receive<ExistsNextPageMessage>((message) =>
                {
                    GetImageCount(message.TagId).Select(result =>
                    {
                        Sender.Tell(result > 0 && (result - 1) > message.Offset + message.Limit);
                        return unit;
                    });
                });
                Receive<ExistsPrevPageMessage>((message) =>
                {
                    GetImageCount(message.TagId).Select(result =>
                    {
                        Sender.Tell(result > 0 && message.Offset >= 0);
                        return unit;
                    });
                });
            }

            private Task<long> GetImageCount(long tagId)
            {
                using (var connection = DBHelper.Instance.GetDbConnection())
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        if (tagId == TagConstant.ALL_TAG_ID)
                        {
                            return RegisterImageRepository.GetAllCountAsync(connection);
                        }
                        else if (tagId == TagConstant.NOTAG_TAG_ID)
                        {
                            return RegisterImageRepository.GetNoTagCountAsync(connection);
                        }
                        else
                        {
                            return RegisterImageRepository.GetByTagCountAsync(connection, tagId);
                        }
                    }
                }
            }

            private Task<(long allCount, List<ImageInfo> imgList)> GetThumbnailImage(long tagId, long offset, long limit, Order imageOrder)
            {
                ImageInfo RegisterImageToImageInfo(Option<RegisterImage> img, Option<ImageRepository> repository)
                {
                    var sourceImagePath = Path.Combine(repository.Fold("", (a, x) => x.Path), img.Fold("", (a, x) => x.FilePath));
                    return new ImageInfo()
                    {
                        ImgID = img.Fold(0L, (acc, x) => x.Id),
                        ThumbnailImageFilePath = GetThumbnailImage(sourceImagePath, img.Fold("", (a, x) => x.Md5Hash)),
                        SourceImageFilePath = new Uri(sourceImagePath)
                    };
                }

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

                using (var connection = DBHelper.Instance.GetDbConnection())
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        if (tagId == TagConstant.ALL_TAG_ID)
                        {
                            var countTask = RegisterImageRepository.GetAllCountAsync(connection);
                            var getListTask = RegisterImageRepository.GetAllAsync(connection, offset, limit, imageOrder).Select(results =>
                                results.Select(x => RegisterImageToImageInfo(x.Item1, x.Item2)).ToList()
                            );
                            
                            return Task.WhenAll(countTask, getListTask).ContinueWith(nouse =>
                            {
                                var list = getListTask.Result;
                                // 高速化のため、画像は並列読み込み
                                Parallel.For(0, list.Count, i => list[i] = LoadImage(list[i]));
                                return (countTask.Result, list);
                            });
                            
                        }
                        else if (tagId == TagConstant.NOTAG_TAG_ID)
                        {
                            var countTask = RegisterImageRepository.GetNoTagCountAsync(connection);
                            var getListTask = RegisterImageRepository.GetNoTagAsync(connection, offset, limit, imageOrder).Select(results =>
                                results.Select(x => RegisterImageToImageInfo(x.Item1, x.Item2)).ToList()
                            );

                            return Task.WhenAll(countTask, getListTask).ContinueWith(nouse =>
                            {
                                var list = getListTask.Result;
                                // 高速化のため、画像は並列読み込み
                                Parallel.For(0, list.Count, i => list[i] = LoadImage(list[i]));
                                return (countTask.Result, list);
                            });
                        }
                        else
                        {
                            var countTask = RegisterImageRepository.GetByTagCountAsync(connection, tagId);
                            var getListTask = RegisterImageRepository.GetByTagAsync(connection, tagId, offset, limit, imageOrder).Select(results =>
                                results.Select(x => RegisterImageToImageInfo(x.Item1, x.Item2)).ToList()
                            );

                            return Task.WhenAll(countTask, getListTask).ContinueWith(nouse =>
                            {
                                var list = getListTask.Result;
                                // 高速化のため、画像は並列読み込み
                                Parallel.For(0, list.Count, i => list[i] = LoadImage(list[i]));
                                return (countTask.Result, list);
                            });
                        }
                    }
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
            private string CreateThumbnailImage(string imgPath, string imgHash)
            {
                using (var image = Image.FromFile(imgPath))
                {
                    var resizeH = image.Height;
                    var resizeW = image.Width;

                    // リサイズ後の縦横を計算
                    if (image.Height > 240)
                    {
                        resizeH = 240;
                        resizeW = (int)((double)resizeW * ((double)240 / (double)image.Height));
                    }

                    using (var canvas = new Bitmap(resizeW, resizeH))
                    {
                        using (var g = Graphics.FromImage(canvas))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.DrawImage(image, new Rectangle(0, 0, resizeW, resizeH));

                            var savePath = Path.Combine(Properties.Settings.Default.ThumbnailImageDirectory, imgHash);
                            // サムネイル画像の保存
                            canvas.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                            return savePath;
                        }
                    }
                }
            }
        }

    }
}
