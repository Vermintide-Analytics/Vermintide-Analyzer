using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VA.LogReader;

namespace Vermintide_Analyzer.Converters
{
    public class BoolToLongerShorter : IValueConverter
    {
        private const string LONGER = "Longer";
        private const string SHORTER = "Shorter";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                return b ? LONGER : SHORTER;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string str)
            {
                if (str == LONGER) return true;
                if (str == SHORTER) return false;
            }
            return null;
        }
    }
}
