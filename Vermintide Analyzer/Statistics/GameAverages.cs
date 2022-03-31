using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VA.LogReader;

namespace Vermintide_Analyzer.Statistics
{
    public class GameAverages
    {
        public double WinLossRatio { get; private set; } = double.NaN;

        [AverageFromGame(nameof(Game.DurationMinutes))]
        public double DurationMinutes { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.TimeAlivePercent))]
        public double TimeAlivePercent { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.TimeDownedPercent))]
        public double TimeDownedPercent { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.TimeDeadPercent))]
        public double TimeDeadPercent { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.DamagePerMin))]
        public double DamageDealtPerMin { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.MonsterDamagePerMin))]
        public double MonsterDamageDealtPerMin { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.AllyDamagePerMin))]
        public double AllyDamageDealtPerMin { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.OverkillDamagePerMin))]
        public double OverKillDamagePerMin { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.EnemiesKilledPerMin))]
        public double EnemiesKilledPerMin { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.ElitesKilledPerMin))]
        public double ElitesKilledPerMin { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.SpecialsKilledPerMin))]
        public double SpecialsKilledPerMin { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.StaggerLengthPerMin))]
        public double StaggerDealtPerMin { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.DamageTakenPerMin))]
        public double DamageTakenPerMin { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.UncappedTempHPGainedPerMin))]
        public double UncappedTempHPGainedPerMin { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.CappedTempHPGainedPerMin))]
        public double CappedTempHPGainedPerMin { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.TimesDowned))]
        public double TimesDowned { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.TimesDied))]
        public double TimesDied { get; private set; } = 0;

        [AverageFromGameStats(nameof(GameStats.HealingItemsApplied))]
        public double HealingItemsApplied { get; private set; } = 0;
        [AverageFromGameStats(nameof(GameStats.HealingClearedWounds))]
        public double HealingClearedWounds { get; private set; } = 0;

        public GameAverages(List<(Game game, GameStats stats)> games)
        {
            foreach(var tuple in games)
            {
                // Make sure each game's stats have been calculated
                tuple.stats.RecalculateStats();
            }

            IEnumerable<PropertyInfo> gameProperties = typeof(GameAverages).GetProperties();
            gameProperties = gameProperties.Where(prop => prop.GetCustomAttribute(typeof(AverageFromGameAttribute)) != null);

            IEnumerable<(PropertyInfo propInfo, AverageFromGameAttribute attr)> gamePropAttributes =
                gameProperties.Select(prop => (prop, (AverageFromGameAttribute)prop.GetCustomAttribute(typeof(AverageFromGameAttribute))));

            IEnumerable<PropertyInfo> statsProperties = typeof(GameAverages).GetProperties();
            statsProperties = statsProperties.Where(prop => prop.GetCustomAttribute(typeof(AverageFromGameStatsAttribute)) != null);

            IEnumerable<(PropertyInfo propInfo, AverageFromGameStatsAttribute attr)> statsPropAttributes =
                statsProperties.Select(prop => (prop, (AverageFromGameStatsAttribute)prop.GetCustomAttribute(typeof(AverageFromGameStatsAttribute))));

            int wins = 0;
            int losses = 0;

            int numGames = games.Count();
            foreach(var game in games)
            {
                if (game.game.Result.IsLoss()) losses++;
                else if (game.game.Result.IsWin()) wins++;

                foreach (var tuple in gamePropAttributes)
                {
                    var currentValObj = tuple.propInfo.GetValue(this);
                    if (currentValObj is double currentVal)
                    {
                        tuple.propInfo.SetValue(this, currentVal + GetGameValue(game.game, tuple.attr.AccumulateFrom));
                    }
                }

                foreach (var tuple in statsPropAttributes)
                {
                    var currentValObj = tuple.propInfo.GetValue(this);
                    if(currentValObj is double currentVal)
                    {
                        tuple.propInfo.SetValue(this, currentVal + GetStatsValue(game.stats, tuple.attr.AccumulateFrom));
                    }
                }
            }

            WinLossRatio = (double)wins / losses;

            foreach (var prop in gameProperties)
            {
                var currentValObj = prop.GetValue(this);
                if (currentValObj is double currentVal)
                {
                    prop.SetValue(this, currentVal / numGames);
                }
            }

            foreach (var prop in statsProperties)
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
                if (prop.PropertyType == typeof(int))
                {
                    return (int)val;
                }
                return (double)val;
            }
            double GetStatsValue(GameStats stats, string propName)
            {
                var prop = typeof(GameStats).GetProperty(propName);
                var val = prop.GetValue(stats);
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

    class AverageFromGameStatsAttribute : Attribute
    {
        public string AccumulateFrom { get; }
        public AverageFromGameStatsAttribute(string accumulate)
        {
            AccumulateFrom = accumulate;
        }
    }
}
