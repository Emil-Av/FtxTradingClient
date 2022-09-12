using FTXTradingClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FTXTradingClient.ViewModels.Commands
{
    // when the tab item is clicked, the TextBox for "Pair" is automatically selected. Thus the Focusable attribute is set default to false. 
    // The command sets Focusable to true when the mouse is over the TextBox
    public class AllowFocusCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        TickerViewModel VM { get; set; }
        public AllowFocusCommand(TickerViewModel vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            TextBox Pair = parameter as TextBox;
            Pair.Focusable = true;
        }
    }
}
