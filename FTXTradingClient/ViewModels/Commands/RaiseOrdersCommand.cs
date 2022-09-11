using FTXTradingClient.FTX;
using FTXTradingClient.Models;
using FTXTradingClient.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FTXTradingClient.ViewModels.Commands
{
    public class RaiseOrdersCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private TickerViewModel VM { get; set; }

        private decimal EntryPrice { get; set; }

        private decimal StopPrice { get; set; }

        private decimal LimitPrice { get; set; }

        private decimal Amount { get; set; }


        private SideType InitialOrderSide { get; set; }

        private SideType StopOrderSide { get; set; }


        private PlaceOrderResponse response { get; set; } = new PlaceOrderResponse();

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
            if (VM.client.ApiKey.Equals("nBUxZ0spCcHVNb4NNUQF7FZ-S4R2orM1ZjogeMZ-"))
            {
                MessageBox.Show("You are currently using the developer's test account which is a read only account. " +
                    "If you want to trade, change the api key and the secret with yours. Also disable the readonly option of your API key in your FTX account. \nAlternatively you can contact the developer.");
            }
            else
            {
                EntryPrice = Convert.ToDecimal(VM.InitialEntry);
                LimitPrice = Convert.ToDecimal(VM.Limit);
                Amount = Convert.ToDecimal(VM.EntryQuantity);

                if (VM.SelectedShortSlong == "Long")
                {
                    InitialOrderSide = SideType.buy;
                    StopOrderSide = SideType.sell;
                }
                else
                {
                    InitialOrderSide = SideType.sell;
                    StopOrderSide = SideType.buy;
                }
                StopPrice = Convert.ToDecimal(VM.InitialStop);
                // initial order
                _ = PlaceInitialOrder(VM.Symbol, InitialOrderSide, Amount, EntryPrice, LimitPrice, false);
                // stop order
                _ = PlaceStopOrder(VM.Symbol, StopOrderSide, Amount, StopPrice, true);
            }
            
        }

        private async Task<PlaceOrderResponse> PlaceStopOrder(string instrument, SideType side, decimal price, decimal amount, bool reduceOnly = false)
        {

            dynamic Api_Response = await VM.api.PlaceStopOrderAsync(instrument, side, price, amount, reduceOnly);

            return Api_Response;
        }

        private async Task<PlaceOrderResponse> PlaceInitialOrder(string symbol, SideType side, decimal amount, decimal entryPrice, decimal limitprice, bool reduceOnly)
        {
            dynamic Api_Response = await VM.api.PlaceStopOrderAsync(symbol, side, amount, entryPrice, limitprice, reduceOnly);
            response = JsonConvert.DeserializeObject<PlaceOrderResponse>(Api_Response);

            if (response.success)
            {
                VM.Status = "Success: " + response.success + ". Reduce Only: " + response.result.reduceOnly + ".Trigger price: " + response.result.triggerPrice;
            }

            return response;
        }
    }
}
