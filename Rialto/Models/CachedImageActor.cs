using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Rialto.Models
{
    public class CachedImageActor : ReceiveActor
    {
        public class LoadImageMessage
        {
            public Uri SourceImageFilePath { get; }
            public LoadImageMessage(Uri sourceImageFilePath)
            {
                SourceImageFilePath = sourceImageFilePath;
            }
        }

        private int MAX_CACHE_ITEM_COUNT = 50;
        private IList<KeyValuePair<Uri, Task<Try<BitmapImage>>>> imageLoadTaskCache = new List<KeyValuePair<Uri, Task<Try<BitmapImage>>>>();

        public CachedImageActor()
        {
            Receive<LoadImageMessage>((message) =>
            {
                Sender.Tell(LoadBitmapImage(message.SourceImageFilePath));
            });
        }

        private Task<Try<BitmapImage>> LoadBitmapImage(Uri filePath)
        {
            return imageLoadTaskCache.Find(x => x.Key.Equals(filePath))
                .Map(x =>
                {
                    imageLoadTaskCache.Remove(x);
                    imageLoadTaskCache.Add(x);
                    return x.Value;
                }).IfNone(() =>
                {
                    var loadTask = BitmapImageAsyncFactory.Create(filePath.AbsolutePath);

                    // キャッシュの最大数を超えた場合は一番古いものから削除
                    if (imageLoadTaskCache.Count >= MAX_CACHE_ITEM_COUNT)
                    {
                        imageLoadTaskCache.HeadOrNone()
                            .ForEach(head => imageLoadTaskCache.Remove(head));
                    }
                    imageLoadTaskCache.Add(new KeyValuePair<Uri, Task<Try<BitmapImage>>>(filePath, loadTask));
                    return loadTask;
                });
        }
    }
}
