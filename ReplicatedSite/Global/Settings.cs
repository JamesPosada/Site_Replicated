﻿using ReplicatedSite.Exigo.Api;
using ReplicatedSite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ReplicatedSite
{
    public static class GlobalSettings
    {
        public static class ExigoApiCredentials
        {
            public static string LoginName = "";
            public static string Password = "";
            public static string CompanyKey = "";
        }

        public static class ExigoPaymentApiCredentials
        {
          
        }

        public static class Encryptions
        {
            public static class General
            {
              
            }
        }

        public static class SendGridCredentials
        {
          
        }

        public static class ExigoApiSettings
        {
            public static ExigoApiContextSource DefaultContextSource = ExigoApiContextSource.Live;

            public static class WebService
            {
                public static string LiveUrl = "http://api.exigo.com/3.0/ExigoApi.asmx";
                public static string Sandbox1Url = "http://sandboxapi1.exigo.com/3.0/ExigoApi.asmx";
                public static string Sandbox2Url = "http://sandboxapi2.exigo.com/3.0/ExigoApi.asmx";
                public static string Sandbox3Url = "http://sandboxapi3.exigo.com/3.0/ExigoApi.asmx";
            }

            public static class OData
            {
                public static string LiveUrl = "http://api.exigo.com/4.0/" + GlobalSettings.ExigoApiCredentials.CompanyKey;
                public static string Sandbox1Url = "http://sandboxapi1.exigo.com/4.0/" + GlobalSettings.ExigoApiCredentials.CompanyKey;
                public static string Sandbox2Url = "http://sandboxapi2.exigo.com/4.0/" + GlobalSettings.ExigoApiCredentials.CompanyKey;
                public static string Sandbox3Url = "http://sandboxapi3.exigo.com/4.0/" + GlobalSettings.ExigoApiCredentials.CompanyKey;
            }
        }

        public static class ConnectionStrings
        {
            public static string Sql = ConfigurationManager.ConnectionStrings["sqlreporting"].ConnectionString;
        }


        //Config: Need to Configure For Enrolling under ID 5
        public static class ReplicatedSite
        {
            public static string DefaultWebAlias = "frezzor";
            public static string DefaultPage = "default";
            public static bool RememberLastWebAliasVisited = true;
            public static bool AllowOrphans = true;
            public static int IdentityRefreshInterval = 15; // In minutes
        }

        public static class BackofficeSettings
        {
            public static int SessionTimeoutInMinutes = 30;
        }


        //Config: Add New Markets
        public static class Markets
        {
            public static string MarketCookieName = "SelectedMarket";
            public static List<Market> AvailableMarkets = new List<Market>
                                                                        {
                                                                            new Market { 
                                                                                Name            = MarketName.UnitedStates,
                                                                                Description     = "United States",     
                                                                                CookieValue     = "US",         
                                                                                CultureCode     = "en-US",
                                                                                IsDefault       = true,
                                                                                Countries       = new List<string> { "US" }
                                                                            },

                                                                            new Market { 
                                                                                Name            = MarketName.Canada,
                                                                                Description     = "Canada",            
                                                                                CookieValue     = "CA",         
                                                                                CultureCode     = "en-US",
                                                                                Countries       = new List<string> { "CA" }
                                                                            }
                                                                        };
        }

        public static class Shopping
        {
            public static string ProductImagePath
            {
                get
                {
                    return string.Format(_productImagePath, GlobalSettings.ExigoApiCredentials.CompanyKey);
                }
            }
            private static string _productImagePath = "https://api.exigo.com/4.0/{0}/productimages/";
        }

        public static class CustomerImages
        {
            public static string DefaultTinyAvatarAsBase64 = "iVBORw0KGgoAAAANSUhEUgAAADYAAAA2CAYAAACMRWrdAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA7FpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYxIDY0LjE0MDk0OSwgMjAxMC8xMi8wNy0xMDo1NzowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wUmlnaHRzPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvcmlnaHRzLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcFJpZ2h0czpNYXJrZWQ9IkZhbHNlIiB4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ9InV1aWQ6MEM4OTYxNjUyOUZGREQxMTg2MTNGRTAyMERGNzdFNzMiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6RjM2OTlGNThFN0Y1MTFFMUI1MkJBQjU2OERFOTYxMDAiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6RjM2OTlGNTdFN0Y1MTFFMUI1MkJBQjU2OERFOTYxMDAiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNS4xIE1hY2ludG9zaCI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjA0NkVDNjY2MTUyMDY4MTE4QTZERTZDREFEOUI0RDhFIiBzdFJlZjpkb2N1bWVudElEPSJ1dWlkOjFDQjZBQ0E3NzRDNkUxMTE4RkVERDY3MTJBMTkyODZCIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+eQaBtgAAAoBJREFUeNrsmt9KAkEUxsdaKsMyVtrSgo1KIdguSgq68qo7X6KLnqqLXqIX6KoICiqJ6A+4UEpGUhlYUth+U0l/jHbdYzsOc2BxUXaZ35w538x8Y2j/4KjOJAzt/TMkGVf9A4ytrm9Jkbm1lSWepC4maUgLprXrxT1aN0voEaYP9rNe5z42EObf31Sq7On5hRXLFed66BwwAC3NjLNUQm/6e9yB5TE1ymoO4KF9zXL5Er8XFgxZyS4mOZzbTkg7gBNGlG3snJLCdQUFRfVs28Eys6avhgEuPR0XCyw1pjfEwU9Y5jAbCPeIA2YaQ2Q9bZmGGGAYfih+qmioZtBgscEwowyKIS3syoOizoQEi8gKdnNflROMYgWiti0KTIEpsGDAKKRZSDBIM+UGkcouIBmKBULvovJYEwfMLt2SgdlXt+KAnVyWSYZjpVpj+dKdWKpI0aCTQlk8ud87K/p+B2w44cAwjPwoGrIlpP2G2D0vBprxtoEhY61kLWdf84wLvaTaOr7wPMHvEmerLWA4dEAG3Aa8e2rfHkHi3XMLbiTK/UWcsHhxhOHdz5rDfPWCiT5/dUcC6gsMVpk1Yfx6suKpY4wovzLWm0JC+pH9fwWDqZmeipOZm98DHYULQgSlbUWQPIHB75ufjvvOkJcOzOpJnkFMB16U0zUYgFALlEc9XjKIYQqhcTvfaW56LWOZZKcgfuoQQgPIzZz95/DUgqqjVgMdnF1I/ll/WrMHkSHRgH6rP+wqtp1Fwff6+wLG5dbnyeR/B9qMuXPz0P6ydWqsPHAquTw32VFQn+sPbQfDDzAMv06PzwzKV1RgCkyBKbCmE/Rioi8kI5h0f3h+FWAAYmXlAkyCDB8AAAAASUVORK5CYII=";
            public static string DefaultLargeAvatarAsBase64 = "iVBORw0KGgoAAAANSUhEUgAAAJsAAACuCAIAAACutOjFAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA7FpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYxIDY0LjE0MDk0OSwgMjAxMC8xMi8wNy0xMDo1NzowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wUmlnaHRzPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvcmlnaHRzLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcFJpZ2h0czpNYXJrZWQ9IkZhbHNlIiB4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ9InV1aWQ6MEM4OTYxNjUyOUZGREQxMTg2MTNGRTAyMERGNzdFNzMiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QjU3RTkyNDJGRThEMTFFMUJENENDRTQxRTY3MDlBMzIiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QjU3RTkyNDFGRThEMTFFMUJENENDRTQxRTY3MDlBMzIiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNS4xIE1hY2ludG9zaCI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjBBODAxMTc0MDcyMDY4MTE4OEM2OEQ2MjNCOUQ1MzUy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";
        }

        public static class Mail
        {
            public static string SMTPServerUrl = "mail.exigo.com";
            public static int SMTPServerPort = 26;
            public static bool SMTPSecureConnectionRequired = false;
            public static string SMTPServerLoginName = "noreply@exigonow.com";
            public static string SMTPServerPassword = "whodaman";
            public static string WebmasterEmailAddress = "web@exigo.com";
            public static string NoReplyEmailAddress = "noreply@exigonow.com";
            public static string CorporateEmailAddress = "customercare@frezzor.com";
        }

        public static class Company
        {
            public static string Name = "Frezzor Inc.";
            public static string Address = "Po Box 13252";
            public static string City = "Newport Beach";
            public static string State = "California";
            public static string Zip = "92658";
            public static string Country = "United States";
            public static string Phone = "+1 949 215 3055";
            public static string Email = "customercare@frezzor.com";
            public static string Twitter = "frezzor";
            public static string Facebook = "frezzor";
        }
    }


    // Constants
    public static class Warehouses
    {
        public const int Default = 1;
        public const int UnitedStates = 1;
        public const int Germany = 2;
        public const int Austria = 3;
        public const int TestWarehouse = 4;
        public const int NewZealand = 5;
        public const int Australia = 6;

    }
    public static class PeriodTypes
    {
        public const int Default = 1;
        public const int Weekly = 1;
        public const int Quarterly = 3;
    }
    public static class NewsDepartments
    {
        public const int Backoffice = 6;
    }
    public static class CustomerTypes
    {
        public const int RetailCustomer = 1;
        public const int PreferredCustomer = 2;
        public const int IndependentDistributor = 3;
        public const int BusinessBuilder = 4;
    }
    public static class CustomerStatusTypes
    {
        public const int Deleted = 0;
        public const int Active = 1;
        public const int Terminated = 2;
        public const int Suspended = 3;
        public const int DoNotUse = 4;
        public const int Cancelled = 5;
        public const int Pending = 6;

    }
    public static class PriceTypes
    {
        public const int Retail = 1;
        public const int Preferred = 2;
        public const int Distributor = 3;
    }
    public static class Languages
    {
        public const int English = 0;
        public const int Spanish = 1;
        public const int German = 2;
        public const int French = 3;
        public const int Finnish = 12;
        public const int Italian = 17;
        public const int Swedish = 204;
    }
    public enum MarketName
    {
        UnitedStates,
        Canada
    }

    public static class VideoCatagories
    {
        public const int NutritionalShakes = 1;
        public const int Omega3s = 2;
        public const int Opportunity = 3;
        public const int Testimonials = 4;
        public const int Ingredients = 5;
        public const int UnCatagorized = 6;
    }
}