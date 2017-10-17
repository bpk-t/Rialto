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
    public class PagingImage
    {
        public long AllCount { get; }
        public BitmapImage Image { get; }
        public long ImgId { get; }
        public long Index { get; }

        public PagingImage(long allCount, BitmapImage iamge, long imgId, long index)
        {
            AllCount = allCount;
            Image = iamge;
            ImgId = imgId;
            Index = index;
        }
    }

    public class ThumbnailImageService : NotificationObject
    { 
        public ThumbnailImageService(ActorSystem system)
        {
            thumbnailImageActor = system.ActorOf<ThumbnailImageActor>(nameof(ThumbnailImageActor));
            OnChangeSelect += (nouse) => { };
            OnChangePage += (nouse) => { };
        }

        private IActorRef thumbnailImageActor;

        private long currentTagId = TagConstant.ALL_TAG_ID;
        private Order currentImageOrder = Order.Desc;
        private int currentPage = 0;
        public int OnePageItemCount { get; set; }

        /// <summary>
        /// 選択画像が変更された場合に通知する
        /// </summary>
        public event Action<Option<PagingImage>> OnChangeSelect;

        /// <summary>
        /// ページ遷移した場合に通知する
        /// </summary>
        public event Action<(int currentPage, int allPage, List<ImageInfo> imgList)> OnChangePage;

        private Option<int> selectImageIndex = None;
        public Task<Option<PagingImage>> SelectImage(long imgId)
        {
            var message = new ThumbnailImageActor.GetImageMessage(imgId, currentTagId);
            return thumbnailImageActor.Ask<Option<PagingImage>>(message)
                .Select(x => {
                    OnChangeSelect(x);
                    return x;
                });
        }
        public void UnSelectImage()
        {
            OnChangeSelect(None);
        }

        public Task<Option<PagingImage>> NextImage(long imgId)
        {
            var message = new ThumbnailImageActor.GetNextImageMessage(imgId, currentTagId);
            return thumbnailImageActor.Ask<Option<PagingImage>>(message)
                .SelectMany(x =>
                {
                    return x.Match(
                        (some) => {
                            return Task.FromResult(x);
                        },
                        () => {
                            return GoToNextPage().SelectMany(next =>
                            {
                                var firstMessage = new ThumbnailImageActor.GetFirstImageMessage(currentTagId);
                                return thumbnailImageActor.Ask<Option<PagingImage>>(firstMessage);
                            });
                        }
                    );
                })
                .Select(x => {
                    OnChangeSelect(x);
                    return x;
                });
        }

        public Task<Option<PagingImage>> PrevImageImage(long imgId)
        {
            var message = new ThumbnailImageActor.GetPrevImageMessage(imgId, currentTagId);
            return thumbnailImageActor.Ask<Option<PagingImage>>(message)
                .SelectMany(x =>
                {
                    return x.Match(
                        (some) => {
                            return Task.FromResult(x);
                        },
                        () => {
                            return GetPrevPage().SelectMany(next => {
                                var firstMessage = new ThumbnailImageActor.GetFirstImageMessage(currentTagId);
                                return thumbnailImageActor.Ask<Option<PagingImage>>(firstMessage);
                            });
                        }
                    );
                })
                .Select(x => {
                    OnChangeSelect(x);
                    return x;
                });
        }

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
        public Task<(int, int, List<ImageInfo>)> SetOnePageItemCountAndRefresh(int onePageItemCount)
        {
            OnePageItemCount = onePageItemCount;
            return GoToPage();
        }

        public Task<(int, int, List<ImageInfo>)> GoToNextPage()
        {
            return ExistsNextPage().SelectMany(exists =>
            {
                if (exists)
                {
                    currentPage++;
                    return GoToPage();
                } else
                {
                    return GetFirstPage();
                }
            });
        }

        public Task<(int, int, List<ImageInfo>)> GetPrevPage()
        {
            return ExistsPrevPage().SelectMany(exists =>
            {
                if (exists)
                {
                    currentPage--;
                    return GoToPage();
                } else
                {
                    return GetFirstPage();
                }
            });
        }

        public Task<(int, int, List<ImageInfo>)> GetFirstPage()
        {
            return GetFirstPage(currentTagId);
        }

        public Task<(int, int, List<ImageInfo>)> GetFirstPage(long tagId)
        {
            currentPage = 0;
            currentTagId = tagId;
            currentImageOrder = Order.Desc;
            return GoToPage();
        }

        public Task<(int, int, List<ImageInfo>)> Reverse()
        {
            currentImageOrder = Order.Asc;
            currentPage = 0;
            return GoToPage();
        }

        private Task<(int, int, List<ImageInfo>)> GoToPage()
        {
            var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, currentPage * OnePageItemCount, OnePageItemCount, currentImageOrder);
            return thumbnailImageActor.Ask<(long, List<ImageInfo>)>(message)
                .Select(x => {
                    (long allCount, List<ImageInfo> images) = x;
                    return (currentPage + 1, Math.Max((int)(allCount / OnePageItemCount), 1), images);
                })
                .Select(x => {
                    OnChangePage(x);
                    return x;
                });
        }

        public async Task Shuffle()
        {
            await Task.Run(() =>
            {
                // TODO 直す
                //ThumbnailImgList.Clear();
                //ThumbnailImgList.OrderBy(_ => Guid.NewGuid()).ForEach(x => ThumbnailImgList.Add(x));
            });
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

            private long cacheOffset;
            private List<ImageInfo> cacheImages = new List<ImageInfo>();

            public ThumbnailImageActor()
            {
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

            private Option<PagingImage> GetImage(long imgId, long tagId)
            {
                var index = cacheImages.FindIndex(x => x.ImgID == imgId);
                if (index >= 0)
                {
                    var countTask = GetImageCount(tagId);
                    countTask.Wait();

                    return Option<ImageInfo>.Some(cacheImages[index])
                        .Map(img => new PagingImage(countTask.Result, LoadBitmapImage(img.SourceImageFilePath), img.ImgID, cacheOffset + index));
                } else
                {
                    return None;
                }
            }

            private Option<PagingImage> GetNextImage(long imgId, long tagId)
            {
                var index = cacheImages.FindIndex(x => x.ImgID == imgId);
                var nextIndex = index + 1;

                if (index >= 0 && cacheImages.Count > nextIndex)
                {
                    var countTask = GetImageCount(tagId);
                    countTask.Wait();

                    return Option<ImageInfo>.Some(cacheImages[nextIndex])
                        .Map(img => new PagingImage(countTask.Result, LoadBitmapImage(img.SourceImageFilePath), img.ImgID, cacheOffset + nextIndex));
                } else
                {
                    return None;
                }
            }

            private Option<PagingImage> GetPrevImage(long imgId, long tagId)
            {
                var index = cacheImages.FindIndex(x => x.ImgID == imgId);
                var prevIndex = index - 1;

                if (index >= 0 && prevIndex >= 0 && cacheImages.Count > prevIndex)
                {
                    var countTask = GetImageCount(tagId);
                    countTask.Wait();

                    return Option<ImageInfo>.Some(cacheImages[prevIndex])
                        .Map(img => new PagingImage(countTask.Result, LoadBitmapImage(img.SourceImageFilePath), img.ImgID, cacheOffset + prevIndex));
                }
                else
                {
                    return None;
                }
            }

            private BitmapImage LoadBitmapImage(Uri filePath)
            {
                var tmpBitmapImage = new BitmapImage();
                tmpBitmapImage.BeginInit();
                tmpBitmapImage.UriSource = filePath;
                tmpBitmapImage.EndInit();
                tmpBitmapImage.Freeze();
                return tmpBitmapImage;
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
