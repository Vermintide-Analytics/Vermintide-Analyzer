using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        #region Equipment (traits and properties)
        public ItemDetails Necklace { get; set; } = new ItemDetails();
        public ItemDetails Charm { get; set; } = new ItemDetails();
        public ItemDetails Trinket { get; set; } = new ItemDetails();
        public ItemDetails ChaosWastesProperties { get; set; } = new ItemDetails();
        #endregion

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

        public string MissionName => Mission.ForDisplay();
        public string MissionTooltip => Mission.GetAttributeOfType<DescriptionAttribute>()?.Description ?? "";

        public string CareerName => Career.ForDisplay();
        public string DifficultyName => Difficulty.ForDisplay();

        public string GameStartDate => GameStart.ToShortDateString();
        public string GameStartTime => GameStart.ToShortTimeString();

        public WeaponData StartingWeapon1 => Weapon1Datas.FirstOrDefault();
        public WeaponData StartingWeapon2 => Weapon2Datas.FirstOrDefault();
        public TalentTree StartingTalents => TalentTrees.FirstOrDefault();
        #endregion


        #region From File
        private const string EVENT_MARKER = "$";
        private const string EVENT_DELIMITER = "|";
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

            // Read header lines
            try
            {
                var headerDataRegex = new Regex(".*: (.*)");
                var versionRegex = new Regex("(\\d*)\\.(\\d*)");
                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (line.StartsWith(EVENT_MARKER))
                    {
                        // We've reached the end of the header data
                        break;
                    }

                    if (line.StartsWith("SCHEMA VERSION"))
                    {
                        var schemaVersion = headerDataRegex.Match(line).Groups[1].Value;
                        var versionMatch = versionRegex.Match(schemaVersion);
                        g.SchemaVersionMajor = byte.Parse(versionMatch.Groups[1].Value);
                        g.SchemaVersionMinor = byte.Parse(versionMatch.Groups[2].Value);
                    }
                    else if (line.StartsWith("GAME VERSION"))
                    {
                        var gameVersion = headerDataRegex.Match(line).Groups[1].Value;
                        var versionMatch = versionRegex.Match(gameVersion);
                        g.GameVersionMajor = byte.Parse(versionMatch.Groups[1].Value);
                        g.GameVersionMinor = byte.Parse(versionMatch.Groups[2].Value);
                    }
                    else if (line.StartsWith("DEATHWISH"))
                    {
                        var state = headerDataRegex.Match(line).Groups[1].Value;
                        g.Deathwish = state == "On";
                    }
                    else if (line.StartsWith("ONSLAUGHT"))
                    {
                        var state = headerDataRegex.Match(line).Groups[1].Value;
                        g.Onslaught = (ONSLAUGHT_TYPE)Enum.Parse(typeof(ONSLAUGHT_TYPE), state);
                    }
                    else if (line.StartsWith("EMPOWERED"))
                    {
                        var state = headerDataRegex.Match(line).Groups[1].Value;
                        g.Empowered = state == "On";
                    }
                }
            }
            catch
            {
                g.Error = ParseError.BadHeader;
                return g;
            }

            if (!g.ValidateSchemaVersion())
            {
                g.Error = ParseError.SchemaMismatch;
                return g;
            }

            // Read data lines
            var eventSplitArr = new string[] { EVENT_MARKER };
            var eventDetailSplitArr = new string[] { EVENT_DELIMITER };
            foreach(var line in File.ReadAllLines(filePath))
            {
                if(line.StartsWith(EVENT_MARKER))
                {
                    var eventStrings = line.Split(eventSplitArr, StringSplitOptions.RemoveEmptyEntries);
                    foreach(var eventString in eventStrings)
                    {
                        var eventDetails = eventString.Split(eventDetailSplitArr, StringSplitOptions.RemoveEmptyEntries);
                        // If we have an event broken down into more/less than 3 parts, it's not valid
                        if(eventDetails.Length == 3)
                        {
                            var newEvent = Event.CreateEvent(eventDetails[0], eventDetails[1], eventDetails[2]);
                            if (newEvent != null)
                            {
                                events.Add(newEvent);
                            }
                        }
                    }
                }
            }

            // Process events
            TrimToFirstRound(events);
            RemoveRedundantEvents(events);

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


            g.FillEquipmentData();
            g.FillWeaponData();
            g.FillTalentTrees();

            g.CalculateTotals();

            return g;
        }

        private static DateTime? StartTimeFromFileName(string fileName)
        {
            string dateTimeString = fileName.Replace(".VA", "").Split(' ').Last();
            if(DateTime.TryParseExact(dateTimeString, LOG_DATE_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
            {
                return result;
            }
            return null;
        }

        private bool ValidateSchemaVersion() =>
            SchemaVersionMajor <= Schema.SCHEMA_VERSION_MAJOR &&
            SchemaVersionMinor <= Schema.SCHEMA_VERSION_MINOR;

        private void ParseRoundStart(Round_Start evt)
        {
            Difficulty = evt.Difficulty;
            Career = evt.Career;
            Campaign = evt.Campaign;
            Mission = evt.Mission;
        }

        private void ParseRoundEnd(Round_End evt)
        {
            Result = evt.Result;
        }

        private void FillEquipmentData()
        {
            FillDataForItem(Necklace, TRAIT_SOURCE.Necklace, PROPERTY_SOURCE.Necklace);
            FillDataForItem(Charm, TRAIT_SOURCE.Charm, PROPERTY_SOURCE.Charm);
            FillDataForItem(Trinket, TRAIT_SOURCE.Trinket, PROPERTY_SOURCE.Trinket);
            FillDataForItem(ChaosWastesProperties, null, PROPERTY_SOURCE.ChaosWastes);

            void FillDataForItem(ItemDetails item, TRAIT_SOURCE? traitSource, PROPERTY_SOURCE? propertySource)
            {
                if(traitSource.HasValue)
                {
                    var traitEvents = Events.Where(evt => evt is Trait_Gained).Cast<Trait_Gained>().Where(tg => tg.Source == traitSource.Value);
                    foreach(var evt in traitEvents)
                    {
                        item.Traits.Add(evt.Trait);
                    }
                }
                if (propertySource.HasValue)
                {
                    var propertyEvents = Events.Where(evt => evt is Property_Gained).Cast<Property_Gained>().Where(pg => pg.Source == propertySource.Value);
                    foreach (var evt in propertyEvents)
                    {
                        item.Properties.Add(new Property(evt.Property, evt.PropertyValue));
                    }
                }
            }
        }

        private void FillWeaponData()
        {
            WeaponData currentWeapon1 = null;
            WeaponData currentWeapon2 = null;

            foreach(var evt in Events)
            {
                // Add every event as having happened while using the current weapons, INCLUDING the event which is a new weapon set (if applicable)
                if (currentWeapon1 != null)
                {
                    currentWeapon1.Events.Add(evt);
                }
                if (currentWeapon2 != null)
                {
                    currentWeapon2.Events.Add(evt);
                }

                if (evt is Weapon_Set weapons)
                {
                    var newWeapon1 = new WeaponData(weapons.Weapon1Owner, WEAPON_SLOT.Weapon1, weapons.Weapon1, weapons.Weapon1Rarity, weapons.Time);
                    var newWeapon2 = new WeaponData(weapons.Weapon2Owner, WEAPON_SLOT.Weapon2, weapons.Weapon2, weapons.Weapon2Rarity, weapons.Time);

                    currentWeapon1 = newWeapon1;
                    Weapon1Datas.Add(newWeapon1);

                    currentWeapon2 = newWeapon2;
                    Weapon2Datas.Add(newWeapon2);
                }
            }

            foreach(var weapon in Weapon1Datas)
            {
                weapon.CalculateItemDetails();
            }
            foreach (var weapon in Weapon2Datas)
            {
                weapon.CalculateItemDetails();
            }

            int count = 1;
            while(count < Weapon1Datas.Count)
            {
                if(Weapon1Datas[count-1].Equals(Weapon1Datas[count]))
                {
                    Weapon1Datas[count - 1].Events.AddRange(Weapon1Datas[count].Events);
                    Weapon1Datas.RemoveAt(count);
                }
                else
                {
                    count++;
                }
            }
            count = 1;
            while (count < Weapon2Datas.Count)
            {
                if (Weapon2Datas[count - 1].Equals(Weapon2Datas[count]))
                {
                    Weapon2Datas[count - 1].Events.AddRange(Weapon2Datas[count].Events);
                    Weapon2Datas.RemoveAt(count);
                }
                else
                {
                    count++;
                }
            }

            foreach (var weapon in Weapon1Datas)
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
            DurationMinutes = Duration / 60f;

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
            var start = events.FirstOrDefault(e => e is Round_Start);

            if (start != null)
            {
                var startIndex = events.IndexOf(start);
                events.RemoveRange(0, startIndex);
            }

            var end = events.FirstOrDefault(e => e is Round_End);

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
        #endregion
    }
}
