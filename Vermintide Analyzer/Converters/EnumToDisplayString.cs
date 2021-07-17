using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
