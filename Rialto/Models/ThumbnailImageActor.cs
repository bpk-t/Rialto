using Akka.Actor;
using Rialto.Model.DataModel;
using Rialto.Models.DAO.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using Rialto.Util;
using Rialto.Models.Repository;
using Rialto.Constant;
using Rialto.Models.Service;
using System.Windows.Media.Imaging;
using Rialto.Models.DAO.Entity;
using System.IO;
using System.Drawing;

namespace Rialto.Models
{
    public class ThumbnailImageActor : ReceiveActor
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

        public class GetImageMessage
        {
            public long ImgId { get; }
            public long TagId { get; }
            public GetImageMessage(long imgId, long tagId)
            {
                ImgId = imgId;
                TagId = tagId;
            }
        }

        public class GetNextImageMessage
        {
            public long ImgId { get; }
            public long TagId { get; }
            public GetNextImageMessage(long imgId, long tagId)
            {
                ImgId = imgId;
                TagId = tagId;
            }
        }

        public class GetPrevImageMessage
        {
            public long ImgId { get; }
            public long TagId { get; }
            public GetPrevImageMessage(long imgId, long tagId)
            {
                ImgId = imgId;
                TagId = tagId;
            }
        }

        public class GetFirstImageMessage
        {
            public long TagId { get; }
            public GetFirstImageMessage(long tagId)
            {
                TagId = tagId;
            }
        }

        public class GetLastImageMessage
        {
            public long TagId { get; }
            public GetLastImageMessage(long tagId)
            {
                TagId = tagId;
            }
        }

        private IActorRef cachedImageActor;
        private long cacheOffset;
        private List<ImageInfo> cacheImages = new List<ImageInfo>();

        public ThumbnailImageActor()
        {
            cachedImageActor = Context.ActorOf<CachedImageActor>(nameof(CachedImageActor));
            Receive<GotToPageMessage>((message) =>
            {
                var result = GetThumbnailImage(message.TagId, message.Offset, message.Limit, message.ImageOrder);
                result.Wait();

                cacheImages = result.Result.imgList;
                cacheOffset = message.Offset;
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
            Receive<GetImageMessage>((message) =>
            {
                Sender.Tell(GetImage(message.ImgId, message.TagId));
            });
            Receive<GetNextImageMessage>((message) =>
            {
                Sender.Tell(GetNextImage(message.ImgId, message.TagId));
            });
            Receive<GetPrevImageMessage>((message) =>
            {
                Sender.Tell(GetPrevImage(message.ImgId, message.TagId));
            });

            Receive<GetFirstImageMessage>((message) =>
            {
                var imgOpt = cacheImages
                    .HeadOrNone()
                    .SelectMany(img => GetImage(img.ImgID, message.TagId))
                    .ToOption();
                Sender.Tell(imgOpt);
            });

            Receive<GetLastImageMessage>((message) =>
            {
                var imgOpt = cacheImages.Rev()
                    .HeadOrNone()
                    .SelectMany(img => GetImage(img.ImgID, message.TagId))
                    .ToOption();
                Sender.Tell(imgOpt);
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

        private Option<Try<PagingImage>> GetImage(long imgId, long tagId)
        {
            var index = cacheImages.FindIndex(x => x.ImgID == imgId);
            if (index >= 0)
            {
                // 先読み
                if ((index + 1) < cacheImages.Count)
                {
                    var message = new CachedImageActor.LoadImageMessage(cacheImages[index + 1].SourceImageFilePath);
                    cachedImageActor.Ask(message);
                }
                var countTask = GetImageCount(tagId);
                countTask.Wait();

                return Option<ImageInfo>.Some(cacheImages[index]).Map(img => LoadBitmapImage(img.SourceImageFilePath)
                                                .Map(loadImage => new PagingImage(countTask.Result, loadImage, img.ImgID, cacheOffset + index))
                        );
            }
            else
            {
                return None;
            }
        }

        private Option<Try<PagingImage>> GetNextImage(long imgId, long tagId)
        {
            var index = cacheImages.FindIndex(x => x.ImgID == imgId);
            var nextIndex = index + 1;

            if (index >= 0 && cacheImages.Count > nextIndex)
            {
                // 先読み
                if ((index + 1) < cacheImages.Count)
                {
                    var message = new CachedImageActor.LoadImageMessage(cacheImages[index + 1].SourceImageFilePath);
                    cachedImageActor.Ask(message);
                }

                var countTask = GetImageCount(tagId);
                countTask.Wait();

                return Option<ImageInfo>.Some(cacheImages[nextIndex]).Map(img => LoadBitmapImage(img.SourceImageFilePath)
                        .Map(loadImage => new PagingImage(countTask.Result, loadImage, img.ImgID, cacheOffset + nextIndex))
                        );
            }
            else
            {
                return None;
            }
        }

        private Option<Try<PagingImage>> GetPrevImage(long imgId, long tagId)
        {
            var index = cacheImages.FindIndex(x => x.ImgID == imgId);
            var prevIndex = index - 1;

            if (index >= 0 && prevIndex >= 0 && cacheImages.Count > prevIndex)
            {
                var countTask = GetImageCount(tagId);
                countTask.Wait();

                return Option<ImageInfo>.Some(cacheImages[prevIndex])
                    .Map(img => LoadBitmapImage(img.SourceImageFilePath)
                        .Map(loadImage => new PagingImage(countTask.Result, loadImage, img.ImgID, cacheOffset + prevIndex))
                        );
            }
            else
            {
                return None;
            }
        }

        private Try<BitmapImage> LoadBitmapImage(Uri filePath)
        {
            var message = new CachedImageActor.LoadImageMessage(filePath);
            var result = cachedImageActor.Ask<Try<BitmapImage>>(message);
            result.Wait();
            return result.Result;
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

            ImageInfo LoadImage(ImageInfo imgInfo)
            {
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
