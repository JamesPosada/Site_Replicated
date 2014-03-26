using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReplicatedSite.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult WebAliasRequired()
        {
            return View();
        }
    }
}
