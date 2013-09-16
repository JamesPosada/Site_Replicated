using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using ReplicatedSite.Exigo.WebService;
using System.Text;
using System.Data.SqlTypes;
using ReplicatedSite.Models;

namespace ReplicatedSite
{
    public static class GlobalUtilities
    {
        /// <summary>
        /// Sets the value of the string to be the first non-nullable parameter found for the strings provided.
        /// </summary>
        /// <param name="strings"></param>
        /// <returns>The first non-null, non-empty string found.</returns>
        public static string Coalesce(params string[] strings)
        {
            return strings.Where(s => !string.IsNullOrEmpty(s)).FirstOrDefault();
        }

        /// <summary>
        /// Condenses the provided string to the provided max length of characters. If the content is longer than the max length, "..." will be appended to the end.
        /// </summary>
        /// <param name="content">The content to be condensed.</param>
        /// <param name="maxLength">The maximum number of allowable characters.</param>
        /// <returns>The content equal or less than the max length.</returns>
        public static string Condense(string content, int maxLength)
        {

            string contentText = Regex.Replace(content, @"<(.|\n)*?>", string.Empty);
            int length = contentText.Length;
            content = Regex.Match(contentText, @"^.{1," + (maxLength - 1) + @"}\b(?<!\s)").Value;
            if (length > maxLength) content += "...";

            return content;
        }

        /// <summary>
        /// Gets the start date for an autoship with the provided frequency type.
        /// </summary>
        /// <param name="frequency">How often the autoship will run</param>
        /// <returns>The start date for an autoship</returns>
        public static DateTime GetNewAutoOrderStartDate(FrequencyType frequency)
        {
            DateTime autoshipstartDate = new DateTime();
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            switch (frequency)
            {
                case FrequencyType.Weekly: autoshipstartDate = now.AddDays(7); break;
                case FrequencyType.BiWeekly: autoshipstartDate = now.AddDays(14); break;
                case FrequencyType.EveryFourWeeks: autoshipstartDate = now.AddDays(28); break;
                case FrequencyType.Monthly: autoshipstartDate = now.AddMonths(1); break;
                case FrequencyType.BiMonthly: autoshipstartDate = now.AddMonths(2); break;
                case FrequencyType.Quarterly: autoshipstartDate = now.AddMonths(3); break;
                case FrequencyType.SemiYearly: autoshipstartDate = now.AddMonths(6); break;
                case FrequencyType.Yearly: autoshipstartDate = now.AddYears(1); break;
                default: autoshipstartDate = now; break;
            }

            // Ensure we are not returning a day of 29, 30 or 31.
           autoshipstartDate = GetNextAvailableAutoOrderStartDate(autoshipstartDate);

            return autoshipstartDate;
        }

        /// <summary>
        /// Gets the next available date for an autoship starting with the provided date.
        /// </summary>
        /// <param name="date">The original start date</param>
        /// <returns>The nearest available start date for an autoship</returns>
        public static DateTime GetNextAvailableAutoOrderStartDate(DateTime date)
        {
            // Ensure we are not returning a day of 29, 30 or 31.
            if (date.Day > 28)
            {
                date = new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1).Date;
            }
        
            return date;
        }

        /// <summary>
        /// Validates the provided credit card number using a Luhn algorithm.
        /// </summary>
        /// <param name="creditCardNumber">The credit card number to validate.</param>
        /// <returns>The validity of the credit card number. True = valid card, False = invalid card.</returns>
        public static bool ValidateCreditCard(string creditCardNumber)
        {
            const string allowed = "0123456789";
            int i;

            var cleanNumber = new StringBuilder();
            for (i = 0; i < creditCardNumber.Length; i++)
            {
                if (allowed.IndexOf(creditCardNumber.Substring(i, 1)) >= 0)
                    cleanNumber.Append(creditCardNumber.Substring(i, 1));
            }
            if (cleanNumber.Length < 13 || cleanNumber.Length > 16)
                return false;

            for (i = cleanNumber.Length + 1; i <= 16; i++)
                cleanNumber.Insert(0, "0");

            int multiplier, digit, sum, total = 0;
            string number = cleanNumber.ToString();

            for (i = 1; i <= 16; i++)
            {
                multiplier = 1 + (i % 2);
                digit = int.Parse(number.Substring(i - 1, 1));
                sum = digit * multiplier;
                if (sum > 9)
                    sum -= 9;
                total += sum;
            }

            return (total % 10 == 0);
        }

        /// <summary>
        /// Gets the full product image path of the provided product image.
        /// </summary>
        /// <param name="productImage">The name of the product image</param>
        /// <returns>The absolute Uri of the provided product image</returns>
        public static string GetProductImagePath(string productImage)
        {
            if (productImage.Contains("nopic.gif"))
            {
                return "Content/Images/imgProductImagePlaceholder.jpg";
            }
            else
            {
                if (productImage.Contains("http://") || productImage.Contains("https://"))
                {
                    return productImage;
                }
                else
                {
                    return GlobalSettings.Shopping.ProductImagePath + productImage;
                }
            }
        }

        /// <summary>
        /// Attempts to parse the provided object as the provided type. If the parsing is unsuccessful, it will reutrn the provided default value.
        /// </summary>
        /// <typeparam name="T">The type to parse your string to.</typeparam>
        /// <param name="s">The object to parse.</param>
        /// <param name="defaultValue">The value that will be returned if parsing is unsuccessful.</param>
        /// <returns></returns>
        public static T TryParse<T>(object s, object defaultValue)
        {
            try
            {
                return (T)Convert.ChangeType(s, typeof(T));
            }
            catch
            {
                return (T)defaultValue;
            }
        }

        /// <summary>
        /// Returns the client's IP address, or (localhost) if there isn't one.
        /// </summary>
        /// <returns>The cleint's IP address</returns>
        public static string GetClientIP()
        {
            var ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if(ip.Equals("::1", StringComparison.InvariantCultureIgnoreCase)) ip = "127.0.0.1";

            return ip;
        }

        #region Markets
        /// <summary>
        /// Gets the market the website is currently using.
        /// </summary>
        /// <returns>The Market object representing the current market.</returns>
        public static Market GetCurrentMarket()
        {
            // Get the cookie, or create a new one if we didn't have one already.
            // We are relying on the cookie to tell us which market to use.
            var cookie = HttpContext.Current.Request.Cookies[GlobalSettings.Markets.MarketCookieName];
            if(cookie == null)
            {
                cookie = new HttpCookie(GlobalSettings.Markets.MarketCookieName);
                cookie.Expires = DateTime.Now.AddYears(1);
                cookie.Value = GlobalSettings.Markets.AvailableMarkets.Where(c => c.IsDefault == true).First().CookieValue;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            // Get the appropriate market
            var market = GlobalSettings.Markets.AvailableMarkets.Where(c => c.CookieValue == cookie.Value).FirstOrDefault();
            return market;
        }
        #endregion
    }
}