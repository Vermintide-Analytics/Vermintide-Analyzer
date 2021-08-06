using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class Game
    {

        public const string LOG_DATE_TIME_FORMAT = "yyyy-M-d_H-m-s";

        public const int HEADER_BYTES = 7;

        public string FilePath { get; set; }
        public DateTime GameStart { get; set; }

        #region Compiled Data
        public ParseError Error { get; private set; } = ParseError.None;

        public byte SchemaVersionMajor { get; private set; }
        public byte SchemaVersionMinor { get; private set; }

        public byte GameVersionMajor { get; private set; }
        public byte GameVersionMinor { get; private set; }

        public bool Deathwish { get; private set; }
        public ONSLAUGHT_TYPE Onslaught { get; private set; }
        public bool Empowered { get; private set; }

        public DIFFICULTY Difficulty { get; private set; }
        public CAREER Career { get; private set; }
        public CAMPAIGN Campaign { get; private set; }
        public MISSION Mission { get; private set; }


        public int EnemiesKilled { get; private set; }
        public int ElitesKilled { get; private set; }
        public int SpecialsKilled { get; private set; }
        public float DamageDealt { get; private set; }
        public float FriendlyFireDealt { get; private set; }
        public float DamageTaken { get; private set; }
        public ROUND_RESULT Result { get; private set; }



        public float Duration { get; private set; }
        public double DurationMinutes { get; set; }
        public float HighestDamage { get; private set; }

        public List<WeaponData> Weapon1Datas { get; private set; } = new List<WeaponData>();
        public List<WeaponData> Weapon2Datas { get; private set; } = new List<WeaponData>();

        public List<TalentTree> TalentTrees { get; private set; } = new List<TalentTree>();
        #endregion

        public List<Event> Events { get; private set; } = new List<Event>();

        public Round_Start RoundStart { get; private set; }
        public Round_End RoundEnd { get; private set; }

        #region For Display
        public string GameVersion => $"{GameVersionMajor}.{GameVersionMinor}";

        public string CampaignName => Campaign.ForDisplay();

        public string MissionName => Mission.ToString().Replace("_", " ");
        public string MissionTooltip => Mission.GetAttributeOfType<DescriptionAttribute>()?.Description ?? "";

        public string CareerName => Career.ForDisplay();
        public string DifficultyName => Difficulty.ForDisplay();

        public string GameStartDate => GameStart.ToShortDateString();
        public string GameStartTime => GameStart.ToShortTimeString();

        public WeaponData StartingWeapon1 => Weapon1Datas.FirstOrDefault();
        public WeaponData StartingWeapon2 => Weapon2Datas.FirstOrDefault();
        public TalentTree StartingTalents => TalentTrees.FirstOrDefault();
        #endregion

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
            Duration = Events.Last()?.Time ?? 0;
            DurationMinutes = Duration / 60;

            var damageEvents = Events.Where(e => e is Damage_Dealt).Cast<Damage_Dealt>();
            var numDamageEvents = damageEvents.Count();
            TotalDamage = damageEvents.Sum(e => e.Damage);
            DamagePerMin = TotalDamage / DurationMinutes;
            AvgDamage = TotalDamage / numDamageEvents;

            var monsterDamageEvents = damageEvents.Where(e => e.Target == DAMAGE_TARGET.Monster);
            var numMonsterDamageEvents = monsterDamageEvents.Count();
            TotalMonsterDamage = monsterDamageEvents.Sum(e => e.Damage);
            MonsterDamagePerMin = TotalMonsterDamage / DurationMinutes;
            AvgMonsterDamage = TotalMonsterDamage / numMonsterDamageEvents;

            var allyDamageEvents = damageEvents.Where(e => e.Target == DAMAGE_TARGET.Ally);
            var numAllyDamageEvents = allyDamageEvents.Count();
            TotalAllyDamage = allyDamageEvents.Sum(e => e.Damage);
            AllyDamagePerMin = TotalAllyDamage / DurationMinutes;
            AvgAllyDamage = TotalAllyDamage / numAllyDamageEvents;

            var staggerEvents = Events.Where(e => e is Enemy_Staggered).Cast<Enemy_Staggered>();
            var numStaggerEvents = staggerEvents.Count();
            TotalStagger = staggerEvents.Sum(e => e.StaggerDuration);
            StaggerPerMin = TotalStagger / DurationMinutes;
            AvgStagger = TotalAllyDamage / numStaggerEvents;

            var killEvents = Events.Where(e => e is Enemy_Killed).Cast<Enemy_Killed>();
            var eliteKillEvents = killEvents.Where(e => e.EnemyType == ENEMY_TYPE.Elite);
            var specialKillEvents = killEvents.Where(e => e.EnemyType == ENEMY_TYPE.Special);
            var numKillEvents = killEvents.Count();
            var numEliteKillEvents = eliteKillEvents.Count();
            var numSpecialKillEvents = specialKillEvents.Count();

            TotalEnemiesKilled = numKillEvents;
            TotalElitesKilled = numEliteKillEvents;
            TotalSpecialsKilled = numSpecialKillEvents;

            EnemiesKilledPerMin = TotalEnemiesKilled / DurationMinutes;
            ElitesKilledPerMin = TotalElitesKilled / DurationMinutes;
            SpecialsKilledPerMin = TotalSpecialsKilled / DurationMinutes;

            TotalOverkillDamage = killEvents.Sum(e => e.OverkillDamage);
            OverkillDamagePerMin = TotalOverkillDamage / DurationMinutes;
            AvgOverkillDamage = TotalOverkillDamage / numKillEvents;

            Headshots = damageEvents.Where(e => e.Headshot).Count();
            HeadshotsPerMin = Headshots / DurationMinutes;

            var damageTakenEvents = Events.Where(e => e is Damage_Taken).Cast<Damage_Taken>();
            TotalDamageTaken = damageTakenEvents.Sum(e => e.Damage);
            DamageTakenPerMin = TotalDamageTaken / DurationMinutes;
            FriendlyFireTaken = damageTakenEvents.Where(e => e.Source.IsFriendlyFire()).Sum(e => e.Damage);
            FriendlyFireTakenPerMin = FriendlyFireTaken / DurationMinutes;

            var tempHPGainedEvents = Events.Where(e => e is Temp_HP_Gained).Cast<Temp_HP_Gained>();
            TotalUncappedTempHPGained = tempHPGainedEvents.Sum(e => e.UncappedHeal);
            UncappedTempHPGainedPerMin = TotalUncappedTempHPGained / DurationMinutes;
            TotalCappedTempHPGained = tempHPGainedEvents.Sum(e => e.CappedHeal);
            CappedTempHPGainedPerMin = TotalCappedTempHPGained / DurationMinutes;

            CalculatePlayerStateTimes();
        }

        private void CalculatePlayerStateTimes()
        {
            TimesDied = 0;
            TimesDowned = 0;

            var playerStateEvents = Events.Where(e => e is Player_State).Cast<Player_State>();

            if (!playerStateEvents.Any())
            {
                // If the player state never changed, we assume they were alive the whole time
                TimeDeadPercent = 0;
                TimeDownedPercent = 0;
                TimeAlivePercent = 100;
                return;
            }

            float currentTime = 0;
            var currentState = PLAYER_STATE.Alive;

            double timeAlive = 0;
            double timeDowned = 0;
            double timeDead = 0;

            // Figure out the player's starting state (player can start a level dead in chaos wastes for example)
            if (playerStateEvents.First().State == PLAYER_STATE.Alive)
            {
                // First event is player being rescued, consider the initial state to be dead
                currentState = PLAYER_STATE.Dead;
            }

            // Accumulate times and states up until the last state event
            foreach (var stateEvent in playerStateEvents)
            {
                if (stateEvent.State == PLAYER_STATE.Downed) TimesDowned++;
                else if (stateEvent.State == PLAYER_STATE.Dead) TimesDied++;

                AddTime(stateEvent.Time);
                currentTime = stateEvent.Time;
                currentState = stateEvent.State;
            }

            // Add time between last state event and end of the game
            AddTime(Duration);

            // Calculate percentages
            TimeAlivePercent = 100 * timeAlive / Duration;
            TimeDownedPercent = 100 * timeDowned / Duration;
            TimeDeadPercent = 100 * timeDead / Duration;

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
        #endregion

        #region From File
        public static Game FromFile(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return null;
            }

            Game g = new Game() { FilePath = filePath };

            FileInfo fi = new FileInfo(filePath);
            g.GameStart = StartTimeFromFileName(fi.Name) ?? fi.CreationTime;

            List<Event> events = new List<Event>();

            using (FileStream reader = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[Event.BYTES];
                int lastReadCount = 0;

                // Read header, which is the first HEADER_BYTES bytes
                byte[] headerBuffer = new byte[HEADER_BYTES];
                lastReadCount = reader.Read(headerBuffer, 0, HEADER_BYTES);
                if(lastReadCount < HEADER_BYTES)
                {
                    g.Error = ParseError.BadHeader;
                    return g;
                }
                g.ParseHeader(headerBuffer);
                if(!g.ValidateSchemaVersion())
                {
                    g.Error = ParseError.SchemaMismatch;
                    return g;
                }

                lastReadCount = reader.Read(buffer, 0, Event.BYTES);
                while (lastReadCount == Event.BYTES)
                {
                    var eventType = Event.GetEventType(buffer[2]);
                    if (!eventType.HasValue)
                    {
                        Console.WriteLine($"Bad event type \"{buffer[2] >> Bitshift.EVENT_TYPE}\", skipping...");
                        continue;
                    }
                    var newEvent = Event.CreateEvent(eventType.Value, buffer);
                    if(newEvent != null)
                    {
                        events.Add(newEvent);
                    }
                    lastReadCount = reader.Read(buffer, 0, Event.BYTES);
                }

                // The data wasn't a multiple of Event.BYTES bytes, say something
                if (lastReadCount > 0)
                {
                    throw new InvalidDataException();
                }
            }

            // Process events
            TrimToFirstRound(events);
            RemoveRedundantEvents(events);
            SetTrueTimes(events);

            g.Events = events;

            g.RoundStart = g.Events.FirstOrDefault(e => e is Round_Start) as Round_Start;
            g.RoundEnd = g.Events.FirstOrDefault(e => e is Round_End) as Round_End;

            g.CalculateChartingValues();

            if(g.RoundStart is null)
            {
                g.Error = ParseError.NoStartEvent;
                return g;
            }
            g.ParseRoundStart(g.RoundStart);

            g.FillWeaponData();
            g.FillTalentTrees();

            g.CalculateTotals();

            return g;
        }

        private static DateTime? StartTimeFromFileName(string fileName)
        {
            string dateTimeString = fileName.Replace(".VA", "");
            if(DateTime.TryParseExact(dateTimeString, LOG_DATE_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
            {
                return result;
            }
            return null;
        }

        private void ParseHeader(byte[] bytes)
        {
            SchemaVersionMajor = bytes[0];
            SchemaVersionMinor = bytes[1];

            GameVersionMajor = bytes[2];
            GameVersionMinor = bytes[3];

            Deathwish = bytes[4] > 0;
            Onslaught = (ONSLAUGHT_TYPE)bytes[5];
            Empowered = bytes[6] > 0;
        }

        private bool ValidateSchemaVersion() =>
            SchemaVersionMajor == Schema.SCHEMA_VERSION_MAJOR &&
            SchemaVersionMinor == Schema.SCHEMA_VERSION_MINOR;

        private void ParseRoundStart(Round_Start evt)
        {
            Difficulty = evt.Difficulty;
            Career = evt.Career;
            Campaign = evt.Campaign;
            var shift = Campaign.MissionEnumShift();
            long adjustedMissionVal = evt.Mission << shift;

            if(Enum.IsDefined(typeof(MISSION), adjustedMissionVal))
            {
                Mission = (MISSION)adjustedMissionVal;
            }
            else
            {
                Mission = MISSION.Unknown;
            }
        }

        private void ParseRoundEnd(Round_End evt)
        {
            Result = evt.Result;
        }

        private void FillWeaponData()
        {
            WeaponData currentWeapon1 = null;
            WeaponData currentWeapon2 = null;
            foreach(var evt in Events)
            {
                if(evt is Weapon_Set weapons)
                {
                    if(currentWeapon1 is null || currentWeapon1.WeaponId != weapons.Weapon1)
                    {
                        currentWeapon1 = new WeaponData(Career.Hero(), WEAPON_SLOT.Weapon1, weapons.Weapon1, weapons.Time);
                        Weapon1Datas.Add(currentWeapon1);
                    }
                    if (currentWeapon2 is null || currentWeapon2.WeaponId != weapons.Weapon2)
                    {
                        currentWeapon2 = new WeaponData(Career.Hero(), WEAPON_SLOT.Weapon2, weapons.Weapon2, weapons.Time);
                        Weapon2Datas.Add(currentWeapon2);
                    }
                }
                else
                {
                    if(currentWeapon1 != null)
                    {
                        currentWeapon1.Events.Add(evt);
                    }
                    if(currentWeapon2 != null)
                    {
                        currentWeapon2.Events.Add(evt);
                    }
                }
            }

            foreach(var weapon in Weapon1Datas)
            {
                weapon.RecalculateStats();
            }
            foreach (var weapon in Weapon2Datas)
            {
                weapon.RecalculateStats();
            }
        }

        private void FillTalentTrees() => TalentTrees
            .AddRange(Events.Where(e => e is Talent_Tree)
            .Cast<Talent_Tree>()
            .Select(e => new TalentTree(e)));

        private void CalculateTotals()
        {
            foreach(var damageEvent in Events.Where(e => e is Damage_Dealt).Cast<Damage_Dealt>())
            {
                if(damageEvent.Target == DAMAGE_TARGET.Ally)
                {
                    FriendlyFireDealt += damageEvent.Damage;
                }
                else
                {
                    DamageDealt += damageEvent.Damage;
                }
            }
            foreach (var damageEvent in Events.Where(e => e is Damage_Taken).Cast<Damage_Taken>())
            {
                DamageTaken += damageEvent.Damage;
            }
            foreach (var killEvent in Events.Where(e => e is Enemy_Killed).Cast<Enemy_Killed>())
            {
                if(killEvent.EnemyType == ENEMY_TYPE.Elite)
                {
                    ElitesKilled++;
                }
                else if(killEvent.EnemyType == ENEMY_TYPE.Special)
                {
                    SpecialsKilled++;
                }
                EnemiesKilled++;
            }
        }

        private void CalculateChartingValues()
        {
            // Set duration
            if (RoundEnd is null)
            {
                Duration = Events.LastOrDefault()?.Time ?? 0;
                Result = ROUND_RESULT.Quit;
            }
            else
            {
                Duration = RoundEnd.Time;
                ParseRoundEnd(RoundEnd);
            }

            // Set highest damage
            foreach(var evt in Events)
            {
                if(evt is Damage_Dealt dDealt)
                {
                    HighestDamage = Math.Max(HighestDamage, dDealt.Damage);
                }
                else if(evt is Damage_Taken dTaken)
                {
                    HighestDamage = Math.Max(HighestDamage, dTaken.Damage);
                }
            }
        }

        private static void TrimToFirstRound(List<Event> events)
        {
            var start = events.FirstOrDefault(e => e.Type == EventType.Round_Start);

            if (start != null)
            {
                var startIndex = events.IndexOf(start);
                events.RemoveRange(0, startIndex);
            }

            var end = events.FirstOrDefault(e => e.Type == EventType.Round_End);

            if (end != null)
            {
                var endIndex = events.IndexOf(end);
                events.RemoveRange(endIndex + 1, events.Count - endIndex - 1);
            }
        }

        public static void RemoveRedundantEvents(List<Event> events)
        {
            var currentHealthEevents = events.Where(e => e is Current_Health).Cast<Current_Health>().ToArray();
            for(int count = 2; count < currentHealthEevents.Length; count++)
            {
                if(currentHealthEevents[count].PermanentHealth == currentHealthEevents[count-1].PermanentHealth &&
                    currentHealthEevents[count].PermanentHealth == currentHealthEevents[count-2].PermanentHealth &&
                    currentHealthEevents[count].TemporaryHealth == currentHealthEevents[count - 1].TemporaryHealth &&
                    currentHealthEevents[count].TemporaryHealth == currentHealthEevents[count - 2].TemporaryHealth)
                {
                    events.Remove(currentHealthEevents[count - 1]);
                }
            }
        }


        public const float TIME_ROLLOVER = 655.35f;
        public static void SetTrueTimes(List<Event> events)
        {
            float lastTime = 0;
            float largeScaleTime = 0;
            foreach (var e in events)
            {
                if (e.RawTime < lastTime)
                {
                    largeScaleTime += TIME_ROLLOVER;
                }

                e.Time = e.RawTime + largeScaleTime;
                lastTime = e.RawTime;
            }
        }
        #endregion
    }
}
