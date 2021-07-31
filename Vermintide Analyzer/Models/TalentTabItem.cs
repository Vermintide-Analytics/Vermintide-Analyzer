using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VA.LogReader;

namespace Vermintide_Analyzer.Models
{
    public class TalentTabItem
    {
        public CAREER Career { get; set; }
        public TalentTree Talents { get; set; }
        public float StartTime { get; set; }
        public bool IsFirst { get; set; } = false;
        public string Header
        {
            get
            {
                if (IsFirst) return "Start";
                var span = TimeSpan.FromSeconds(StartTime);
                return $"{span.Minutes}m{span.Seconds}s";
            }
        }
        public string Tooltip
        {
            get
            {
                if (IsFirst) return "As of the start of the round";
                var span = TimeSpan.FromSeconds(StartTime);
                return $"As of {span.Minutes} minutes, {span.Seconds} seconds in";
            }
        }

        public TalentTabItem(TalentTree talents, CAREER career)
        {
            Career = career;
            Talents = talents;
            StartTime = talents.StartTime;
        }
    }
}
