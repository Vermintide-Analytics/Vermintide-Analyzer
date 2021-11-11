using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Vermintide_Analyzer.Converters
{
    public class NetworkPingToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int)value;
            Color c =
                val > 175 ? Colors.Red :
                val > 125 ? Colors.Yellow:
                Colors.Green;

            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
