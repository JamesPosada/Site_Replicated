using ReplicatedSite.Filters;
using ReplicatedSite.Models;
using ReplicatedSite.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace ReplicatedSite.Controllers
{
    public class AppController : Controller
    {
        public ActionResult Default()
        {
            return View();
        }

        [OutputCache(Duration = 86400)]
        public JsonNetResult GetCountries()
        {
            var service = new CountryRegionService();
            var countries = service.GetCountries();

            return new JsonNetResult(new
            {
                success = true,
                countries = countries
            });
        }

        [OutputCache(VaryByParam = "id", Duration = 86400)]
        public JsonNetResult GetRegions(string id)
        {
            var service = new CountryRegionService();
            var regions = service.GetRegions(id);

            return new JsonNetResult(new {
                success = true,
                regions = regions
            });
        }

        [OutputCache(VaryByParam = "id", Duration = 86400)]
        public FileResult Avatar(int id)
        {
            var bytes = new byte[0];

            // Try to return the image found at the avatar path
            bytes = GlobalUtilities.GetImageBytes(GlobalUtilities.GetCustomerAvatarUrl(id));


            // If we didn't find anything there, convert the default image (which is Base64) to a byte array.
            // We'll use that instead
            if (bytes == null || bytes.Length == 0)
            {
                bytes = Convert.FromBase64String("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");
            }

            
            // Return the image
            return File(bytes, "application/png", "{0}.png".FormatWith(id));
        }
    }
}
