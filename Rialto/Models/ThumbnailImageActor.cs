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

            /// <summary>
            /// 0始まり
            /// </summary>
            public long Page { get; }
            public long Limit { get; }
            public Order ImageOrder { get; }
            public GotToPageMessage(long tagId, long page, int limit, Order imageOrder)
            {
                TagId = tagId;
                Page = page;
                Limit = limit;
                ImageOrder = imageOrder;
            }
        }

        public class GoToNextPageMessage { }
        public class GoToPrevPageMessage { }
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
        public class GetNextImageMessage { }
        public class GetPrevImageMessage { }

        private IActorRef cachedImageActor;
        private long cacheTagId;
        private long cachePage;
        private long cacheLimit;
        private Order cacheImageOrder;
        private Option<long> cacheSelectedImageId = None;
        private List<ImageInfo> cacheImages = new List<ImageInfo>();

        public ThumbnailImageActor()
        {
            cachedImageActor = Context.ActorOf<CachedImageActor>(nameof(CachedImageActor));
            Receive<GotToPageMessage>((message) =>
            {
                var result = GetThumbnailImage(message.TagId, message.Page, message.Limit, message.ImageOrder);
                result.Wait();

                cacheImages = result.Result.ThumbnailImageList;
                cacheTagId = message.TagId;
                cachePage = message.Page;
                cacheLimit = message.Limit;
                cacheImageOrder = message.ImageOrder;
                Sender.Tell(result.Result);
            });
            Receive<GoToNextPageMessage>((message) => 
            {
                var result = GoToNextPage();
                result.Wait();
                Sender.Tell(result.Result);
            });
            Receive<GoToPrevPageMessage>((message) =>
            {
                var result = GotoPrevPage();
                result.Wait();
                Sender.Tell(result.Result);
            });
            Receive<GetImageMessage>((message) =>
            {
                cacheSelectedImageId = Some(message.ImgId);
                var result = GetImage(message.ImgId, message.TagId);
                result.Wait();
                Sender.Tell(result.Result);
            });
            Receive<GetNextImageMessage>((message) =>
            {
                var result = GetNextImage(cacheSelectedImageId.IfNone(() => throw new Exception()), cacheTagId)
                    .Select(x => {
                        x.IfSome(y => y.IfSucc(z => cacheSelectedImageId = Some(z.ImageId)));
                        return x;
                    });
                result.Wait();
                Sender.Tell(result.Result);
            });
            Receive<GetPrevImageMessage>((message) =>
            {
                var result = GetPrevImage(cacheSelectedImageId.IfNone(() => throw new Exception()), cacheTagId)
                    .Select(x => {
                        x.IfSome(y => y.IfSucc(z => cacheSelectedImageId = Some(z.ImageId)));
                        return x;
                    });
                result.Wait();
                Sender.Tell(result.Result);
            });
        }

        private Task<PagingInfo> GoToNextPage()
        {
            var nextPage = cachePage + 1;
            return ExistsNextPage(cacheTagId, nextPage, cacheLimit).SelectMany(existsNextPage =>
            {
                var page = existsNextPage ? nextPage : 0;
                cachePage = page;
                return GetThumbnailImage(cacheTagId, page, cacheLimit, cacheImageOrder).Select(x => {
                    cacheImages = x.ThumbnailImageList;
                    return x;
                });
            });
        }
        private Task<PagingInfo> GotoPrevPage()
        {
            var prevPage = cachePage - 1;
            return ExistsPrevPage(cacheTagId, prevPage).SelectMany(existsPrevPage =>
            {
                var page = existsPrevPage ? prevPage : 0;
                cachePage = page;
                return GetThumbnailImage(cacheTagId, page, cacheLimit, cacheImageOrder).Select(x => {
                    cacheImages = x.ThumbnailImageList;
                    return x;
                });
            });
        }
        private Task<bool> ExistsNextPage(long tagId, long page, long limit)
        {
            return GetImageCount(tagId)
                .Select(result => result > 0 && result > page * limit + limit);
        }
        private Task<bool> ExistsPrevPage(long tagId, long page)
        {
            return GetImageCount(tagId)
                .Select(result => result > 0 && page > 0);
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

        private Task<Option<Try<PagingImage>>> GetImage(long imgId, long tagId) => GetImage(imgId, tagId, None);
        private Task<Option<Try<PagingImage>>> GetImage(long imgId, long tagId, Option<PagingInfo> pageInfo)
        {
            if (cacheImages.IsEmpty())
            {
                return Task.FromResult<Option<Try<PagingImage>>>(None);
            }
            else
            {
                var index = cacheImages.FindIndex(x => x.ImgID == imgId);
                if (index >= 0)
                {
                    return GetImageCount(tagId).SelectMany(imgCount =>
                    {
                        var img = cacheImages[index];
                        return LoadBitmapImage(img.SourceImageFilePath).Select(loadImage =>
                        {
                            var pagingImage = new PagingImage(
                                                        allCount: imgCount,
                                                        image: loadImage,
                                                        imageId: img.ImgID,
                                                        index: cachePage + cacheLimit + index,
                                                        page: pageInfo);
                            return Some(Try(pagingImage));

                        });
                    });
                }
                else
                {
                    return Task.FromResult<Option<Try<PagingImage>>>(None);
                }
            }
        }

        private Task<Option<Try<PagingImage>>> GetNextImage(long imgId, long tagId)
        {
            if (cacheImages.IsEmpty())
            {
                return Task.FromResult<Option<Try<PagingImage>>>(None);
            }
            else
            {
                var index = cacheImages.FindIndex(x => x.ImgID == imgId);
                var nextIndex = index + 1;

                if (index >= 0)
                {
                    if (cacheImages.Count > nextIndex)
                    {
                        return Some(cacheImages[nextIndex])
                            .Select(img => GetImage(img.ImgID, cacheTagId))
                            .Fold(Task.FromResult<Option<Try<PagingImage>>>(None), (a, x) => x);
                    } else
                    {
                        // 次のページへ遷移
                        return GoToNextPage().SelectMany(nextPageInfo =>
                        {
                            return cacheImages.HeadOrNone()
                                    .Select(img => GetImage(img.ImgID, cacheTagId, Some(nextPageInfo)))
                                    .Fold(Task.FromResult<Option<Try<PagingImage>>>(None), (a, x) => x);
                        });
                    }
                } else
                {
                    // 指定したIDの画像が存在しない
                    return Task.FromResult<Option<Try<PagingImage>>>(None);
                }
            }
        }

        private Task<Option<Try<PagingImage>>> GetPrevImage(long imgId, long tagId)
        {
            if (cacheImages.IsEmpty())
            {
                return Task.FromResult<Option<Try<PagingImage>>>(None);
            }
            else
            {
                var index = cacheImages.FindIndex(x => x.ImgID == imgId);
                var prevIndex = index - 1;

                if (index >= 0)
                {
                    if (prevIndex >= 0 && cacheImages.Count > prevIndex)
                    {
                        return Some(cacheImages[prevIndex])
                            .Select(img => GetImage(img.ImgID, cacheTagId))
                            .Fold(Task.FromResult<Option<Try<PagingImage>>>(None), (a, x) => x);
                    }
                    else
                    {
                        return GotoPrevPage().SelectMany(prevPageInfo => {
                            return cacheImages.Rev().HeadOrNone()
                                    .Select(img => GetImage(img.ImgID, cacheTagId, Some(prevPageInfo)))
                                    .Fold(Task.FromResult<Option<Try<PagingImage>>>(None), (a, x) => x);
                        });
                    }
                }
                else
                {
                    // 指定したIDの画像が存在しない
                    return Task.FromResult<Option<Try<PagingImage>>>(None);
                }
            }
        }

        private Task<BitmapImage> LoadBitmapImage(Uri filePath)
        {
            var message = new CachedImageActor.LoadImageMessage(filePath);
            return cachedImageActor.Ask<Task<BitmapImage>>(message).SelectMany(x => x);
        }

        private Task<PagingInfo> GetThumbnailImage(long tagId, long page, long limit, Order imageOrder)
        {
            return _GetThumbnailImage(tagId, page, limit, imageOrder).Select(x => {
                // 先読み
                x.ThumbnailImageList
                    .Select(img => new CachedImageActor.LoadImageMessage(img.SourceImageFilePath))
                    .Fold(
                        Task.FromResult(0),
                        (acc, message) => {
                            return acc.SelectMany(nouse => {
                                return Task.Delay(TimeSpan.FromMilliseconds(1000))
                                        .ContinueWith(nouse2 => cachedImageActor.Ask(message))
                                        .SelectMany(y => y);
                                    })
                                    .Map(nouse => 0);
                            });
                return x;
            });
        }
        private Task<PagingInfo> _GetThumbnailImage(long tagId, long page, long limit, Order imageOrder)
        {
            ImageInfo RegisterImageToImageInfo(Option<RegisterImage> img, Option<ImageRepository> repository)
            {
                var sourceImagePath = Path.Combine(repository.Fold("", (a, x) => x.Path), img.Fold("", (a, x) => x.FilePath));
                var result = new ImageInfo()
                {
                    ImgID = img.Fold(0L, (acc, x) => x.Id),
                    SourceImageFilePath = new Uri(sourceImagePath)
                };

                var imageLoadTask = GetThumbnailImage(sourceImagePath, img.Fold("", (a, x) => x.Md5Hash))
                        .Bind(thumbnailImageUri => Try(BitmapImageAsyncFactory.Create(thumbnailImageUri.AbsolutePath)))
                        .BiFold(state: Task.FromResult(Try((BitmapImage)null)),
                           Succ: (acc, x) => x.Map(y => Try(y)),
                           Fail: (acc, e) => Task.FromResult(Try<BitmapImage>(e))
                        );
                result.SetImage(imageLoadTask);

                return result;
            }

            using (var connection = DBHelper.Instance.GetDbConnection())
            {
                using (var tran = connection.BeginTransaction())
                {
                    var offset = page * limit;

                    if (tagId == TagConstant.ALL_TAG_ID)
                    {
                        var countTask = RegisterImageRepository.GetAllCountAsync(connection);
                        var getListTask = RegisterImageRepository.GetAllAsync(connection, Some(offset), Some(limit), imageOrder).Select(results =>
                            results.Select(x => RegisterImageToImageInfo(x.Item1, x.Item2)).ToList()
                        );
                        var allPage = countTask.Result / limit + (countTask.Result % limit > 0 ? 1 : 0);
                        return Task.WhenAll(countTask, getListTask).ContinueWith(nouse => new PagingInfo(page + 1, allPage, getListTask.Result));
                    }
                    else if (tagId == TagConstant.NOTAG_TAG_ID)
                    {
                        var countTask = RegisterImageRepository.GetNoTagCountAsync(connection);
                        var getListTask = RegisterImageRepository.GetNoTagAsync(connection, Some(offset), Some(limit), imageOrder).Select(results =>
                            results.Select(x => RegisterImageToImageInfo(x.Item1, x.Item2)).ToList()
                        );
                        var allPage = countTask.Result / limit + (countTask.Result % limit > 0 ? 1 : 0);
                        return Task.WhenAll(countTask, getListTask).ContinueWith(nouse => new PagingInfo(page + 1, allPage, getListTask.Result));
                    }
                    else
                    {
                        var countTask = RegisterImageRepository.GetByTagCountAsync(connection, tagId);
                        var getListTask = RegisterImageRepository.GetByTagAsync(connection, tagId, Some(offset), Some(limit), imageOrder).Select(results =>
                            results.Select(x => RegisterImageToImageInfo(x.Item1, x.Item2)).ToList()
                        );
                        var allPage = countTask.Result / limit + (countTask.Result % limit > 0 ? 1 : 0);
                        return Task.WhenAll(countTask, getListTask).ContinueWith(nouse => new PagingInfo(page + 1, allPage, getListTask.Result));
                    }
                }
            }

        }

        /// <summary>
        /// サムネイル画像のパス情報を返す
        /// </summary>
        /// <param name="imgPath">元画像のファイルパス</param>
        /// <param name="imgHash">元画像のハッシュ</param>
        /// <returns></returns>
        private Try<Uri> GetThumbnailImage(string imgPath, string imgHash)
        {
            Try<string> GetPath(string fileName) => Try(() =>
                {
                    if (!Directory.Exists(Properties.Settings.Default.ThumbnailImageDirectory))
                    {
                        throw new Exception("サムネイル画像保存ディレクトリが存在しません。");
                    }
                    return Path.Combine(Properties.Settings.Default.ThumbnailImageDirectory, fileName);
                }
            );
            return GetPath(imgHash).Bind((string filePath) => {
                if (!File.Exists(filePath))
                {
                    return CreateThumbnailImage(imgPath, imgHash);
                } else
                {
                    return Try(() => new Uri(Path.GetFullPath(filePath)));
                }
            });
        }

        /// <summary>
        /// サムネイル画像を作成する
        /// </summary>
        /// <param name="imgPath">元画像のファイルパス</param>
        /// <param name="imgHash">元画像のハッシュ</param>
        private Try<Uri> CreateThumbnailImage(string imgPath, string imgHash)
        {
            return Try(() => {
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

                            return new Uri(savePath);
                        }
                    }
                }
            });
        }
    }
}
