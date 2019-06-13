using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TourScannerBot.Models.WeatherApi;

namespace TourScannerBot.ApiManager.WeatherApi
{
    public class WeatherApiManager
    {
        private readonly IConfiguration configuration;

        private HttpClient httpClient;

        public WeatherApiManager(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<WeatherForecast> ReturnWeather(string city)
        {
            var content = await this.ExecuteGetRequest(city);
            var resultCity = await content.Content.ReadAsAsync<WeatherForecast>();

            return resultCity;
        }

        public async Task<HttpResponseMessage> ExecuteGetRequest(string city)
        {
            var section = this.configuration.GetSection("WeatherApi");
            var resultUrl = string.Format(section["URL"], city);

            var result = await httpClient.GetAsync(resultUrl);

            return result;
        }
    }
}
