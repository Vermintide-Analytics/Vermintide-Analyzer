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
    public class GameToDifficultyModifierString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string output = "";
            if (value is Game g)
            {
                if (g.Deathwish) output += "Deathwish, ";
                if (g.Onslaught != ONSLAUGHT_TYPE.None) output += $"{Enum.GetName(typeof(ONSLAUGHT_TYPE), g.Onslaught)}, ";
                if (g.Empowered) output += "Empowered, ";
            }
            else if(value is GameHeader gh)
            {
                if (gh.Deathwish) output += "Deathwish, ";
                if (gh.Onslaught != ONSLAUGHT_TYPE.None) output += $"{Enum.GetName(typeof(ONSLAUGHT_TYPE), gh.Onslaught)}, ";
                if (gh.Empowered) output += "Empowered, ";
            }
            return output.Trim(',', ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
