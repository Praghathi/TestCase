using ExchangeRate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ExchangeRate.Controllers
{
    public class ValuesController : ApiController
    {
        public CurrencyRate currencyRatesVar = new CurrencyRate();


        private static CurrencyRate _download_serialized_json_data<CurrencyRate>(string url) where CurrencyRate : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                    Debug.WriteLine(json_data);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<CurrencyRate>(json_data) : new CurrencyRate();

            }
        }

        // GET api/<controller>
        [HttpGet]
        public string Get(string startDate, string endDate, string baseCurrency, string targetCurrency)
        {
            double minRate = 0;
            double maxRate = 0;
            double avgRate = 0, exchangeRate = 0;
            int count = 0;
            double baseCurValue = 0, tgtCurValue = 0;
            string minDate = string.Empty, maxDate = string.Empty;
            string curDate = string.Empty;
            string json;
            
            startDate = startDate.Trim();
            endDate = endDate.Trim();
            var url = string.Empty;
          
            baseCurrency = baseCurrency.ToUpper().Trim();
            targetCurrency = targetCurrency.ToUpper().Trim();

            int startYear = Convert.ToInt32(startDate.Substring(0, 3));
            if ( startYear < 2009)
            {
                url = "https://api.exchangeratesapi.io/history?start_at=" + startDate + "&end_at=" + endDate;
            }
            else if (targetCurrency == "EUR")
            {
                url = "https://api.exchangeratesapi.io/history?start_at=" + startDate + "&end_at=" + endDate + "&symbols=" + baseCurrency;
            }
            else if (baseCurrency == "EUR")
            {
                url = "https://api.exchangeratesapi.io/history?start_at=" + startDate + "&end_at=" + endDate + "&symbols=" + targetCurrency;
            }
            else
            {
                url = "https://api.exchangeratesapi.io/history?start_at=" + startDate + "&end_at=" + endDate + "&symbols=" + baseCurrency + "," + targetCurrency;
            }

            try
            {

                var currencyRatesVar = _download_serialized_json_data<CurrencyRate>(url);
                var i = string.Empty;

                // Loop over list.
                foreach (KeyValuePair<string, Dictionary<string, double>> currentobj in currencyRatesVar.Rates)
                {
                    curDate = currentobj.Key;
                    ++count;

                    baseCurValue = 0; tgtCurValue = 0;
                    foreach (KeyValuePair<string, double> innerobj in currentobj.Value)
                    {
                        if (innerobj.Key == baseCurrency)
                            baseCurValue = innerobj.Value;
                        else if (innerobj.Key == targetCurrency)
                            tgtCurValue = innerobj.Value;

                    }

                    /* to adjust for eur conversion */
                    if (baseCurrency == "EUR")
                        baseCurValue = 1;
                    else if (targetCurrency == "EUR")
                        tgtCurValue = 1;

                    /* exchange rate conversion from std base currency */
                    if (baseCurValue == 0 || tgtCurValue == 0)
                        exchangeRate = 0;
                    else
                        exchangeRate = (1 / baseCurValue) * tgtCurValue;


                    /* intialising for the first iteration */
                    if (count == 1)
                    {
                        minRate = exchangeRate;
                        minDate = curDate;
                        maxRate = exchangeRate;
                        maxDate = curDate;
                        avgRate = exchangeRate;
                    }
                    else
                    {
                        /* min rate  & max rate checking */
                        if (exchangeRate < minRate && exchangeRate !=0)
                        {
                            minRate = exchangeRate;
                            minDate = curDate;
                        }
                        else if (maxRate < exchangeRate)
                        {
                            maxRate = exchangeRate;
                            maxDate = curDate;
                        }

                        /*adding all exchange rates for avg calculation */
                        avgRate = avgRate + exchangeRate;
                    }
                }

                /* average rate calculation */
                avgRate = avgRate / count;


                Result result = new Result();
                result.minRate = minRate;
                result.maxRate = maxRate;
                result.avgRate = avgRate;
                result.minDate = minDate;
                result.maxDate = maxDate;

                /*
                var response = new HttpResponseMessage();
                response.Content = new StringContent(outputStr);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                */
                json = JsonConvert.SerializeObject(result);
            }

            catch (Exception)
            {
                json = "No data";
            }

            return json;
            
        }

        
     
        /*
        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
        */
    }
}