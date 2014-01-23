using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReplicatedSite.HtmlHelpers
{
    public static class ImageHtmlHelpers
    {
        /// <summary>
        /// Gets the URL of the provided customer's avatar photo. Defaults to the current backoffice owner if no CustomerID is provided.
        /// </summary>
        /// <param name="helper">The UrlHelper object</param>
        /// <param name="CustomerID">The customer ID of the desired avatar URL.</param>
        /// <returns>The avatar photo's URL.</returns>
        public static string Avatar(this UrlHelper helper, int CustomerID = 0)
        {
            if (CustomerID == 0) CustomerID = Identity.Owner.CustomerID;

            return helper.Action("Avatar", "App", new { id = CustomerID });
        }
    }
}