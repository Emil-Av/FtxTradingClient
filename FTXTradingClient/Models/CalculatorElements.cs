using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.Models
{
    public class CalculatorElements
    {
        public string USDRiskValue { get; set; }

        public string Pair { get; set; }

        // row 1 (below comboboxes)
        public string InitialEntry { get; set; }

        public string Limit { get; set; }

        public string InitialStop { get; set; }

        public string EntrySize { get; set; }

        public string EntryQuantity { get; set; }

        // row 2

        public string OneToOneProfit { get; set; }

        public string PosInPerc { get; set; }

        public string AccountPercPos { get; set; }

        public string BestTarget { get; set; }

        public string OCOQuantity { get; set; }

        // row 3

        public string Loss { get; set; }
        
        public string PosLossPerc { get; set; }

        public string AccountPercLoss { get; set; }

        public string WorstTarget { get; set; }

        public string StopQuantity { get; set; }
    }
}
