using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Globalization;
using System.Web.Security;
using ReplicatedSite.Models;

namespace ReplicatedSite
{
    public class Identity : IIdentity
    {
        #region Singleton Instances
        public static Identity Current
        {
            get
            {
                return (HttpContext.Current.Items["WebIdentity"] as Identity);
            }
        }
        #endregion

        #region Constructors
        public Identity()
        {
        }
        #endregion

        #region Settings
        string IIdentity.AuthenticationType
        {
            get { return "Custom"; }
        }
        bool IIdentity.IsAuthenticated
        {
            get { return true; }
        }
        public string Name { get; set; }
        #endregion

        #region Properties
        public int CustomerID { get; set; }
        public int CustomerTypeID { get; set; }
        public int CustomerStatusID { get; set; }
        public int WarehouseID { get; set; }
        public int HighestAchievedRankID { get; set; }
        public DateTime CreatedDate { get; set; }

        public string WebAlias { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        public string Notes1 { get; set; }
        public string Notes2 { get; set; }
        public string Notes3 { get; set; }
        public string Notes4 { get; set; }       

        public string DisplayName
        {
            get { return GlobalUtilities.Coalesce(this.Company, this.FirstName + " " + this.LastName); }
        }

        public Market Market
        {
            get { return GlobalUtilities.GetCurrentMarket(); }
        }
        #endregion

        #region Private Methods
        private string GetBrowsersDefaultCultureCode()
        {
            string[] languages = HttpContext.Current.Request.UserLanguages;

            if(languages == null || languages.Length == 0)
                return "en-US";
            try
            {
                string language = languages[0].Trim();
                return language;
            }

            catch(ArgumentException)
            {
                return "en-US";
            }
        }
        #endregion
    }
}