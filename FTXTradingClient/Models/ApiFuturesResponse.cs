using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.Models
{
    // classes for the API data
    public class ApiFuturesData
    {
        public string Name { get; set; }
        //public string underlying { get; set; }
        //public string description { get; set; }
        //public string type { get; set; }
        //public DateTime? expiry { get; set; }
        //public bool perpetual { get; set; }
        //public bool expired { get; set; }
        //public bool enabled { get; set; }
        //public bool postOnly { get; set; }
        //public double priceIncrement { get; set; }
        //public double sizeIncrement { get; set; }
        //public double last { get; set; }
        //public double bid { get; set; }
        //public double ask { get; set; }
        //public double index { get; set; }
        //public double mark { get; set; }
        //public double imfFactor { get; set; }
        //public double lowerBound { get; set; }
        //public double upperBound { get; set; }
        //public string underlyingDescription { get; set; }
        //public string expiryDescription { get; set; }
        //public object moveStart { get; set; }
        //public double marginPrice { get; set; }
        //public double positionLimitWeight { get; set; }
        //public string group { get; set; }
        //public double change1h { get; set; }
        //public double change24h { get; set; }
        //public double changeBod { get; set; }
        //public double volumeUsd24h { get; set; }
        //public double volume { get; set; }
        //public double openInterest { get; set; }
        //public double openInterestUsd { get; set; }
    }

    public class ApiFuturesResponse
    {
        public bool Success { get; set; }
        public List<ApiFuturesData> Result { get; set; }
    }
}
