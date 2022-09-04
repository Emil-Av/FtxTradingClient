using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.FTX.Models
{
    // classes for the data that comes from the web socket
    public class WSFuturesResponse
    {
        public string Channel { get; set; }
        public string Market { get; set; }
        public string Type { get; set; }
        public WSFuturesData Data { get; set; } = new WSFuturesData();

    }
    public class WSFuturesData
    {
        public double? Bid { get; set; }
        public double? Ask { get; set; }
        public double? BidSize { get; set; }
        public double? AskSize { get; set; }
        public double? Last { get; set; }
        public double? Time { get; set; }
    }
}
