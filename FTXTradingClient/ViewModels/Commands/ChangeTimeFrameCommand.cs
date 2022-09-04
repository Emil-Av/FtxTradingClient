using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FTXTradingClient.ViewModel.Commands
{
    public class ChangeTimeFrameCommand : ICommand
    {
        public TickerViewModel VM { get; set; }

        public event EventHandler CanExecuteChanged;

        public ChangeTimeFrameCommand(TickerViewModel vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Dictionary<string, int> TimeFrames = new Dictionary<string, int>();

            string timeFrame = parameter as string;

            TimeFrames.Add("5M", 300);
            TimeFrames.Add("15M", 900);
            TimeFrames.Add("1H", 3600);
            TimeFrames.Add("4H", 14400);
            TimeFrames.Add("D", 86400);
            TimeFrames.Add("W", 604800);
            TimeFrames.Add("M", 86400 * 30);

            _ = VM.ChangeTimeFrame(VM.Symbol, TimeFrames[timeFrame]);
            // update the two properties so when changing the time frame with mouse click the correct information is displayed
            VM.TimeFrame = TimeFrames[timeFrame];
            VM.ChartInfo = VM.Symbol + "   " + timeFrame;
        }
    }
}
