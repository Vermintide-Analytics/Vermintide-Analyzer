using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using VA.LogReader;

namespace Vermintide_Analyzer.Converters
{
    public class RoundResultToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ROUND_RESULT result = (ROUND_RESULT)value;
            Color c = Color.FromRgb(45,45,45);
            switch (result)
            {
                case ROUND_RESULT.Loss:
                    c = Color.FromRgb(60, 25, 25);
                    break;
                case ROUND_RESULT.Win:
                    c = Color.FromRgb(20, 50, 20);
                    break;
            }

            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
