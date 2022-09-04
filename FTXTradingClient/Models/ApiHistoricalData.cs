using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.Models
{
    // classes to get the historical data from the API
    public class ApiHistoricalData
    {
        public DateTime StartTime { get; set; }
        public double Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }

    public class ApiHistoricalDataResponse
    {
        public bool Success { get; set; }
        public List<ApiHistoricalData> Result { get; set; }
    }
}
