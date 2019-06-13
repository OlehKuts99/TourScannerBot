using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TourScannerBot.Models.CityApi;

namespace TourScannerBot.ApiManager.CityApi
{
    public class CityApiManager
    {
        private readonly IConfiguration configuration;

        private HttpClient httpClient;

        public CityApiManager(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<CityModel> ReturnCity(string city)
        {
            var content = await this.ExecuteGetRequest(city);
            var resultCity = await content.Content.ReadAsAsync<CityModel>();

            return resultCity;
        }

        public async Task<HttpResponseMessage> ExecuteGetRequest(string city)
        {
            var section = this.configuration.GetSection("CityApi");
            var resultUrl = string.Format(section["URL"], city);

            var result = await httpClient.GetAsync(resultUrl);

            return result;
        }
    }
}
