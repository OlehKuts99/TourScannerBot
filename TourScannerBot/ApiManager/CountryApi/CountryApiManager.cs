using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TourScannerBot.Models.CountryApi;

namespace TourScannerBot.ApiManager.CountryApi
{
    public class CountryApiManager
    {
        private readonly IConfiguration configuration;

        private HttpClient httpClient;

        public CountryApiManager(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<List<CountryModel>> ReturnCountry(string country)
        {
            var content = await this.ExecuteGetRequest(country);
            var resultCountry = await content.Content.ReadAsAsync<List<CountryModel>>();

            return resultCountry;
        }

        public async Task<HttpResponseMessage> ExecuteGetRequest(string country)
        {
            var section = this.configuration.GetSection("CountryApi");
            var resultUrl = string.Format(section["URL"], string.Join("%20", country.Split(" ")));

            var result = await httpClient.GetAsync(resultUrl);

            return result;
        }
    }
}
