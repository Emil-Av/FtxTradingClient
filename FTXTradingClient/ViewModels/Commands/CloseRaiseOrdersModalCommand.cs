using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FTXTradingClient.ViewModels.Commands
{
    public class CloseRaiseOrdersModalCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Modal Modal = (Modal)parameter;
            Modal.IsOpen = false;
        }
    }
}
