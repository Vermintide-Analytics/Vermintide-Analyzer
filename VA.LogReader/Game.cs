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
                        lastReadCount = reader.Read(buffer, 0, Event.BYTES);
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


            g.FillEquipmentData();
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

        private void FillEquipmentData()
        {
            FillDataForItem(Necklace, TRAIT_SOURCE.Necklace, PROPERTY_SOURCE.Necklace);
            FillDataForItem(Charm, TRAIT_SOURCE.Charm, PROPERTY_SOURCE.Charm);
            FillDataForItem(Trinket, TRAIT_SOURCE.Trinket, PROPERTY_SOURCE.Trinket);
            FillDataForItem(ChaosWastesProperties, null, PROPERTY_SOURCE.Chaos_Wastes);

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
