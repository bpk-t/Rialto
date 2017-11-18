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
        public Try<BitmapImage> Image { get; }
        public long ImageId { get; }
        public long Index { get; }

        /// <summary>
        /// ページングが発生した場合にSomeになる
        /// </summary>
        public Option<PagingInfo> Page { get; }

        public PagingImage(long allCount, Try<BitmapImage> image, long imageId, long index, Option<PagingInfo> page)
        {
            AllCount = allCount;
            Image = image;
            ImageId = imageId;
            Index = index;
            Page = page;
        }
    }

    public class PagingInfo
    {
        public long CurrentPage { get; }
        public long AllPage { get; }
        public List<ImageInfo> ThumbnailImageList { get; }

        public PagingInfo(long currentPage, long allPage, List<ImageInfo> thumbnailImageList)
        {
            CurrentPage = currentPage;
            AllPage = allPage;
            ThumbnailImageList = thumbnailImageList;
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
        public int OnePageItemCount { get; set; }

        /// <summary>
        /// 選択画像が変更された場合に通知する
        /// </summary>
        public event Action<Option<Try<PagingImage>>> OnChangeSelect;

        /// <summary>
        /// ページ遷移した場合に通知する
        /// </summary>
        public event Action<PagingInfo> OnChangePage;

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

        public Task<Option<Try<PagingImage>>> NextImage()
        {
            var message = new ThumbnailImageActor.GetNextImageMessage();
            return thumbnailImageActor.Ask<Option<Try<PagingImage>>>(message)
                .Select(x => {
                    x.IfSome(y => y.IfSucc(pagingImg => pagingImg.Page.IfSome(page => OnChangePage(page))));
                    OnChangeSelect(x);
                    return x;
                });
        }

        public Task<Option<Try<PagingImage>>> PrevImageImage()
        {
            var message = new ThumbnailImageActor.GetPrevImageMessage();
            return thumbnailImageActor.Ask<Option<Try<PagingImage>>>(message)
                .Select(x => {
                    x.IfSome(y => y.IfSucc(pagingImg => pagingImg.Page.IfSome(page => OnChangePage(page))));
                    OnChangeSelect(x);
                    return x;
                });
        }

        /// <summary>
        /// 1ページの表示件数を変更
        /// </summary>
        /// <param name="onePageItemCount">1ページの表示件数</param>
        /// <returns></returns>
        public Task<PagingInfo> SetOnePageItemCountAndRefresh(int onePageItemCount)
        {
            OnePageItemCount = onePageItemCount;
            return GoToPage();
        }

        public Task<PagingInfo> GoToNextPage()
        {
            var message = new ThumbnailImageActor.GoToNextPageMessage();
            return thumbnailImageActor.Ask<PagingInfo>(message)
                .Select(x => {
                    OnChangePage(x);
                    return x;
                });
        }

        public Task<PagingInfo> GetPrevPage()
        {
            var message = new ThumbnailImageActor.GoToPrevPageMessage();
            return thumbnailImageActor.Ask<PagingInfo>(message)
                .Select(x => {
                    OnChangePage(x);
                    return x;
                });
        }

        public Task<PagingInfo> GetFirstPage(long tagId)
        {
            currentTagId = tagId;
            currentImageOrder = Order.Desc;
            return GoToPage();
        }

        public Task<PagingInfo> Reverse()
        {
            currentImageOrder = Order.Asc;
            return GoToPage();
        }

        public Task<PagingInfo> GoToPage(int page = 0)
        {
            var message = new ThumbnailImageActor.GotToPageMessage(currentTagId, page, OnePageItemCount, currentImageOrder);
            return thumbnailImageActor.Ask<PagingInfo>(message)
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
