using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FTXTradingClient.ViewModel.ValueConverters
{
    // not yet implemented
    public class BoolToButtonBackgroundConverter : IValueConverter
    {
        public Brush BuyBackground { get; set; }

        public Brush SellBackground { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string OrderSide = (string)value;

            if (OrderSide.Contains("Buy"))
            {
                return BuyBackground;
            }
            else
            {
                return SellBackground;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
