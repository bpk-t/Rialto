using Akka.Actor;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Windows.Media.Imaging;

namespace Rialto.Models
{
    public class ThumbnailImageActor : ReceiveActor
    {
        private static readonly Logger logger = LogManager.GetLogger("fileLogger");

        public class GetImageMessage
        {
            public string SourceImagePath { get; }
            public string ImageHash { get; }
            public GetImageMessage(string sourceImagePath, string imageHash)
            {
                SourceImagePath = sourceImagePath;
                ImageHash = imageHash;
            }
        }

        Dictionary<string, Task<Try<BitmapImage>>> cacheMap = new Dictionary<string, Task<LanguageExt.Try<BitmapImage>>>();
        public ThumbnailImageActor()
        {
            Receive<GetImageMessage>(message => {
                if (cacheMap.TryGetValue(message.ImageHash, out var cacheTask))
                {
                    Sender.Tell(cacheTask);
                } else
                {
                    var imageLoadTask = (
                        from thumbnailImageUri in GetThumbnailImage(message.SourceImagePath, message.ImageHash)
                        from bitmapImage in BitmapImageAsyncFactory.Create(thumbnailImageUri.AbsolutePath)
                        select bitmapImage);
                    cacheMap.Add(message.ImageHash, imageLoadTask);
                    Sender.Tell(imageLoadTask);
                }
                cacheMap.Where(kv => kv.Value.IsCompleted).ToList().ForEach(kv => cacheMap.Remove(kv.Key));
            });
        }

        /// <summary>
        /// サムネイル画像のパス情報を返す
        /// </summary>
        /// <param name="imgPath">元画像のファイルパス</param>
        /// <param name="imgHash">元画像のハッシュ</param>
        /// <returns></returns>
        private Task<Uri> GetThumbnailImage(string imgPath, string imgHash)
        {
            Task<string> GetPath(string fileName)
            {
                try
                {
                    if (!Directory.Exists(Properties.Settings.Default.ThumbnailImageDirectory))
                    {
                        Directory.CreateDirectory(Properties.Settings.Default.ThumbnailImageDirectory);
                    }
                    return Task.FromResult(Path.Combine(Properties.Settings.Default.ThumbnailImageDirectory, fileName));
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    return Task.FromException<string>(e);
                }
            }

            return GetPath(imgHash).Bind((string filePath) => {
                if (!File.Exists(filePath))
                {
                    return CreateThumbnailImage(imgPath, imgHash);
                }
                else
                {
                    return Task.FromResult(new Uri(Path.GetFullPath(filePath)));
                }
            });
        }

        /// <summary>
        /// サムネイル画像を作成する
        /// </summary>
        /// <param name="imgPath">元画像のファイルパス</param>
        /// <param name="imgHash">元画像のハッシュ</param>
        private Task<Uri> CreateThumbnailImage(string imgPath, string imgHash)
        {
            return Task.Run(() => {
                try
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

                                return new Uri(savePath);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw e;
                }
            });
        }
    }
}
