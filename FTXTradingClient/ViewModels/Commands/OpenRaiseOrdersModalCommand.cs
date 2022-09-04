using FTXTradingClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FTXTradingClient.ViewModels.Commands
{
    public class OpenRaiseOrdersModalCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public OpenRaiseOrdersModalCommand()
        {

        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Modal Modal = (Modal)parameter;
            Modal.IsOpen = true;
        }
    }
}
