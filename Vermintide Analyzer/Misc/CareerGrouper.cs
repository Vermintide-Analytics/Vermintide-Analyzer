﻿using System.ComponentModel;
using System.Globalization;
using VA.LogReader;

namespace Vermintide_Analyzer.Misc
{
    public class CareerGrouper : GroupDescription
    {
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            if(item is string strVal)
            {
                CAREER career = strVal.FromDisplay<CAREER>();
                return career.Hero().ForDisplay();
            }

            return "";
        }
    }
}
