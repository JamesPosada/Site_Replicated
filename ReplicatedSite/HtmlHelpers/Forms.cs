using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReplicatedSite.Services;
using System.Text;

namespace ReplicatedSite
{
    public static class FormHtmlHelpers
    {
        public static IEnumerable<SelectListItem> Months(this HtmlHelper helper)
        {
            return DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Where(m => !string.IsNullOrEmpty(m))
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = ((index + 1) + " - " + monthName).ToString()
                       });
        }
        public static IEnumerable<SelectListItem> ExpirationYears(this HtmlHelper helper, int yearCount = 20)
        {
            var years = new List<SelectListItem>();

            for (var year = DateTime.Now.Year; year <= DateTime.Now.AddYears(yearCount).Year; year++)
            {
                years.Add(new SelectListItem()
                {
                    Text = year.ToString(),
                    Value = year.ToString(),
                    Selected = (year == DateTime.Now.Year)
                });
            }

            return years.AsEnumerable();
        }
        
        public static IEnumerable<SelectListItem> Countries(this HtmlHelper helper, string defaultCountryCode = "US")
        {
            var countryRegionService = new CountryRegionService();
            var response = countryRegionService.GetCountries();

            return response.Select(c => new SelectListItem()
            {
                Text     = c.CountryName,
                Value    = c.CountryCode,
                Selected = c.CountryCode == defaultCountryCode
            });
        }
        public static IEnumerable<SelectListItem> Regions(this HtmlHelper helper, string countryCode, string defaultRegionCode = "")
        {
            var countryRegionService = new CountryRegionService();
            var response = countryRegionService.GetRegions(countryCode);

            return response.Select(c => new SelectListItem()
            {
                Text     = c.RegionName,
                Value    = c.RegionCode,
                Selected = c.RegionCode == defaultRegionCode
            });
        }

        public static MvcHtmlString CountryOptions(this HtmlHelper helper, string defaultCountryCode = "US")
        {
            var countryRegionService = new CountryRegionService();
            var response = countryRegionService.GetCountries();

            var html = new StringBuilder();
            foreach (var country in response)
            {
                html.AppendFormat("<option value='{0}' {2}>{1}</option>"
                    , country.CountryCode
                    , country.CountryName
                    , country.CountryCode.Equals(defaultCountryCode, StringComparison.InvariantCultureIgnoreCase) ? "selected" : "");
            }

            return new MvcHtmlString(html.ToString());
        }
        public static MvcHtmlString RegionOptions(this HtmlHelper helper, string countryCode, string defaultRegionCode = "")
        {
            var countryRegionService = new CountryRegionService();
            var response = countryRegionService.GetRegions(countryCode);

            if (response.Count > 1)
            {
                response = response.Where(c => !c.RegionCode.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            var html = new StringBuilder();
            foreach (var region in response)
            {
                html.AppendFormat("<option value='{0}' {2}>{1}</option>"
                    , region.RegionCode
                    , region.RegionName
                    , region.RegionCode.Equals(defaultRegionCode, StringComparison.InvariantCultureIgnoreCase) ? "selected" : "");
            }

            return new MvcHtmlString(html.ToString());
        }
    }
}