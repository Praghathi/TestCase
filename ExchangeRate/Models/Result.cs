using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExchangeRate.Models
{
    public class Result
    {
        public double minRate { get; set; }
        public string minDate { get; set; }
        public double maxRate { get; set; }
        public string maxDate { get; set; }
        public double avgRate { get; set; }
    }
}