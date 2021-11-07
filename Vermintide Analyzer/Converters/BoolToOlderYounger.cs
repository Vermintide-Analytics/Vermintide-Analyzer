using System;
using System.Globalization;
using System.Windows.Data;

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
