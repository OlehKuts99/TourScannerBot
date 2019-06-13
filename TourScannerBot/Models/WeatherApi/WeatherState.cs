using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourScannerBot.Models.WeatherApi
{
    public class WeatherState
    {
        [JsonProperty(PropertyName = "Main")]
        public string State { get; set; }
    }
}
