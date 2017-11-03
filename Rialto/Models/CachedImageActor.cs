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

        private int MAX_CACHE_ITEM_COUNT = 10;
        private IList<KeyValuePair<Uri, BitmapImage>> imageCache = new List<KeyValuePair<Uri, BitmapImage>>();

        public CachedImageActor()
        {
            Receive<LoadImageMessage>((message) =>
            {
                Sender.Tell(LoadBitmapImage(message.SourceImageFilePath));
            });
        }

        private Try<BitmapImage> LoadBitmapImage(Uri filePath)
        {
            return imageCache.Find(x => x.Key.Equals(filePath))
                .Map(x =>
                {
                    imageCache.Remove(x);
                    imageCache.Add(x);
                    return Try(x.Value);
                }).IfNone(() =>
                {
                    return Try(() => {
                        var tmpBitmapImage = new BitmapImage();
                        tmpBitmapImage.BeginInit();
                        tmpBitmapImage.UriSource = filePath;
                        tmpBitmapImage.EndInit();
                        tmpBitmapImage.Freeze();

                        // キャッシュの最大数を超えた場合は一番古いものから削除
                        if (imageCache.Count >= MAX_CACHE_ITEM_COUNT)
                        {
                            imageCache.HeadOrNone()
                                .ForEach(head => imageCache.Remove(head));
                        }
                        imageCache.Add(new KeyValuePair<Uri, BitmapImage>(filePath, tmpBitmapImage));
                        return tmpBitmapImage;
                    });
                });
        }
    }
}
