using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TourScannerBot.Models.CurrencyApi;

namespace TourScannerBot.ApiManager.CurrencyApi
{
    public class CurrencyApiManager
    {
        private readonly IConfiguration configuration;

        private HttpClient httpClient;

        public CurrencyApiManager(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<List<CurrencyState>> ReturnCurrency()
        {
            var content = await this.ExecuteGetRequest();
            var resultCity = await content.Content.ReadAsAsync<List<CurrencyState>>();

            return resultCity;
        }

        public async Task<HttpResponseMessage> ExecuteGetRequest()
        {
            var section = this.configuration.GetSection("CurrencyApi");
            var resultUrl = string.Format(section["URL"]);

            var result = await httpClient.GetAsync(resultUrl);

            return result;
        }
    }
}
