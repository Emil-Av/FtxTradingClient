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
    public class BoolToForegroundConverter : IValueConverter
    {
        // converter for the foreground color of the Price TextBlock. It is bounded to the static property LastPrice of the TickersModel

        public Brush HigherPrice { get; set; }

        public Brush LowerPrice { get; set; }

        public Brush SamePrice { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string LastPrice = (string)value;

            if (LastPrice == "higher")
            {
                SamePrice = HigherPrice;
                return HigherPrice;
            }
            else if (LastPrice == "lower")
            {
                SamePrice = LowerPrice;
                return LowerPrice;
            }
            else
            {
                return SamePrice;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
