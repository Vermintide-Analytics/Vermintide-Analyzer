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
    public abstract class BoolToVisibility : IValueConverter
    {
        public abstract Visibility VisibilityIfTrue { get; }
        public abstract Visibility VisibilityIfFalse { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? VisibilityIfTrue : VisibilityIfFalse;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is Visibility v && v == VisibilityIfTrue;
    }

    public class TrueVisibleFalseCollapsed : BoolToVisibility
    {
        public override Visibility VisibilityIfTrue => Visibility.Visible;
        public override Visibility VisibilityIfFalse => Visibility.Collapsed;
    }
    public class TrueVisibleFalseHidden : BoolToVisibility
    {
        public override Visibility VisibilityIfTrue => Visibility.Visible;
        public override Visibility VisibilityIfFalse => Visibility.Hidden;
    }
    public class TrueCollapsedFalseVisible : BoolToVisibility
    {
        public override Visibility VisibilityIfTrue => Visibility.Collapsed;
        public override Visibility VisibilityIfFalse => Visibility.Visible;
    }
}
