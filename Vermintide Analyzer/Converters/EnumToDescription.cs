using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using VA.LogReader;

namespace Vermintide_Analyzer.Converters
{
    public class EnumToDescription<T> : IValueConverter where T : Enum
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is T enumVal)
            {
                var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(enumVal.GetType().GetField(enumVal.ToString()), typeof(DescriptionAttribute));
                if(attr != null)
                {
                    return attr.Description;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class TraitToDescription : EnumToDescription<TRAIT> { }

}
