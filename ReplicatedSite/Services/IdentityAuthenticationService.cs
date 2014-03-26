using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using ReplicatedSite.Exigo.WebService;
using ReplicatedSite.Models;
using ReplicatedSite.Exigo.OData;

namespace ReplicatedSite.Services
{
    public class IdentityAuthenticationService
    {
        // Owner Identities
        public static OwnerIdentity GetIdentity(string webAlias)
        {
            var key = webAlias.ToUpper();

            var identity = HttpContext.Current.Cache["OwnerIdentity-" + key] as OwnerIdentity;

            if (identity == null)
            {
                try
                {
                    var customer = ExigoApiContext.CreateWebServiceContext().GetCustomerSite(new GetCustomerSiteRequest
                    {
                        WebAlias = key
                    });

                    identity = new Identity
                    {
                        CustomerID = customer.CustomerID,
                        WebAlias = customer.WebAlias,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Company = customer.Company,
                        Email = customer.Email,
                        DaytimePhone = customer.Phone,
                        EveningPhone = customer.Phone2,

                        Notes1 = customer.Notes1,
                        Notes2 = customer.Notes2,
                        Notes3 = customer.Notes3,
                        Notes4 = customer.Notes4
                    };

//                    // Replicated SQL
//                    var context = ExigoApiFactory.CreateSqlDapperContext();

//                    identity = context.Query<OwnerIdentity>(@"
//                            SELECT
//                                cs.CustomerID,
//                                c.CustomerTypeID,
//                                c.CustomerStatusID,
//                                HighestAchievedRankID = c.RankID,
//                                c.CreatedDate,
//                                WarehouseID = COALESCE(c.DefaultWarehouseID, @defaultwarehouseid),
//
//                                cs.WebAlias,
//                                cs.FirstName,
//                                cs.LastName,
//                                cs.Company,
//                                cs.Email,
//                                cs.Phone,
//                                cs.Phone2,
//                                cs.Fax,
//
//                                cs.Address1,
//                                cs.Address2,
//                                cs.City,
//                                cs.State,
//                                cs.Zip,
//                                cs.Country,
//
//                                cs.Notes1,
//                                cs.Notes2,
//                                cs.Notes3,
//                                cs.Notes4
//
//                            FROM CustomerSites cs
//                                INNER JOIN Customers c
//                                    ON c.CustomerID = cs.CustomerID
//                            WHERE cs.webalias = @webalias
//                        ", new
//                         {
//                             webalias = key,
//                             defaultwarehouseid = Warehouses.Default
//                         }).FirstOrDefault();
//                    context.Close();

                    // Save the identity
                    HttpContext.Current.Cache.Insert(key,
                        identity,
                        null,
                        DateTime.Now.AddMinutes(GlobalSettings.ReplicatedSite.IdentityRefreshInterval),
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.Normal,
                        null);




                    // Web Service (for non-Enterprize clients only)
                    /*var customer = ExigoApiContext.CreateWebServiceContext().GetCustomerSite(new GetCustomerSiteRequest
                    {
                        WebAlias = key
                    });
                     
                    identity = new Identity
                    {
                        CustomerID   = customer.CustomerID,
                        WebAlias     = customer.WebAlias,
                        FirstName    = customer.FirstName,
                        LastName     = customer.LastName,
                        Company      = customer.Company,
                        Email        = customer.Email,
                        DaytimePhone = customer.Phone,
                        EveningPhone = customer.Phone2,

                        Notes1 = customer.Notes1,
                        Notes2 = customer.Notes2,
                        Notes3 = customer.Notes3,
                        Notes4 = customer.Notes4
                    };

                     
                    HttpContext.Current.Cache.Insert(key,
                        identity,
                        null,
                        DateTime.Now.AddMinutes(15),
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.Normal,
                        null);
                    */
                }
                catch
                {
                    return null;
                }
            }

            return identity;
        }


        // Customer Identities
        /// <summary>
        /// Signs the customer into the backoffice.
        /// </summary>
        /// <param name="loginName">The customer's login name</param>
        /// <param name="password">The customer's password</param>
        /// <returns>Whether or not the customer was successfully signed in.</returns>
        public bool SignIn(string loginName, string password)
        {
            // Attempt to authenticate the customer using OData
            var customer = (from c in ExigoApiFactory.CreateODataContext().CreateQuery<Customer>("AuthenticateLogin")
                    .AddQueryOption("loginName", "'" + loginName + "'")
                    .AddQueryOption("password", "'" + password + "'")
                            select new Customer { CustomerID = c.CustomerID }).FirstOrDefault();

            // If we could not authenticate the user, the customerID will still be 0. Stop here if it is.
            if (customer == null) return false;


            // If we got here, we are authorized. Let's attempt to get the identity.
            var identity = GetIdentity(customer.CustomerID);


            // If identity is still null, we couldn't fetch the customer's details. Stop here.
            if (identity == null) return false;


            // Add the customer ID to the items in case we need this in the same request later on.
            // We need this because we don't have access to the Identity.Current in this same request later on.
            HttpContext.Current.Items.Add("CustomerID", customer.CustomerID);

            // Looks like we got the identity. Let's create the ticket.
            return CreateFormsAuthenticationTicket(identity);
        }

        /// <summary>
        /// Signs the user out of the backoffice
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Fetches the identity by creating a new one and populating it from the database.
        /// </summary>
        /// <param name="customerID">The customer ID of the desired identity</param>
        /// <returns>A fresh UserIdentity object.</returns>
        public CustomerIdentity GetIdentity(int customerID)
        {
            // Create the identity
            CustomerIdentity identity = null;


            // Get the customer
            var customer = ExigoApiFactory.CreateODataContext().Customers
                .Where(c => c.CustomerID == customerID)
                .FirstOrDefault();

            if (customer == null) return identity;


            // Map the customer to the identity
            identity = new CustomerIdentity()
            {
                CustomerID = customer.CustomerID,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Company = customer.Company,
                LoginName = customer.LoginName,
                CustomerTypeID = customer.CustomerTypeID,
                CustomerStatusID = customer.CustomerStatusID,
                LanguageID = customer.LanguageID,
                DefaultWarehouseID = customer.DefaultWarehouseID,
                CurrencyCode = customer.CurrencyCode
            };


            // Return the identity
            return identity;
        }

        /// <summary>
        /// Creates the forms authentication ticket
        /// </summary>
        /// <param name="customerID">The customer ID</param>
        /// <returns>Whether or not the ticket was created successfully.</returns>
        public bool CreateFormsAuthenticationTicket(CustomerIdentity identity)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                identity.CustomerID.ToString(),
                DateTime.Now,
                DateTime.Now.AddMinutes(GlobalSettings.BackofficeSettings.SessionTimeoutInMinutes),
                false,
                identity.SerializeProperties());

            // encrypt the ticket
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // create the cookie.
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName]; //saved user
            if (cookie == null)
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            }
            else
            {
                cookie.Value = encTicket;
                HttpContext.Current.Response.Cookies.Set(cookie);
            }

            return true;
        }
    }
}