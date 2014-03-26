using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Globalization;
using System.Web.Security;
using ReplicatedSite.Models;
using ReplicatedSite.Services;

namespace ReplicatedSite
{
    public class Identity
    {
        public static CustomerIdentity Customer
        {
            get
            {
                var identity = HttpContext.Current.User.Identity as CustomerIdentity; 
                return identity;
            }
        }
        public static OwnerIdentity Owner
        {
            get
            {
                return (HttpContext.Current.Items["OwnerWebIdentity"] as OwnerIdentity);
            }
        }
    }

    public class OwnerIdentity : IIdentity
    {
        #region Constructors
        public OwnerIdentity()
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

            if (languages == null || languages.Length == 0)
                return "en-US";
            try
            {
                string language = languages[0].Trim();
                return language;
            }

            catch (ArgumentException)
            {
                return "en-US";
            }
        }
        #endregion
    }

    public class CustomerIdentity : IIdentity
    {
        #region Constructors
        public CustomerIdentity(System.Web.Security.FormsAuthenticationTicket ticket)
        {
            Name        = ticket.Name;
            Expires     = ticket.Expiration;

            // Populate this object with the properties
            DeserializeProperties(ticket.UserData);
        }
        public CustomerIdentity()
        {

        }
        #endregion

        #region Identity Settings
        string IIdentity.AuthenticationType
        {
            get { return "Custom"; }
        }
        bool IIdentity.IsAuthenticated
        {
            get { return true; }
        }
        public string Name { get; set; }
        public DateTime Expires { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// These are the properties that will be transferred from the FormsAuthenticationTicket to the Identity. 
        /// If the property is not listed here, it will not be accounted for or auto-populated from the ticket when DeserializeProperties() is invoked.
        /// </summary>
        private List<string> SerializableFields = new List<string>()
        {
            "CustomerID",
            "FirstName",
            "LastName",
            "Company",
            "LoginName",
            "CustomerTypeID",
            "CustomerStatusID",
            "LanguageID",
            "DefaultWarehouseID",
            "CurrencyCode"
        };

        public int CustomerID   { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company   { get; set; }
        public string LoginName { get; set; }
        public int CustomerTypeID { get; set; }
        public int CustomerStatusID { get; set; }
        public int LanguageID { get; set; }
        public int DefaultWarehouseID { get; set; }
        public string CurrencyCode { get; set; }

        // Easy-access Properties
        public string FullName 
        {
            get { return this.Company.IfNullOrEmpty(this.FirstName + " " + this.LastName); }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Saves any changes made to the current identity by re-serializing it and storing it to the authentication cookie.
        /// </summary>
        public void Save()
        {
            var service = new IdentityAuthenticationService();
            service.CreateFormsAuthenticationTicket(this);
        }

        /// <summary>
        /// Refreshes the current identity by fetching a fresh identity and saving it to the autnehtication cookie.
        /// </summary>
        public void Refresh()
        {
            var service = new IdentityAuthenticationService();

            var identity = service.GetIdentity(this.CustomerID);
            service.CreateFormsAuthenticationTicket(identity);
        }
        #endregion

        #region Serialization
        public string SerializeProperties()
        {
            // Get the string format
            var formatter = string.Empty;
            for(var i = 0; i < SerializableFields.Count; i++)
            {
                if(!string.IsNullOrEmpty(formatter)) formatter += "|";
                formatter += "{" + i + "}";
            }

            // Get the field data using reflection
            var fieldData = new List<object>();
            var type = typeof(CustomerIdentity);

            foreach(var field in SerializableFields)
            {
                foreach(var property in type.GetProperties())
                {
                    if(property.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase))
                    {
                        fieldData.Add(property.GetValue(this));
                        break;
                    }
                }
            }

            // Return the formatted data
            return string.Format(formatter, fieldData.ToArray());
        }
        public void DeserializeProperties(string data)
        {
            var counter = 0;
            var dataArray = data.Split('|');


            // Re-populate this object using reflection
            var type = typeof(CustomerIdentity);
            foreach(var field in SerializableFields)
            {
                foreach(var property in type.GetProperties())
                {
                    if(property.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase))
                    {
                        property.SetValue(this, Convert.ChangeType(dataArray[counter], property.PropertyType));
                        counter++;
                        break;
                    }
                }
            }
        }

        public static CustomerIdentity Deserialize(string data)
        {
            try
            {
                var ticket = FormsAuthentication.Decrypt(data);
                return new CustomerIdentity(ticket);
            }
            catch
            {
                var service = new IdentityAuthenticationService();
                service.SignOut();
                return null;
            }
        }
        #endregion
    }
}