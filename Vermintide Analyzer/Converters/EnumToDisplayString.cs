using System;
using System.Globalization;
using System.Windows.Data;
using VA.LogReader;

namespace Vermintide_Analyzer.Converters
{
    public class EnumToDisplayString<T> : IValueConverter where T : Enum
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => ((T)value).ForDisplay();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => ((string)value).FromDisplay<T>();
    }

    public class CareerToDisplayString : EnumToDisplayString<CAREER> { }
    public class CampaignToDisplayString : EnumToDisplayString<CAMPAIGN> { }
    public class TraitToDisplayString : EnumToDisplayString<TRAIT> { }
    public class PropertyToDisplayString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => ((Property)value).ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ParseErrorToDisplayString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => ((ParseError)value).ForDisplay().SplitCamelCase();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

}
