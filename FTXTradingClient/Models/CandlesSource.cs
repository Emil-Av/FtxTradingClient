using FancyCandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTXTradingClient.Models
{
    // class for the candle's source, implementing the interface ICandleSource from FancyCandles
    public class CandlesSource : ObservableCollection<ICandle>, ICandlesSource
    {
        public CandlesSource(TimeFrame timeFrame)
        {
            this.timeFrame = timeFrame;
        }

        private readonly TimeFrame timeFrame;
        public FancyCandles.TimeFrame TimeFrame 
        { 
            get 
            { 
                return timeFrame; 
            } 
        }
    }
}
