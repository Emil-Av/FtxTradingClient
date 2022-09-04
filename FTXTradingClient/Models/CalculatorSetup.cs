using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.Models
{
    public class CalculatorSetup
    {
        public double AccountSize { get; set; } = 2500;

        public double AccountSizeInBTC { get; set; }

        public double AccountRiskInPerc { get; set; } = 1;

        public double AmountRiskInDollars { get; set; }

        public double ExchPosSizeLimitPerc { get; set; }

        public double AcceptableSlippageInPerc { get; set; }

        public double ScaleOutInPerc { get; set; }

        public double FeeInPerc { get; set; }
    }
}
