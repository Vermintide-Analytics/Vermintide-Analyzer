using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VA.LogReader;

namespace Vermintide_Analyzer.Misc
{
    public class MissionGrouper : GroupDescription
    {
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            if(item is string strVal)
            {
                MISSION mission = strVal.FromDisplay<MISSION>();
                return mission.Campaign().ForDisplay();
            }

            return "";
        }
    }
}
