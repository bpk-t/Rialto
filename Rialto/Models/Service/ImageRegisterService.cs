using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangExt;
using Akka.Actor;

namespace Rialto.Models.Service
{
    public class ImageRegisterService
    {
        public void RegisterImages(IList<Uri> imageUris, Option<long> tagId)
        {

        }

        class ImageRegisterActor : ReceiveActor
        {
            public class RegisterImages
            {
                public IList<Uri> ImageUris { get; }
                public Option<long> TagId { get; }
                public RegisterImages(IList<Uri> imageUris, Option<long> tagId)
                {
                    ImageUris = imageUris;
                    TagId = tagId;
                }
            }
            public ImageRegisterActor()
            {
                Receive<RegisterImages>((message) =>
                {
                    // TODO 登録結果を返す
                    //Sender.Tell();
                });
            }
        }
    }
}
