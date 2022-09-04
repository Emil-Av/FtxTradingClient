using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FTXTradingClient.ViewModel.Commands
{
    public class MouseLeftButtonDownCommand : ICommand
    {
        public TickerViewModel VM { get; set; }

        public event EventHandler CanExecuteChanged;

        public MouseLeftButtonDownCommand(TickerViewModel vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Dictionary<int, string> TimeFrames = new Dictionary<int, string>();

            TimeFrames.Add(300, "5M");
            TimeFrames.Add(900, "15M");
            TimeFrames.Add(3600, "1H");
            TimeFrames.Add(14400, "4H");
            TimeFrames.Add(86400, "D");
            TimeFrames.Add(604800, "W");
            TimeFrames.Add(86400 * 30, "M");

            // cast the object into stack panel
            StackPanel spTicker = parameter as StackPanel;
            // create new text block object from the first child of the stack panel
            TextBlock tbMarket = (TextBlock)spTicker.Children[0];
            // load the chart
            _ = VM.ChangeTimeFrame(tbMarket.Text, VM.TimeFrame);
            // update the ChartInfo property
            VM.ChartInfo = tbMarket.Text + "   " + TimeFrames[VM.TimeFrame];
        }
    }
}
