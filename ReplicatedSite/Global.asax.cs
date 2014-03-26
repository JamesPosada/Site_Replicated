using ReplicatedSite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ReplicatedSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            this.BeginRequest += new EventHandler(Application_BeginRequest);
            base.Init();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Get the route data
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            var url = HttpContext.Current.Request.RawUrl;
            if (routeData.Values.Count == 1 && routeData.Values["requestId"] != null) return;


            // Determine if we need to do any logic here.
            // If we have an identity and the current identity matches the web alias in the routes, stop here.
            var identity = HttpContext.Current.Items["OwnerWebIdentity"] as OwnerIdentity;
            if(identity != null && identity.WebAlias.Equals(routeData.Values["webalias"].ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }


            // Register the current web alias 
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current))));            


            // Determine some web alias data
            var defaultWebAlias      = GlobalSettings.ReplicatedSite.DefaultWebAlias;
            var lastWebAlias         = GetLastWebAlias();
            var defaultPage          = urlHelper.Action(routeData.Values["action"].ToString(), routeData.Values["controller"].ToString(), new { webalias = lastWebAlias });
            var webAliasRequiredPage = urlHelper.Action("webaliasrequired", "error", new { webalias = defaultWebAlias });


            // Determine which URL's should be ignored
            var routesToIgnore = new List<string>()
            {
                webAliasRequiredPage
            };
            if(routesToIgnore.Contains(routeData.Route.ToString(), StringComparer.InvariantCultureIgnoreCase))
            {
                return;
            }


            // If we don't have a web alias or page at all, redirect to the default page of the corporate site.
            if(routeData == null)
            {
                HttpContext.Current.Response.Redirect(defaultPage);
            }

            if (routeData.Values["webalias"] == null) return;
            var currentWebAlias     = routeData.Values["webalias"].ToString();


            // If we are an orphan and we don't allow them, redirect to a capture page.
            if(!GlobalSettings.ReplicatedSite.AllowOrphans && currentWebAlias.Equals(defaultWebAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                HttpContext.Current.Response.Redirect(webAliasRequiredPage);
            }


            // If we are an orphan, try to redirect the user back to a previously-visited replicated site
            if(currentWebAlias.Equals(defaultWebAlias, StringComparison.InvariantCultureIgnoreCase) && !currentWebAlias.Equals(lastWebAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                HttpContext.Current.Response.Redirect(defaultPage);
            }


            // Attempt to authenticate the web alias,
            HttpContext.Current.Items["OwnerWebIdentity"] = IdentityAuthenticationService.GetIdentity(currentWebAlias);
            if (HttpContext.Current.Items["OwnerWebIdentity"] != null)
            {
                if(GlobalSettings.ReplicatedSite.RememberLastWebAliasVisited)
                {
                    SetLastWebAliasCookie(currentWebAlias);
                }
                else
                {
                    DeleteLastWebAliasCookie();
                }
            }
            else
            {
                HttpContext.Current.Response.Redirect(webAliasRequiredPage);
            }
        }

        #region Cookies
        private string lastWebAliasCookieName = "LastWebAlias";
        private HttpCookie GetLastWebAliasCookie()
        {
            var cookie = Request.Cookies[lastWebAliasCookieName];
            if(cookie == null)
            {
                cookie = new HttpCookie(lastWebAliasCookieName);
            }

            return cookie;
        }
        private string GetLastWebAlias()
        {
            var cookie = Request.Cookies[lastWebAliasCookieName];
            if(cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return GlobalSettings.ReplicatedSite.DefaultWebAlias;
            }

            return cookie.Value;
        }
        private void SetLastWebAliasCookie(string webAlias)
        {
            if(webAlias.ToLower() != GlobalSettings.ReplicatedSite.DefaultWebAlias.ToLower())
            {
                var cookie     = GetLastWebAliasCookie();
                cookie.Value   = webAlias;
                cookie.Expires = DateTime.Now.AddYears(1);

                Response.Cookies.Add(cookie);
            }
        }
        private void DeleteLastWebAliasCookie()
        {
            var cookie = Request.Cookies[lastWebAliasCookieName];
            if(cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
        }
        #endregion
    }
}