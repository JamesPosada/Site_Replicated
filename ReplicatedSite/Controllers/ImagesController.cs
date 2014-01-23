using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ReplicatedSite.Controllers
{
    public class ImagesController : Controller
    {
        [HttpGet]
        public FileStreamResult Image1(bool cache = true)
        {
            var url = string.Format("http://api.exigo.com/4.0/{0}/customerimages/{1}{2}", 
                    GlobalSettings.ExigoApiCredentials.CompanyKey, 
                    Identity.Owner.WebAlias,
                    (!cache) ? "?s=" + Guid.NewGuid().ToString().ToLower() : string.Empty);

            return Website(url);
        }
        [HttpGet]
        public FileStreamResult Image2(bool cache = true)
        {
            var url = string.Format("http://api.exigo.com/4.0/{0}/customerimages/{1}/2{2}", 
                    GlobalSettings.ExigoApiCredentials.CompanyKey,
                    Identity.Owner.WebAlias,
                    (!cache) ? "?s=" + Guid.NewGuid().ToString().ToLower() : string.Empty);

            return Website(url);
        }

        private FileStreamResult Website(string url)
        {
            var defaultAvatarAsBase64 = GlobalSettings.CustomerImages.DefaultLargeAvatarAsBase64;

            var imageStream = new MemoryStream();
            try
            {
                var imageUrl = url;
                var request  = (HttpWebRequest)WebRequest.Create(imageUrl);
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK) throw new Exception("GOTO_CATCH");
                using (var stream = response.GetResponseStream())
                {
                    using (var tempStream = new MemoryStream())
                    {
                        stream.CopyTo(tempStream);
                        imageStream = new MemoryStream(tempStream.ToArray());
                    }
                }
            }
            catch
            {
                var bytes   = Convert.FromBase64String(defaultAvatarAsBase64);
                imageStream = new MemoryStream(bytes);
            }

            var result = new FileStreamResult(imageStream, "image/jpeg");

            result.FileDownloadName = string.Format("{0}.jpg",
                Identity.Owner.WebAlias);
            
            return result;
        }
    }
}

public enum WebsiteImageType
{
    Primary,
    Secondary
}