using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FTXTradingClient.Models
{
    // model for the observable collection
    public class TickersModel
    {
        public string Market { get; set; }

        public double? Price { get; set; }

        // the LastPrice property used for the converter BoolToForeground.
        public static string LastPrice { get; set; }

        public TickersModel(string market)
        {
            Market = market;
        }
  
        public TickersModel(string market, double? price)
        {
            Market = market;
            Price = price;
        }
    }
}
