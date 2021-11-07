using System;
using VA.LogReader;

namespace Vermintide_Analyzer.Models
{
    public class WeaponTabItem
    {
        public WeaponData Weapon { get; set; }
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

        public WeaponTabItem(WeaponData weaponData)
        {
            Weapon = weaponData;
            StartTime = weaponData.StartTime;
        }
    }
}
