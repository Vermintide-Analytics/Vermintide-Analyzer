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
    public class NullableBoolToYesNoEither : IValueConverter
    {
        private const string YES = "Yes";
        private const string NO = "No";
        private const string EITHER = "Either";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool?)value;
            if (val.HasValue)
            {
                return val.Value ? YES : NO;
            }
            return EITHER;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            if (str == YES) return true;
            if (str == NO) return false;
            return null;
        }
    }
}
