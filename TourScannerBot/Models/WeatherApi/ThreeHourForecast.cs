using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourScannerBot.Models.WeatherApi
{
    public class ThreeHourForecast
    {
        [JsonProperty(PropertyName = "Main")]
        public Temperature Temperature { get; set; }

        public List<WeatherState> Weather { get; set; }

        [JsonProperty(PropertyName = "Dt_txt")]
        public string ForecastDateTime { get; set; }
    }
}
