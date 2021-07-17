using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VA.LogReader;

namespace Vermintide_Analyzer.Statistics
{
    public class GameAverages
    {
        public double WinLossRatio { get; private set; } = double.NaN;

        [AverageFromGame(nameof(Game.DurationMinutes))]
        public double DurationMinutes { get; private set; } = 0;

        [AverageFromGame(nameof(Game.TimeAlivePercent))]
        public double TimeAlivePercent { get; private set; } = 0;
        [AverageFromGame(nameof(Game.TimeDownedPercent))]
        public double TimeDownedPercent { get; private set; } = 0;
        [AverageFromGame(nameof(Game.TimeDeadPercent))]
        public double TimeDeadPercent { get; private set; } = 0;

        [AverageFromGame(nameof(Game.DamagePerMin))]
        public double DamageDealtPerMin { get; private set; } = 0;
        [AverageFromGame(nameof(Game.MonsterDamagePerMin))]
        public double MonsterDamageDealtPerMin { get; private set; } = 0;
        [AverageFromGame(nameof(Game.AllyDamagePerMin))]
        public double AllyDamageDealtPerMin { get; private set; } = 0;

        [AverageFromGame(nameof(Game.OverkillDamagePerMin))]
        public double OverKillDamagePerMin { get; private set; } = 0;

        [AverageFromGame(nameof(Game.EnemiesKilledPerMin))]
        public double EnemiesKilledPerMin { get; private set; } = 0;
        [AverageFromGame(nameof(Game.ElitesKilledPerMin))]
        public double ElitesKilledPerMin { get; private set; } = 0;
        [AverageFromGame(nameof(Game.SpecialsKilledPerMin))]
        public double SpecialsKilledPerMin { get; private set; } = 0;

        [AverageFromGame(nameof(Game.StaggerPerMin))]
        public double StaggerDealtPerMin { get; private set; } = 0;

        [AverageFromGame(nameof(Game.DamageTakenPerMin))]
        public double DamageTakenPerMin { get; private set; } = 0;
        
        [AverageFromGame(nameof(Game.TimesDowned))]
        public double TimesDowned { get; private set; } = 0;
        [AverageFromGame(nameof(Game.TimesDied))]
        public double TimesDied { get; private set; } = 0;

        public GameAverages(List<Game> games)
        {
            foreach(var game in games)
            {
                // Make sure each game's stats have been calculated
                game.RecalculateStats();
            }

            IEnumerable<PropertyInfo> properties = typeof(GameAverages).GetProperties();
            properties = properties.Where(prop => prop.GetCustomAttribute(typeof(AverageFromGameAttribute)) != null);

            IEnumerable<(PropertyInfo propInfo, AverageFromGameAttribute attr)> propAttributes =
                properties.Select(prop => (prop, (AverageFromGameAttribute)prop.GetCustomAttribute(typeof(AverageFromGameAttribute))));

            int wins = 0;
            int losses = 0;

            int numGames = games.Count();
            foreach(var game in games)
            {
                if (game.Result.IsLoss()) losses++;
                else if (game.Result.IsWin()) wins++;

                foreach(var tuple in propAttributes)
                {
                    var currentValObj = tuple.propInfo.GetValue(this);
                    if(currentValObj is double currentVal)
                    {
                        tuple.propInfo.SetValue(this, currentVal + GetGameValue(game, tuple.attr.AccumulateFrom));
                    }
                }
            }

            WinLossRatio = (double)wins / losses;

            foreach(var prop in properties)
            {
                var currentValObj = prop.GetValue(this);
                if(currentValObj is double currentVal)
                {
                    prop.SetValue(this, currentVal / numGames);
                }
            }

            // Helper
            double GetGameValue(Game g, string propName)
            {
                var prop = typeof(Game).GetProperty(propName);
                var val = prop.GetValue(g);
                if(prop.PropertyType == typeof(int))
                {
                    return (int)val;
                }
                return (double)val;
            }
        }

        public int Compare(GameAverages other, string propName)
        {
            var propInfo = GetType().GetProperty(propName);
            double mine = (double)propInfo.GetValue(this);
            double others = (double)propInfo.GetValue(other);

            return mine.CompareTo(others);
        }
    }

    class AverageFromGameAttribute : Attribute
    {
        public string AccumulateFrom { get; }
        public AverageFromGameAttribute(string accumulate)
        {
            AccumulateFrom = accumulate;
        }
    }
}
