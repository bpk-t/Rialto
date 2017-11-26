﻿using Akka.Actor;
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
using System.Data.Common;
using NLog;
using System.Threading;

namespace Rialto.Models
{
    public class ThumbnailPagingActor : ReceiveActor
    {
        private static readonly Logger logger = LogManager.GetLogger("fileLogger");

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
        private IActorRef thumbnailImageActor;
        private long cacheTagId;
        private long cachePage;
        private long cacheLimit;
        private Order cacheImageOrder;
        private Option<long> cacheSelectedImageId = None;
        private List<ImageInfo> cacheImages = new List<ImageInfo>();

        public ThumbnailPagingActor()
        {
            cachedImageActor = Context.ActorOf<CachedImageActor>(nameof(CachedImageActor));
            thumbnailImageActor = Context.ActorOf<ThumbnailImageActor>(nameof(ThumbnailImageActor));
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
                        return LoadBitmapImage(img.SourceImageFilePath).Select(loadImageTry =>
                        {
                            var pagingImage = new PagingImage(
                                                        allCount: imgCount,
                                                        image: loadImageTry,
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

        private Task<Try<BitmapImage>> LoadBitmapImage(Uri filePath)
        {
            var message = new CachedImageActor.LoadImageMessage(filePath);
            return cachedImageActor.Ask<Task<Try<BitmapImage>>>(message).SelectMany(x => x);
        }

        private CancellationTokenSource cancelTokenSource;
        private Task<PagingInfo> GetThumbnailImage(long tagId, long page, long limit, Order imageOrder)
        {
            return _GetThumbnailImage(tagId, page, limit, imageOrder).Select(x => 
            {
                // 先読み
                if (cancelTokenSource != null)
                {
                    cancelTokenSource.Cancel();
                }
                cancelTokenSource = new CancellationTokenSource();
                var token = cancelTokenSource.Token;

                Task.Run(async () => {
                    foreach (var img in x.ThumbnailImageList)
                    {
                        token.ThrowIfCancellationRequested();

                        var message = new CachedImageActor.LoadImageMessage(img.SourceImageFilePath);

                        await cachedImageActor.Ask(message);
                        await Task.Delay(TimeSpan.FromMilliseconds(1500));                        
                    }
                }, token);
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

                var message = new ThumbnailImageActor.GetImageMessage(sourceImagePath, img.Fold("", (a, x) => x.Md5Hash));
                var imageLoadTask = thumbnailImageActor.Ask<Task<Try<BitmapImage>>>(message);
                result.SetImage(imageLoadTask.SelectMany(x => x));
                return result;
            }

            (Task<long>, Task<IEnumerable<(Option<RegisterImage>, Option<ImageRepository>)>>) GetAllCountAndList(DbConnection connection, long offset)
            {
                if (tagId == TagConstant.ALL_TAG_ID)
                {
                    var countTask = RegisterImageRepository.GetAllCountAsync(connection);
                    var getListTask = RegisterImageRepository.GetAllAsync(connection, Some(offset), Some(limit), imageOrder);
                    return (countTask, getListTask);
                }
                else if (tagId == TagConstant.NOTAG_TAG_ID)
                {
                    var countTask = RegisterImageRepository.GetNoTagCountAsync(connection);
                    var getListTask = RegisterImageRepository.GetNoTagAsync(connection, Some(offset), Some(limit), imageOrder);
                    return (countTask, getListTask);
                }
                else
                {
                    var countTask = RegisterImageRepository.GetByTagCountAsync(connection, tagId);
                    var getListTask = RegisterImageRepository.GetByTagAsync(connection, tagId, Some(offset), Some(limit), imageOrder);
                    return (countTask, getListTask);
                }
            }
            return DBHelper.Instance.Execute((connection, tran) =>
            {
                var offset = page * limit;
                var (countTask, getListTask) = GetAllCountAndList(connection, offset);
                var getRegisterImgTask = getListTask.Select(results =>
                        results.Select(x => RegisterImageToImageInfo(x.Item1, x.Item2)).ToList()
                    );
                var allPage = countTask.Result / limit + (countTask.Result % limit > 0 ? 1 : 0);
                return Task.WhenAll(countTask, getListTask).ContinueWith(nouse => new PagingInfo(page + 1, allPage, getRegisterImgTask.Result));
            });
        }
    }
}