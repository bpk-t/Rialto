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
        public event Action<Option<Try<PagingImage>>> OnChangeSelect;

        /// <summary>
        /// ページ遷移した場合に通知する
        /// </summary>
        public event Action<(int currentPage, int allPage, List<ImageInfo> imgList)> OnChangePage;

        private Option<int> selectImageIndex = None;
        public Task<Option<Try<PagingImage>>> SelectImage(long imgId)
        {
            var message = new ThumbnailImageActor.GetImageMessage(imgId, currentTagId);
            return thumbnailImageActor.Ask<Option<Try<PagingImage>>>(message)
                .Select(x => {
                    OnChangeSelect(x);
                    return x;
                });
        }
        public void UnSelectImage()
        {
            OnChangeSelect(None);
        }

        public Task<Option<Try<PagingImage>>> NextImage(long imgId)
        {
            var message = new ThumbnailImageActor.GetNextImageMessage(imgId, currentTagId);
            return thumbnailImageActor.Ask<Option<Try<PagingImage>>>(message)
                .SelectMany(x =>
                {
                    return x.Match(
                        some => {
                            return Task.FromResult(x);
                        },
                        () => {
                            return GoToNextPage().SelectMany(next =>
                            {
                                var firstMessage = new ThumbnailImageActor.GetFirstImageMessage(currentTagId);
                                return thumbnailImageActor.Ask<Option<Try<PagingImage>>>(firstMessage);
                            });
                        }
                    );
                })
                .Select(x => {
                    OnChangeSelect(x);
                    return x;
                });
        }

        public Task<Option<Try<PagingImage>>> PrevImageImage(long imgId)
        {
            var message = new ThumbnailImageActor.GetPrevImageMessage(imgId, currentTagId);
            return thumbnailImageActor.Ask<Option<Try<PagingImage>>>(message)
                .SelectMany(x =>
                {
                    return x.Match(
                        (some) => {
                            return Task.FromResult(x);
                        },
                        () => {
                            return GetPrevPage().SelectMany(next => {
                                var firstMessage = new ThumbnailImageActor.GetFirstImageMessage(currentTagId);
                                return thumbnailImageActor.Ask<Option<Try<PagingImage>>>(firstMessage);
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

        public Task<(int, int, List<ImageInfo>)> GoToPage(int page)
        {
            currentImageOrder = Order.Desc;
            currentPage = page - 1;
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
    }
}
