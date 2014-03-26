using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReplicatedSite.Models;
using ReplicatedSite.Exigo.WebService;

namespace ReplicatedSite.Services
{
    public class CountryRegionService
    {
        public List<Country> GetCountries()
        {
            var result = new List<Country>();

            using (var context = ExigoApiFactory.CreateWebServiceContext())
            {
                result = context.GetCountryRegions(new GetCountryRegionsRequest())
                    .Countries.Select(c => new Country()
                    {
                        CountryCode = c.CountryCode,
                        CountryName = c.CountryName
                    }).ToList();
            }

            return result;
        }
        public List<Region> GetRegions(string CountryCode)
        {
            var result = new List<Region>();

            using (var context = ExigoApiFactory.CreateWebServiceContext())
            {
                result = context.GetCountryRegions(new GetCountryRegionsRequest()
                    {
                        CountryCode = CountryCode
                    })
                    .Regions.Select(c => new Region()
                    {
                        RegionCode = c.RegionCode,
                        RegionName = c.RegionName
                    }).ToList();
            }

            return result;
        }
        public CountryRegionCollection GetCountryRegions(string CountryCode)
        {
            var result = new CountryRegionCollection();

            using (var context = ExigoApiFactory.CreateWebServiceContext())
            {
                var response = context.GetCountryRegions(new GetCountryRegionsRequest()
                {
                    CountryCode = CountryCode
                });


                result.Countries.Select(c => new Country()
                    {
                        CountryCode = c.CountryCode,
                        CountryName = c.CountryName
                    }).ToList();

                result.Regions = response.Regions.Select(c => new Region()
                    {
                        RegionCode = c.RegionCode,
                        RegionName = c.RegionName
                    }).ToList();
            }

            return result;
        }
    }
}