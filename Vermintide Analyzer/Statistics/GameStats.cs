using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VA.LogReader;

namespace Vermintide_Analyzer.Statistics
{
    public class GameStats
    {
        public Game Game { get; set; }

        public float RangeFilterStart { get; set; } = float.MinValue;
        public float RangeFilterEnd { get; set; } = float.MaxValue;

        public GameStats(Game g)
        {
            Game = g;
        }

        #region Calculated
        #region Damage Dealt
        public double TotalDamage { get; private set; } = double.NaN;
        public double DamagePerMin { get; private set; } = double.NaN;
        public double TotalMonsterDamage { get; private set; } = double.NaN;
        public double MonsterDamagePerMin { get; private set; } = double.NaN;
        public double TotalAllyDamage { get; private set; } = double.NaN;
        public double AllyDamagePerMin { get; private set; } = double.NaN;

        public double AvgDamage { get; private set; } = double.NaN;
        public double AvgMonsterDamage { get; private set; } = double.NaN;
        public double AvgAllyDamage { get; private set; } = double.NaN;
        #endregion

        #region Stagger Dealt
        public double TotalStagger { get; private set; } = double.NaN;
        public double StaggerPerMin { get; private set; } = double.NaN;
        public double AvgStagger { get; private set; } = double.NaN;

        #endregion

        #region Enemies Killed
        public int TotalEnemiesKilled { get; private set; } = 0;
        public double EnemiesKilledPerMin { get; private set; } = double.NaN;
        public int TotalElitesKilled { get; private set; } = 0;
        public double ElitesKilledPerMin { get; private set; } = double.NaN;
        public int TotalSpecialsKilled { get; private set; } = 0;
        public double SpecialsKilledPerMin { get; private set; } = double.NaN;
        #endregion

        #region Overkill Damage
        public double TotalOverkillDamage { get; private set; } = double.NaN;
        public double OverkillDamagePerMin { get; private set; } = double.NaN;
        public double AvgOverkillDamage { get; private set; } = double.NaN;
        #endregion

        #region Headshots
        public int Headshots { get; private set; } = 0;
        public double HeadshotsPerMin { get; private set; } = double.NaN;
        #endregion

        #region Damage Taken
        public double TotalDamageTaken { get; private set; } = double.NaN;
        public double DamageTakenPerMin { get; private set; } = double.NaN;
        public double FriendlyFireTaken { get; private set; } = double.NaN;
        public double FriendlyFireTakenPerMin { get; private set; } = double.NaN;
        #endregion

        #region Temp HP Generated
        public double TotalUncappedTempHPGained { get; private set; } = double.NaN;
        public double UncappedTempHPGainedPerMin { get; private set; } = double.NaN;
        public double TotalCappedTempHPGained { get; private set; } = double.NaN;
        public double CappedTempHPGainedPerMin { get; private set; } = double.NaN;
        #endregion

        #region Player State
        public List<(float time, PLAYER_STATE state)> PlayerStateTimes { get; private set; } = new List<(float time, PLAYER_STATE state)>();
        public int TimesDowned { get; private set; } = 0;
        public int TimesDied { get; private set; } = 0;
        public double TimeDownedPercent { get; private set; } = double.NaN;
        public double TimeDeadPercent { get; private set; } = double.NaN;
        public double TimeAlivePercent { get; private set; } = double.NaN;
        #endregion
        #endregion

        #region Recalculate
        public void RecalculateStats()
        {
            var start = Math.Max(0, RangeFilterStart);
            var end = Math.Min(Game.Duration, RangeFilterEnd);

            var durationMinutes = Math.Max((end - start) / 60d, 0);

            var allEvents = Game.Events.Where(e => e.Time >= start && e.Time <= end);

            var damageEvents = allEvents.Where(e => e is Damage_Dealt).Cast<Damage_Dealt>();
            var numDamageEvents = damageEvents.Count();
            TotalDamage = damageEvents.Sum(e => e.Damage);
            DamagePerMin = TotalDamage / durationMinutes;
            AvgDamage = TotalDamage / numDamageEvents;

            var monsterDamageEvents = damageEvents.Where(e => e.Target == DAMAGE_TARGET.Monster);
            var numMonsterDamageEvents = monsterDamageEvents.Count();
            TotalMonsterDamage = monsterDamageEvents.Sum(e => e.Damage);
            MonsterDamagePerMin = TotalMonsterDamage / durationMinutes;
            AvgMonsterDamage = TotalMonsterDamage / numMonsterDamageEvents;

            var allyDamageEvents = damageEvents.Where(e => e.Target == DAMAGE_TARGET.Ally);
            var numAllyDamageEvents = allyDamageEvents.Count();
            TotalAllyDamage = allyDamageEvents.Sum(e => e.Damage);
            AllyDamagePerMin = TotalAllyDamage / durationMinutes;
            AvgAllyDamage = TotalAllyDamage / numAllyDamageEvents;

            var staggerEvents = allEvents.Where(e => e is Enemy_Staggered).Cast<Enemy_Staggered>();
            var numStaggerEvents = staggerEvents.Count();
            TotalStagger = staggerEvents.Sum(e => e.StaggerDuration);
            StaggerPerMin = TotalStagger / durationMinutes;
            AvgStagger = TotalAllyDamage / numStaggerEvents;

            var killEvents = allEvents.Where(e => e is Enemy_Killed).Cast<Enemy_Killed>();
            var eliteKillEvents = killEvents.Where(e => e.EnemyType == ENEMY_TYPE.Elite);
            var specialKillEvents = killEvents.Where(e => e.EnemyType == ENEMY_TYPE.Special);
            var numKillEvents = killEvents.Count();
            var numEliteKillEvents = eliteKillEvents.Count();
            var numSpecialKillEvents = specialKillEvents.Count();

            TotalEnemiesKilled = numKillEvents;
            TotalElitesKilled = numEliteKillEvents;
            TotalSpecialsKilled = numSpecialKillEvents;

            EnemiesKilledPerMin = TotalEnemiesKilled / durationMinutes;
            ElitesKilledPerMin = TotalElitesKilled / durationMinutes;
            SpecialsKilledPerMin = TotalSpecialsKilled / durationMinutes;

            TotalOverkillDamage = killEvents.Sum(e => e.OverkillDamage);
            OverkillDamagePerMin = TotalOverkillDamage / durationMinutes;
            AvgOverkillDamage = TotalOverkillDamage / numKillEvents;

            Headshots = damageEvents.Where(e => e.Headshot).Count();
            HeadshotsPerMin = Headshots / durationMinutes;

            var damageTakenEvents = allEvents.Where(e => e is Damage_Taken).Cast<Damage_Taken>();
            TotalDamageTaken = damageTakenEvents.Sum(e => e.Damage);
            DamageTakenPerMin = TotalDamageTaken / durationMinutes;
            FriendlyFireTaken = damageTakenEvents.Where(e => e.Source.IsFriendlyFire()).Sum(e => e.Damage);
            FriendlyFireTakenPerMin = FriendlyFireTaken / durationMinutes;

            var tempHPGainedEvents = allEvents.Where(e => e is Temp_HP_Gained).Cast<Temp_HP_Gained>();
            TotalUncappedTempHPGained = tempHPGainedEvents.Sum(e => e.UncappedHeal);
            UncappedTempHPGainedPerMin = TotalUncappedTempHPGained / durationMinutes;
            TotalCappedTempHPGained = tempHPGainedEvents.Sum(e => e.CappedHeal);
            CappedTempHPGainedPerMin = TotalCappedTempHPGained / durationMinutes;

            CalculatePlayerStateTimes(start, end);
        }

        private void CalculatePlayerStateTimes(float start, float end)
        {
            var allEvents = Game.Events.Where(e => e.Time >= start && e.Time <= end);

            TimesDied = 0;
            TimesDowned = 0;

            var playerStateEvents = allEvents.Where(e => e is Player_State).Cast<Player_State>();

            if (!playerStateEvents.Any())
            {
                // If the player state never changed, we assume they were alive the whole time
                TimeDeadPercent = 0;
                TimeDownedPercent = 0;
                TimeAlivePercent = 100;
                PlayerStateTimes.Add((0, PLAYER_STATE.Alive));
                return;
            }

            float currentTime = start;
            var currentState = PLAYER_STATE.Alive;

            double timeAlive = 0;
            double timeDowned = 0;
            double timeDead = 0;

            // Figure out the player's starting state (player can start a level dead in chaos wastes for example)
            if (playerStateEvents.First().State == PLAYER_STATE.Alive)
            {
                // First event is player being rescued, consider the initial state to be dead
                currentState = PLAYER_STATE.Dead;
                PlayerStateTimes.Add((0, PLAYER_STATE.Dead));
            }
            else
            {
                PlayerStateTimes.Add((0, PLAYER_STATE.Alive));
            }

            // Accumulate times and states up until the last state event
            foreach (var stateEvent in playerStateEvents)
            {
                if (stateEvent.State == PLAYER_STATE.Downed) TimesDowned++;
                else if (stateEvent.State == PLAYER_STATE.Dead) TimesDied++;

                AddTime(stateEvent.Time);
                currentTime = stateEvent.Time;
                currentState = stateEvent.State;
                PlayerStateTimes.Add((currentTime, currentState));
            }

            // Add time between last state event and end of the game
            AddTime(end);

            // Calculate percentages
            var duration = end - start;
            TimeAlivePercent = 100 * timeAlive / duration;
            TimeDownedPercent = 100 * timeDowned / duration;
            TimeDeadPercent = 100 * timeDead / duration;

            // Helper function
            void AddTime(float upToTime)
            {
                switch (currentState)
                {
                    case PLAYER_STATE.Alive:
                        timeAlive += upToTime - currentTime;
                        break;
                    case PLAYER_STATE.Downed:
                        timeDowned += upToTime - currentTime;
                        break;
                    case PLAYER_STATE.Dead:
                        timeDead += upToTime - currentTime;
                        break;
                }
            }
        }

        public PLAYER_STATE? GetPlayerStateAtTime(float time) =>
            PlayerStateTimes.Any() ? (PLAYER_STATE?)PlayerStateTimes.Last(st => st.time <= time).state : null;
        #endregion
    }
}
