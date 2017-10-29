using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

        private Dictionary<Uri, BitmapImage> imageCache = new Dictionary<Uri, BitmapImage>();
        public CachedImageActor()
        {
            Receive<LoadImageMessage>((message) =>
            {
                Sender.Tell(LoadBitmapImage(message.SourceImageFilePath));
            });
        }

        private BitmapImage LoadBitmapImage(Uri filePath)
        {
            if (imageCache.TryGetValue(filePath, out var image))
            {
                return image;
            } else
            {
                var tmpBitmapImage = new BitmapImage();
                tmpBitmapImage.BeginInit();
                tmpBitmapImage.UriSource = filePath;
                tmpBitmapImage.EndInit();
                tmpBitmapImage.Freeze();

                imageCache.Add(filePath, tmpBitmapImage);
                return tmpBitmapImage;
            }
        }
    }
}
