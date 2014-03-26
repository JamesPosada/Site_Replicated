using ReplicatedSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ReplicatedSite.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ReplicatedSiteAuthorizeAttribute : AuthorizeAttribute
    {
         public override void OnAuthorization(AuthorizationContext filterContext)
         {
             if (filterContext.HttpContext.Request.IsAuthenticated)
             {
                 var identity = filterContext.HttpContext.User.Identity as Identity;
                 if (identity == null)
                 {
                     base.OnAuthorization(filterContext);
                     return;
                 }
             }
             else
             {
                 base.OnAuthorization(filterContext);
             }
         }
    }
}