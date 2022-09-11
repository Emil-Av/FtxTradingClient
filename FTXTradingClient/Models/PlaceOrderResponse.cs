using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.Models
{
    public class Result
    {
        public int id { get; set; }
        public string market { get; set; }
        public string future { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public double orderPrice { get; set; }
        public double triggerPrice { get; set; }
        public double size { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public object triggeredAt { get; set; }
        public object orderId { get; set; }
        public object error { get; set; }
        public bool reduceOnly { get; set; }
        public object trailValue { get; set; }
        public object trailStart { get; set; }
        public object cancelledAt { get; set; }
        public object cancelReason { get; set; }
        public bool retryUntilFilled { get; set; }
        public string orderType { get; set; }
    }

    public class PlaceOrderResponse
    {
        public bool success { get; set; }
        public Result result { get; set; }
    }
}
