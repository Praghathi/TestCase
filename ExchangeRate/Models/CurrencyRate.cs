namespace ExchangeRate.Models
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Net;
    using Newtonsoft.Json;
    using System.Globalization;
    using Newtonsoft.Json.Converters;


  
    public class CurrencyRate
    {
        [JsonProperty("end_at")]
        public DateTimeOffset EndAt { get; set; }

        [JsonProperty("rates")]
        public Dictionary<string, Dictionary<string, double>> Rates { get; set; }

        [JsonProperty("start_at")]
        public DateTimeOffset StartAt { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }
    }

}