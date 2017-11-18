using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Rialto.Models
{
    public class BitmapImageAsyncFactory
    {
        private static readonly Logger logger = LogManager.GetLogger("fileLogger");

        public static Task<Try<BitmapImage>> Create(string filePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var stream = new ScapegoatStream(new MemoryStream(File.ReadAllBytes(filePath))))
                    {
                        var tmpBitmapImage = new BitmapImage();
                        tmpBitmapImage.BeginInit();
                        tmpBitmapImage.StreamSource = stream;
                        tmpBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        tmpBitmapImage.EndInit();
                        tmpBitmapImage.Freeze();
                        return Try(tmpBitmapImage);
                    }
                } catch (Exception e)
                {
                    logger.Error(e);
                    return Try<BitmapImage>(e);
                }
            });
        }

        public class ScapegoatStream : Stream
        {
            private Stream target;
            public ScapegoatStream(Stream target)
            {
                this.target = target;
            }

            public override bool CanRead => target.CanRead;
            public override bool CanSeek => target.CanSeek;
            public override bool CanWrite => target.CanWrite;
            public override long Length => target.Length;
            public override long Position { get => target.Position; set => target.Position = value; }

            public override void Flush()
            {
                target.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return target.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return target.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                target.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                target.Write(buffer, offset, count);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    target.Dispose();
                    target = null;
                }
                base.Dispose(disposing);
            }
        }
    }
}
