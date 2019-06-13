using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TourScannerBot.ApiManager.CityApi;
using TourScannerBot.ApiManager.CountryApi;
using TourScannerBot.ApiManager.CurrencyApi;
using TourScannerBot.ApiManager.WeatherApi;
using TourScannerBot.Models.WeatherApi;

namespace TourScannerBot.ApiManager
{
    public class CommonApiManager : ICommonApiManager
    {
        private BookingDetails bookingDetails;

        private readonly IConfiguration configuration;

        private readonly HttpClient httpClient;

        public CommonApiManager(IConfiguration configuration, HttpClient httpClient, BookingDetails bookingDetails)
        { 
            this.bookingDetails = bookingDetails;
            this.configuration = configuration;
            this.httpClient = httpClient;

            CityApiManager = new CityApiManager(configuration, httpClient);
            CountryApiManager = new CountryApiManager(configuration, httpClient);
            CurrencyApiManager = new CurrencyApiManager(configuration, httpClient);
            WeatherApiManager = new WeatherApiManager(configuration, httpClient);
        }

        public CityApiManager CityApiManager { get; set; }

        public CountryApiManager CountryApiManager { get; set; }

        public CurrencyApiManager CurrencyApiManager { get; set; }

        public WeatherApiManager WeatherApiManager { get; set; }

        public async Task<string> MainFlow()
        {
            var resultString = await this.ReturnCurrencyString();

            resultString += '\n';
            resultString += await this.ReturnWeatherString();

            return resultString;
        }

        private async Task<string> ReturnCurrencyString()
        {
            var cityCountry = await this.ReturnCity();
            var countryCurrency = await this.ReturnCountry(cityCountry);
            var currencyResult = await this.ReturnCurrency(countryCurrency, cityCountry);

            if (currencyResult == double.MaxValue)
            {
                return string.Format("I can't find currency converter for {0}", cityCountry);
            }

            return string.Format("You are going to {0}, where is currency : {1}, 1 USD - {2} {3}",
                    cityCountry, countryCurrency, currencyResult, countryCurrency);
        }

        private async Task<string> ReturnCity()
        {
            var result = await CityApiManager.ReturnCity(bookingDetails.Destination);
            var resultString = bookingDetails.PossibleVariants.Count <= 1 ? 
                result.Data[0].Country : result.Data[int.Parse(bookingDetails.PossibleVariants[0]) - 1].Country;

            return resultString;
        }

        private async Task<string> ReturnCountry(string country)
        {
            var result = await CountryApiManager.ReturnCountry(country);
            var resultString = result[0].Currencies[0].Code;

            return resultString;
        }

        private async Task<double> ReturnCurrency(string code, string country)
        {
            var result = await CurrencyApiManager.ReturnCurrency();
            var currencyState = result.Where(c => c.Code == code).FirstOrDefault();

            if (currencyState == null && country != "Ukraine")
            {
                return double.MaxValue;
            }

            var resultNumber = country == "Ukraine" ? result.Where(c => c.Code == "USD").First().Rate 
                : currencyState.Rate / result.Where(c => c.Code == "USD").First().Rate;

            return country == "Ukraine" ? resultNumber : 1 / resultNumber;
        }

        private async Task<string> ReturnWeatherString()
        {
            var result = await WeatherApiManager.ReturnWeather(bookingDetails.Destination);
            var dateTime = DateTime.Parse(bookingDetails.TravelDate);

            if (dateTime.Date - DateTime.Now.Date > TimeSpan.FromDays(5))
            {
                return string.Format("Sorry, i can't find weather for {0}", dateTime.ToString("dd-MM-yyyy")); 
            }

            var weatherString = string.Format("In {0}, at {1} will be {2} weather, temperature will be : {3} C°", bookingDetails.Destination, 
                dateTime.ToString("dd-MM-yyyy"), this.DetermineWeateherState(dateTime, result), 
                this.ReturnTemperature(dateTime, result));

            return weatherString;
        }

        private string DetermineWeateherState(DateTime date, WeatherForecast weatherForecast)
        {
            var list = weatherForecast.List.Where(f => DateTime.Parse(f.ForecastDateTime).Date == date.Date).ToList();
            var resultString = "None";

            foreach (var part in list)
            {
                for (int i = 0; i < part.Weather.Count; i++)
                {
                    if (part.Weather[i].State == "Rain")
                    {
                        resultString = "Rain";
                    }

                    if (part.Weather[i].State == "Clouds" && resultString != "Rain")
                    {
                        resultString = "Clouds";
                    }
                }
            }

            if (resultString == "None")
            {
                resultString = "Clear";
            }

            return resultString;
        }

        private double ReturnTemperature(DateTime date, WeatherForecast weatherForecast)
        {
            var list = weatherForecast.List.Where(f => DateTime.Parse(f.ForecastDateTime).Date == date.Date).ToList();
            var returnString = list.Max(f => f.Temperature.Temp);

            return returnString;
        }
    }
}
