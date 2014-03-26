using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReplicatedSite.Controllers;

namespace ReplicatedSite.Filters
{
    public class HandleExigoErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled 
                || !filterContext.HttpContext.IsCustomErrorEnabled 
                || !filterContext.HttpContext.Request.IsAuthenticated)
                return;

            var statusCode = (int) HttpStatusCode.InternalServerError;
            if (filterContext.Exception is HttpException)
            {
                statusCode = filterContext.Exception.As<HttpException>().GetHttpCode();
            }
            else if (filterContext.Exception is UnauthorizedAccessException)
            {
                //to prevent login prompt in IIS
                // which will appear when returning 401.
                statusCode = (int)HttpStatusCode.Forbidden; 
            }

            // Decide which exception to throw, HTML or JSON
            var requestedAction = filterContext.RequestContext.RouteData.Values["action"].ToString();
            var actionReturnTypeName = string.Empty;
            var controllerMethods = filterContext.Controller.GetType().GetMethods();
            foreach(var method in controllerMethods)
            {
                if(method.Name.Equals(requestedAction, StringComparison.InvariantCultureIgnoreCase))
                {
                    actionReturnTypeName = method.ReturnType.Name;
                    break;
                }
            }

            // Determine which type of result to return
            ActionResult result;
            if(filterContext.HttpContext.Request.IsAjaxRequest())
            {
                result = CreateJsonResult(filterContext, statusCode);
            }
            else
            {
                switch(actionReturnTypeName.ToLower())
                {
                    case "actionresult":
                        default: 
                        result = CreateActionResult(filterContext, statusCode); break;
                    case "jsonresult":
                    case "jsonnetresult": 
                        result = CreateJsonResult(filterContext, statusCode); break;
                }
            }
            filterContext.Result = result;

            // Prepare the response code.
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        protected virtual ActionResult CreateActionResult(ExceptionContext filterContext, int statusCode)
        {
            var ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);
            var statusCodeName = ((HttpStatusCode) statusCode).ToString();
            var exceptionName = filterContext.Exception.GetType().Name.Replace("Exception", "");

            var viewName = string.Empty;
            viewName = SelectFirstView(ctx,
                                               "~/Views/Error/{0}.cshtml".FormatWith(exceptionName),
                                               "~/Views/Error/{0}.cshtml".FormatWith(statusCodeName),
                                               "~/Views/Error/General.cshtml");

            var controllerName = "Error";
            var actionName = "Index";
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var result = new ViewResult
                             {
                                 ViewName = viewName,
                                 ViewData = new ViewDataDictionary<HandleErrorInfo>(model)
                             };
            result.ViewBag.StatusCode = statusCode;

            filterContext.RouteData.Values["view"] = viewName.Replace("~/Views/Error/", "").Replace(".cshtml", "");

            return result;
        }
        protected virtual JsonNetResult CreateJsonResult(ExceptionContext filterContext, int statusCode)
        {         
            var ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);
            var statusCodeName = ((HttpStatusCode) statusCode).ToString();
            var exceptionName = filterContext.Exception.GetType().Name.Replace("Exception", "");
            if(string.IsNullOrEmpty(exceptionName)) exceptionName = "UnexpectedError";

            var controllerName = (string) filterContext.RouteData.Values["controller"];
            var actionName = (string) filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            return new JsonNetResult(new {
                success = false,
                exception = filterContext.Exception,
                error = filterContext.Exception.Message,
                statuscode = statusCode,
                statuscodedescription = statusCodeName,
                type = exceptionName,
                data = new ViewDataDictionary<HandleErrorInfo>(model),
                controller = controllerName,
                action = actionName
            });
        }


        protected string SelectFirstView(ControllerContext ctx, params string[] viewNames)
        {
            return viewNames.First(view => ViewExists(ctx, view));
        }

        protected bool ViewExists(ControllerContext ctx, string name)
        {
            var result = ViewEngines.Engines.FindView(ctx, name, null);
            return result.View != null;
        }
    }
}