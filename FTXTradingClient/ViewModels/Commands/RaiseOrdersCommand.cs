using FTXTradingClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FTXTradingClient.ViewModels.Commands
{
    public class RaiseOrdersCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        TickerViewModel VM;

        public RaiseOrdersCommand(TickerViewModel vm)
        {
            VM = vm;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //VM.api.PlaceOrderAsync(VM.Symbol, VM.CalculatorComboBoxes.OrderType, VM.InitialEntry, VM.SideType, VM.Amo)
        }
    }
}
