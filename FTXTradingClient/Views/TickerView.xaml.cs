using FTXTradingClient.FTX;
using FTXTradingClient.FTX.Util;
using FTXTradingClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FTXTradingClient.Models;
using System.Net.Http;
using Newtonsoft.Json;
using FancyCandles;
using System.IO;

namespace FTXTradingClient.Views
{
    /// <summary>
    /// Interaktionslogik für TickerView.xaml
    /// </summary>
    public partial class TickerView : Window
    {
        public TickerView()
        {
            // start the view
            InitializeComponent();
            CandlesChart.LoadSettings(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\Assets\chartSettings.chs");
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
