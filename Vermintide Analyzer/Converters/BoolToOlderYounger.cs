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
    public class BoolToOlderYounger : IValueConverter
    {
        private const string OLDER = "Older";
        private const string YOUNGER = "Younger";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                return b ? OLDER : YOUNGER;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string str)
            {
                if (str == OLDER) return true;
                if (str == YOUNGER) return false;
            }
            return null;
        }
    }
}
