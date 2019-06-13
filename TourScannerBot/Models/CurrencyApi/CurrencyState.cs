using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourScannerBot.Models.CurrencyApi
{
    public class CurrencyState
    {
        public double Rate { get; set; }

        [JsonProperty(PropertyName = "Cc")]
        public string Code { get; set; }
    }
}
