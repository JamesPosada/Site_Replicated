using ReplicatedSite;
using ReplicatedSite.Controllers;
using ReplicatedSite.Services;
using ReplicatedSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Backoffice.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult Addresses()
        {
            return View();
        }

        public ActionResult PaymentOptions()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        #region Signing in
        [AllowAnonymous]
        public ActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonNetResult Login(LoginViewModel model)
        {
            try
            {
                var service = new IdentityAuthenticationService();
                if (service.SignIn(model.LoginName, model.Password))
                {
                    // Get the customer's basic info for the client-side cookies
                    var customer = ExigoApiFactory.CreateODataContext().Customers
                        .Where(c => c.CustomerID == (int)HttpContext.Items["CustomerID"])
                        .Select(c => new
                        {
                            Name = c.FirstName + " " + c.LastName,
                            Email = c.Email
                        })
                        .FirstOrDefault();

                    // Return the response
                    return new JsonNetResult(new
                    {
                        success = true,
                        loginname = model.LoginName,
                        name = customer.Name,
                        email = customer.Email
                    });
                }
                else
                {
                    return new JsonNetResult(new
                    {
                        success = false,
                        model = model,
                        error = "Invalid username/password. Please try again."
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new
                {
                    success = false,
                    model = model,
                    error = ex.Message
                });
            }
        }
        #endregion

        #region Signing Out
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        #endregion
    }
}