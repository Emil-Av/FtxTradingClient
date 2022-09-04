using FTXTradingClient.FTX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.Models
{
    public class CalculatorComboBoxes
    {
        public List<string> ShortLong { get; set; } = new List<string>() { "Long", "Short" };

        public List<string> Strategy { get; set; } = new List<string>() { "Cradle", "Breakout", "Booster" };

        public List<string> MarginSpot { get; set; } = new List<string>() { "Margin", "Spot" };

        public List<int> DecimalPlaces { get; set; } = new List<int>() { 2, 3, 4, 5, 6, 7, 8 };

        public List<OrderType> OrderType { get; set; } = new List<OrderType>() { FTX.OrderType.limit, FTX.OrderType.market };

    }
}
