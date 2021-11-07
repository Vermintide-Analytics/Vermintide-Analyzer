using System;
using System.Globalization;
using System.Windows.Data;

namespace Vermintide_Analyzer.Converters
{
    public class FormatDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double d)
            {
                return d.ToString("F2");
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => double.Parse(value as string);
    }


    public class FormatPercent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return $"{d.ToString("F2")}%";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
