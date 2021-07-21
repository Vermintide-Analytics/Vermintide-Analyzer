﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VA.LogReader;

namespace Vermintide_Analyzer.Converters
{
    public class WeaponToIcon : IValueConverter
    {
        private const string UNKNOWN_WEAPON = "/Images/Weapons/Unknown.png";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is WeaponData wd ? ( wd.WeaponId == WeaponData.UNKNOWN_WEAPON ? UNKNOWN_WEAPON : $"/Images/Weapons/{wd.Hero.Name()}/{wd.WeaponName}.png") : UNKNOWN_WEAPON;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
