using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using ReplicatedSite.Exigo.WebService;
using ReplicatedSite.Models;

namespace ReplicatedSite.Services
{
    public class IdentityAuthenticationService
    {
        public static Identity GetIdentity(string webAlias)
        {
            var key = webAlias.ToUpper();

            var identity = HttpContext.Current.Cache[key] as Identity;

            if(identity == null)
            {
                try
                {
                    // Replicated SQL
                    var context = ExigoApiContext.CreateSqlDapperContext();
                    identity = context.Query<Identity>(@"
                            SELECT
                                cs.CustomerID,
                                c.CustomerTypeID,
                                c.CustomerStatusID,
                                HighestAchievedRankID = c.RankID,
                                c.CreatedDate,
                                WarehouseID = COALESCE(c.DefaultWarehouseID, @defaultwarehouseid),

                                cs.WebAlias,
                                cs.FirstName,
                                cs.LastName,
                                cs.Company,
                                cs.Email,
                                cs.Phone,
                                cs.Phone2,
                                cs.Fax,

                                cs.Address1,
                                cs.Address2,
                                cs.City,
                                cs.State,
                                cs.Zip,
                                cs.Country,

                                cs.Notes1,
                                cs.Notes2,
                                cs.Notes3,
                                cs.Notes4

                            FROM CustomerSites cs
                                INNER JOIN Customers c
                                    ON c.CustomerID = cs.CustomerID
                            WHERE cs.webalias = @webalias
                        ", new { 
                             webalias = key,
                             defaultwarehouseid = Warehouses.Default
                         }).FirstOrDefault();
                    context.Close();

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
    }
}