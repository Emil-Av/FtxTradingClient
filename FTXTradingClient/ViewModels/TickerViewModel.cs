using FancyCandles;
using FTXTradingClient.FTX;
using FTXTradingClient.FTX.Models;
using FTXTradingClient.FTX.Util;
using FTXTradingClient.Models;
using FTXTradingClient.ViewModel.Commands;
using FTXTradingClient.ViewModels.Commands;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using WebSocket4Net;


namespace FTXTradingClient.ViewModel
{
    public class TickerViewModel : INotifyPropertyChanged
    {
        #region API, ListView and Chart Properties
        // url for the web socket
        protected string url = "wss://ftx.com/ws/";

        // the web socket instance
        protected WebSocket _webSocketClient;

        // action for the web socket
        public Action OnWebSocketConnect;

        public Client client { get; set; } = new Client("nBUxZ0spCcHVNb4NNUQF7FZ-S4R2orM1ZjogeMZ-", "LuFUdRpvQcW82i_y31mdeJ_sSdJutzvKBFZf4X9u");
        public FtxRestApi api { get; set; } 


        // default time frame on start - 4H
        public int TimeFrame = 14400;

        private string _chartInfo;

        public string ChartInfo
        {
            get => _chartInfo;
            set
            {
                _chartInfo = value;
                OnPropertyChanged(nameof(ChartInfo));
            }
        }

        private string _status;

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private string _tickersFilter = string.Empty;

        public string TickersFilter
        {
            get => _tickersFilter;
            set
            {
                _tickersFilter = value;
                OnPropertyChanged(nameof(TickersFilter));
                TickersCollectionView.Refresh();
            }
        }

        // lock object to use in BindingOperations
        private readonly object _tickersLock;


        // FutureResponse property that gets updated on every new message from the web socket
        private WSFuturesResponse _futuresResponse;
        public WSFuturesResponse FuturesResponse
        {
            get => _futuresResponse;

            set
            {
                // on every update the OC gets updated here
                _futuresResponse = value;
                OnPropertyChanged(nameof(FuturesResponse));

                // on every update get the Tickers object that got updated
                var found = Tickers.FirstOrDefault(x => x.Market == FuturesResponse.Market);
                // get it's index
                int i = Tickers.IndexOf(found);

                // when there is a new price
                if (Tickers[i].Price != FuturesResponse.Data.Last)
                {
                    // change the color of the price according to the new price
                    if (Tickers[i].Price < FuturesResponse.Data.Last)
                    {
                        TickersModel.LastPrice = "higher";
                    }
                    else if (Tickers[i].Price > FuturesResponse.Data.Last)
                    {
                        TickersModel.LastPrice = "lower";
                    }
                    else
                    {
                        TickersModel.LastPrice = "same";
                    }
                    // update the OC with the new price
                    Tickers[i] = new TickersModel(FuturesResponse.Market, FuturesResponse.Data.Last);
                }

                // update the chart being displayed
                if (FuturesResponse.Market == Symbol)
                {
                    int n = Candles.Count;
                    if (n != 0)
                    {
                        Candle lastCandle = (Candle)Candles[n - 1];
                        // update the last candle only when there is a change in price. Thus the dispatcher isn't called twice a second which drastically
                        // improves the performance of the app
                        if (lastCandle.C != FuturesResponse.Data.Last)
                        {
                            if (FuturesResponse.Data.Last != null)
                            {
                                double newC = (double)FuturesResponse.Data.Last;
                                double newH = Math.Max(newC, lastCandle.H);
                                double newL = Math.Min(newC, lastCandle.L);
                                Candle newCandle = new Candle(lastCandle.t, lastCandle.O, newH, newL, newC, lastCandle.V);

                                try
                                {
                                    Application.Current.Dispatcher.Invoke(delegate
                                    {
                                        if (Candles.Count == n)
                                        {
                                            Candles[n - 1] = newCandle;
                                        }
  
                                    }, DispatcherPriority.Background);
                                }
                                catch (TaskCanceledException)
                                {

                                }
                            }
                        }    
                    }
                }
            }
        }

        // create the observable collection (the list view)
        private ObservableCollection<TickersModel> _tickers;
        public ObservableCollection<TickersModel> Tickers
        {
            get
            {
                return _tickers;
            }
            set
            {
                _tickers = value;
                OnPropertyChanged("Tickers");
                // binding operations is needed because the OC is modified in a different thread than the UI thread
                BindingOperations.EnableCollectionSynchronization(_tickers, _tickersLock);
            }
        }

        public ICollectionView TickersCollectionView { get; }


        private CandlesSource _candles;

        public CandlesSource Candles
        {
            get
            {
                return _candles;
            }
            set
            {
                _candles = value;
                OnPropertyChanged(nameof(Candles));
            }
        }

        #endregion

        #region Commands
        public ChangeTimeFrameCommand ChangeTimeFrameCommand { get; set; }

        public MouseLeftButtonDownCommand MouseLeftButtonDownCommand { get; set; }

        public OpenRaiseOrdersModalCommand OpenRaiseOrdersModalCommand { get; set; }

        public CloseRaiseOrdersModalCommand CloseRaiseOrdersModalCommand { get; set; }

        public RaiseOrdersCommand RaiseOrdersCommand { get; set; }
        #endregion


        #region Calculator Setup and ComboBoxes Properties

        public CalculatorComboBoxes CalculatorComboBoxes { get; set; }

        private string _selectedShortLong = "Long";

        public string SelectedShortSlong
        {
            get
            {
                return _selectedShortLong;
            }
            set
            {
                _selectedShortLong = value;
                OnPropertyChanged(nameof(SelectedShortSlong));
                InitialEntry = InitialEntry;
                if (value == "Long")
                {
                    BuySellButtonContent = "Buy " + Symbol;
                    SideType = SideType.buy;
                }
                else
                {
                    BuySellButtonContent = "Sell " + Symbol;
                    SideType = SideType.sell;
                }
            }
        }

        private string _buySellButtonContent;
        public string BuySellButtonContent
        {
            get
            {
                return _buySellButtonContent;
            }
            set
            {
                _buySellButtonContent = value;
                OnPropertyChanged(nameof(BuySellButtonContent));
            }
        }

        private string _symbol;

        public string Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                OnPropertyChanged(nameof(Symbol));

                if (SelectedShortSlong == "Long")
                {
                    BuySellButtonContent = "Buy " + value;
                }
                else
                {
                    BuySellButtonContent = "Sell " + value;
                }
            }
        }

        private int _selectedDecimalPlaces = 6;

        public int SelectedDecimalPlaces
        {
            get
            {
                return _selectedDecimalPlaces;
            }
            set
            {
                _selectedDecimalPlaces = value;
                Status = value.ToString();
            }
        }
        private string _selectedMarginSpot = "Margin";

        public string SelectedMarginSpot
        {
            get
            {
                return _selectedMarginSpot;
            }
            set
            {
                _selectedMarginSpot = value;
                Status = value;
            }
        }

        private string _selectedStrategy = "Cradle";

        public string SelectedStrategy
        {
            get
            {
                return _selectedStrategy;
            }
            set
            {
                _selectedStrategy = value;
                Status = value;
            }
        }

        private double _tradeAccountSize = 5000;

        public double TradeAccountSize
        {
            get => _tradeAccountSize;
            set
            {
                _tradeAccountSize = value;
                OnPropertyChanged(nameof(TradeAccountSize));

                try
                {
                    double result = TradeAccountSize * double.Parse(AccountRiskInPerc) / 100;
                    AmountRiskInDollars = result;
                }
                catch { }
            }
        }


        private string _accountRiskInPerc = "0.5";

        public string AccountRiskInPerc
        {
            get
            {
                return _accountRiskInPerc;
            }

            set
            {
                _accountRiskInPerc = value;
                OnPropertyChanged(nameof(AccountRiskInPerc));

                if (AccountRiskInPerc != "")
                {
                    double result = TradeAccountSize * double.Parse(AccountRiskInPerc) / 100;
                    AmountRiskInDollars = result;
                }
            }
        }

        private double _amountRiskInDollars = 25;

        public double AmountRiskInDollars
        {
            get
            {
                return _amountRiskInDollars;
            }
            set
            {
                _amountRiskInDollars = value;
                OnPropertyChanged(nameof(AmountRiskInDollars));

            }
        }

        private double _exchangePosSizeLimitInPerc = 1;

        public double ExchangePosSizeLimitInPerc
        {
            get
            {
                return _exchangePosSizeLimitInPerc;
            }
            set
            {
                _exchangePosSizeLimitInPerc = value;
                OnPropertyChanged(nameof(ExchangePosSizeLimitInPerc));
            }
        }

        private double _acceptableSlippagePerc = 0.2;

        public double AcceptableSlippacePerc
        {
            get
            {
                return _acceptableSlippagePerc;
            }
            set
            {
                _acceptableSlippagePerc = value;
                OnPropertyChanged(nameof(AcceptableSlippacePerc));
            }
        }

        private double _scaleOut = 0.5;

        public double ScaleOut
        {
            get
            {
                return _scaleOut;
            }
            set
            {
                _scaleOut = value;
                OnPropertyChanged(nameof(ScaleOut));
            }
        }

        private double _tradeFees = 0.041 / 100;

        public double TradeFees
        {
            get
            {
                return _tradeFees;
            }
            set
            {
                _tradeFees = value;
                OnPropertyChanged(nameof(TradeFees));
            }
        }


        #endregion

        #region Calculator Elements

        #region row 1
        // row 1
        private bool _isOpenPopUp;
        public bool IsOpenPopUp
        {
            get
            {
                return _isOpenPopUp;
            }
            set
            {
                _isOpenPopUp = value;
                OnPropertyChanged(nameof(IsOpenPopUp));
            }
        }

        private double _entrySize;

        public double EntrySize
        {
            get
            {
                return _entrySize;
            }
            set
            {
                _entrySize = value;
                OnPropertyChanged(nameof(EntrySize));
            }
        }

        private double _entryQuantity;

        public double EntryQuantity
        {
            get
            {
                return _entryQuantity;
            }
            set
            {
                _entryQuantity = value;
                OnPropertyChanged(nameof(EntryQuantity));
            }
        }

        private double _initialStop;

        public double InitialStop
        {
            get
            {
                return _initialStop;
            }


            set
            {
                _initialStop = value;
                OnPropertyChanged(nameof(InitialStop));
                CalculateEntry();
            }
        }

        private double _initialEntry;

        public double InitialEntry
        {
            get
            {
                return _initialEntry;
            }
            set
            {
                _initialEntry = value;
                OnPropertyChanged(nameof(InitialEntry));
                if (SelectedShortSlong == "Short")
                {
                    if (InitialEntry > InitialStop)
                    {
                        IsOpenPopUp = true;
                    }
                    else
                    {
                        IsOpenPopUp = false;
                    }

                    Limit = value * (1 - AcceptableSlippacePerc / 100);
                }
                else
                {
                    if (InitialEntry < InitialStop)
                    {
                        IsOpenPopUp = true;
                    }
                    else
                    {
                        IsOpenPopUp = false;
                    }
                    Limit = value * (1 + AcceptableSlippacePerc / 100);
                }
                CalculateEntry();
            }
        }

        private double _limit;
        public double Limit
        {
            get
            {
                return _limit;
            }
            set
            {
                _limit = value;
                OnPropertyChanged(nameof(Limit));
            }
        }
        #endregion

        #region row 2

        private double _oneToOneProfit;

        public double OneToOneProfit
        {
            get
            {
                return _oneToOneProfit;
            }
            set
            {
                _oneToOneProfit = value;
                OnPropertyChanged(nameof(OneToOneProfit));
            }
        }

        private double _posPerc;

        public double PosPerc
        {
            get
            {
                return _posPerc;
            }
            set
            {
                _posPerc = value;
                OnPropertyChanged(nameof(PosPerc));
            }
        }

        private double _accountPercPos;

        public double AccountPercPos
        {
            get
            {
                return _accountPercPos;
            }
            set
            {
                _accountPercPos = value;
                OnPropertyChanged(nameof(AccountPercPos));
            }
        }

        private double _bestTarget;

        public double BestTarget
        {
            get
            {
                return _bestTarget;
            }
            set
            {
                _bestTarget = value;
                OnPropertyChanged(nameof(BestTarget));
            }
        }

        private double _ocoQuantity;

        public double OCOQuantity
        {
            get
            {
                return _ocoQuantity;
            }
            set
            {
                _ocoQuantity = value;
                OnPropertyChanged(nameof(OCOQuantity));
            }
        }

        #endregion

        #region row 3

        private double _worstTarget;

        public double WorstTarget
        {
            get
            {
                return _worstTarget;
            }
            set
            {
                _worstTarget = value;
                OnPropertyChanged(nameof(WorstTarget));
            }
        }

        #endregion

        #endregion

        #region Events
        // event for the implementation of the interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor and Methods
        public TickerViewModel()
        {
            // initialize the list, the resource and the observable collection in the constructor. Start the connection and get the chart data.
            _tickersLock = new object();
            api = new FtxRestApi(client);
            Tickers = new ObservableCollection<TickersModel>();
            ChangeTimeFrameCommand = new ChangeTimeFrameCommand(this);
            MouseLeftButtonDownCommand = new MouseLeftButtonDownCommand(this);
            CalculatorComboBoxes = new CalculatorComboBoxes();
            OpenRaiseOrdersModalCommand = new OpenRaiseOrdersModalCommand();
            CloseRaiseOrdersModalCommand = new CloseRaiseOrdersModalCommand();
            RaiseOrdersCommand = new RaiseOrdersCommand(this);

            TickersCollectionView = CollectionViewSource.GetDefaultView(_tickers);
            // the member .Filter defines if a value should be displayed in the OC. If the function FilterTickers() returns true, the value can be displayed, otherwise it can't.
            TickersCollectionView.Filter = FilterTickers;

            ChartInfo = "BTC-PERP" + "    4H";
            _ = GetChartData("BTC-PERP", 14400);
            _ = StartWebSocket();
        }
        // method used for the search bar
        private bool FilterTickers(object obj)
        {
            if (obj is TickersModel tickers)
            {
                // the bool value is returned to .Filter member
                return tickers.Market.Contains(TickersFilter.ToUpper());
            }

            return false;
        }

        // method to implement the INotifyPropertyChanged interface
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Calculator Methods
        private void CalculateEntry()
        {
            if (SelectedShortSlong == "Short")
            {
                // calculate row 1
                if (SelectedMarginSpot == "Margin")
                {
                    if (AmountRiskInDollars / (InitialStop - Limit) * Limit > TradeAccountSize)
                    {
                        EntryQuantity = Math.Round(TradeAccountSize / Limit, 6);
                    }
                    else
                    {
                        EntryQuantity = Math.Round(AmountRiskInDollars / (Limit - InitialStop + (InitialStop * TradeFees * 2)), 6);
                    }
                }
                else
                {
                    MessageBox.Show("N/A");
                }

                // calculate row 3
                WorstTarget = Math.Round((2 * Limit) - InitialStop - 4 * Limit * TradeFees / (1 + TradeFees), 6);

                // calculate row 2
                OneToOneProfit = Math.Round((Limit * EntryQuantity) - (WorstTarget * EntryQuantity) - (Limit * EntryQuantity * TradeFees) +
                                            (WorstTarget * EntryQuantity * TradeFees), 2);

                BestTarget = Math.Round((2 * InitialEntry) - InitialStop - 4 * InitialEntry * TradeFees / (1 + TradeFees), 6);

            }
            else
            {
                // calculate row 1
                if (SelectedMarginSpot == "Margin")
                {
                    if (AmountRiskInDollars / (Limit - InitialStop) * Limit > TradeAccountSize)
                    {
                        EntryQuantity = Math.Round(TradeAccountSize / Limit, 6);
                    }
                    else
                    {
                        EntryQuantity = Math.Round(AmountRiskInDollars / (Limit - InitialStop + (InitialStop * TradeFees * 2)), 6);
                    }
                }
                else
                {
                    if (AmountRiskInDollars / (Limit - InitialStop) * Limit > (TradeAccountSize * ExchangePosSizeLimitInPerc))
                    {
                        EntryQuantity = Math.Round(TradeAccountSize * ExchangePosSizeLimitInPerc / Limit, 6);
                    }
                    else
                    {
                        EntryQuantity = Math.Round(AmountRiskInDollars / (Limit - InitialStop + (InitialStop * TradeFees * 2)), 6);
                    }
                }

                // calculate row 3

                WorstTarget = Math.Round((2 * Limit) - InitialStop + 4 * Limit * TradeFees / (1 - TradeFees), 6);

                // calculate row 2
                OneToOneProfit = Math.Round((WorstTarget * EntryQuantity) - (Limit * EntryQuantity) -
                                            ((Limit * EntryQuantity * TradeFees) + (WorstTarget * EntryQuantity * TradeFees)), 2);

                BestTarget = Math.Round((2 * InitialEntry) - InitialStop + 4 * InitialEntry * TradeFees / (1 - TradeFees), 6);


            }

            // below are the elements which are independent from short or long

            // row 1
            EntrySize = Math.Round(Limit * EntryQuantity, 2);

            // row 2
            PosPerc = Math.Round(OneToOneProfit / (EntryQuantity * Limit) * 100, 1);
            AccountPercPos = Math.Round(OneToOneProfit / TradeAccountSize * 100, 1);
            OCOQuantity = EntryQuantity * ScaleOut;

        }
        #endregion

        #region Raising Orders

        private SideType _sideType;
        public SideType SideType
        {
            get => _sideType;

            set
            {
                _sideType = value;
                OnPropertyChanged(nameof(SideType));
            }
        }

        #endregion

        #region Chart methods

        public async Task ChangeTimeFrame(string symbol, int timeFrameInSeconds)
        {
            await GetChartData(symbol, timeFrameInSeconds);
        }

        public async Task GetChartData(string symbol, int timeFrameInSeconds)
        {
            Candles = await LoadChart(symbol, timeFrameInSeconds);
        }
        public async Task<CandlesSource> LoadChart(string symbol, int timeFrameInSeconds)
        {
            Symbol = symbol;
            Dictionary<int, TimeFrame> DictionaryTimeFrames = new Dictionary<int, TimeFrame>
            {
                { 300, FancyCandles.TimeFrame.M5 },
                { 900, FancyCandles.TimeFrame.M15 },
                { 3600, FancyCandles.TimeFrame.H1 },
                { 14400, FancyCandles.TimeFrame.H4 },
                { 86400, FancyCandles.TimeFrame.Daily },
                { 604800, FancyCandles.TimeFrame.Weekly },
                { 86400 * 30, FancyCandles.TimeFrame.Monthly }
            };
            TimeFrame timeFrame = DictionaryTimeFrames[timeFrameInSeconds];
            Candles = new CandlesSource(timeFrame);

            ApiHistoricalDataResponse chartData = await GetChartDataAsync(symbol, timeFrameInSeconds);

            foreach (var res in chartData.Result)
            {
                int i = 0;
                DateTime t0 = res.StartTime;
                Candles.Add(new Candle(t0.AddMinutes(i * timeFrame.ToMinutes()),
                            res.Open, res.High, res.Low, res.Close, i));
            }

            return Candles;
        }
        private async Task<ApiHistoricalDataResponse> GetChartDataAsync(string market, int timeFrameInSeconds)
        {
            Uri endpoint = new Uri($"https://ftx.com/api/markets/" + market + "/candles?resolution=" + timeFrameInSeconds);
            HttpClient client = new HttpClient();

            string response = await Task.Run(() =>
            client.GetStringAsync(endpoint).Result);

            ApiHistoricalDataResponse chartData = JsonConvert.DeserializeObject<ApiHistoricalDataResponse>(response);

            await Task.Delay(1000);

            return chartData;

        }

        #endregion

        #region WebSocket methods
        public async Task StartWebSocket()
        {
            await StartConnection(this, client, api);
        }

        // one time event
        private async Task StartConnection(TickerViewModel wsApi, Client client, FtxRestApi api)
        {
            // get all futures data from the API
            var futures = await api.GetAllFuturesAsync();
            // parse the data
            ApiFuturesResponse all_futures = JsonConvert.DeserializeObject<ApiFuturesResponse>(futures);

            wsApi.OnWebSocketConnect += () =>
            {
                wsApi.SendCommand(FtxWebSocketRequestGenerator.GetAuthRequest(client));
                // go through the list with all futures

                foreach (ApiFuturesData future in all_futures.Result)
                {
                    // sort out the PERP contracts
                    if (future.Name.Contains("PERP"))
                    {
                        // use the names to subscribe to the ticker channel
                        wsApi.SendCommand(FtxWebSocketRequestGenerator.GetSubscribeRequest("ticker", future.Name));
                        // add the future in the OC
                        Tickers.Add(new TickersModel(future.Name));
                    }
                }
            };

            await wsApi.Connect();
        }

        public async Task Connect()
        {
            _webSocketClient = await CreateWebSocket(url);
            OnWebSocketConnect?.Invoke();
        }

        protected async Task<WebSocket> CreateWebSocket(string url)
        {
            var webSocket = new WebSocket(url);

            webSocket.Security.EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls | SslProtocols.Tls12;
            webSocket.EnableAutoSendPing = false;
            webSocket.Opened += WebsocketOnOpen;
            webSocket.Error += WebSocketOnError;
            webSocket.Closed += WebsocketOnClosed;
            webSocket.MessageReceived += WebsocketOnMessageReceive;
            webSocket.DataReceived += OnDataRecieved;
            await OpenConnection(webSocket);
            return webSocket;
        }

        protected async Task OpenConnection(WebSocket webSocket)
        {
            webSocket.Open();

            while (webSocket.State != WebSocketState.Open)
            {
                await Task.Delay(25);
            }
        }

        public async Task Stop()
        {
            _webSocketClient.Opened -= WebsocketOnOpen;
            _webSocketClient.Error -= WebSocketOnError;
            _webSocketClient.Closed -= WebsocketOnClosed;
            _webSocketClient.MessageReceived -= WebsocketOnMessageReceive;
            _webSocketClient.DataReceived -= OnDataRecieved;
            _webSocketClient?.Close();
            _webSocketClient?.Dispose();

            while (_webSocketClient.State != WebSocketState.Closed) await Task.Delay(25);
        }


        protected void WebsocketOnOpen(object sender, EventArgs e)
        {
            Status = "Connected";
        }

        protected void WebSocketOnError(object sender, ErrorEventArgs e)
        {
            Status = e.Exception.ToString();
        }

        protected void WebsocketOnClosed(object o, EventArgs e)
        {
            Status = e.ToString();
        }

        // continuous event
        public void WebsocketOnMessageReceive(object o, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            // on each new message update the FuturesResponse property which in it's setter updates the OC
            FuturesResponse = JsonConvert.DeserializeObject<WSFuturesResponse>(messageReceivedEventArgs.Message);
        }

        private void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            Status = e.Data.Length.ToString();
        }

        public void SendCommand(string command)
        {
            _webSocketClient?.Send(command);
        }

    }
    #endregion
}
