using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class WeaponData
    {
        public const byte UNKNOWN_WEAPON = 63;
        #region Static Weapon Dictionaries
        public static Dictionary<byte, string> WeaponsMarkus = new Dictionary<byte, string>()
        {
            {0, "Mace"},
            {1, "Sword"},
            {2, "Two-Handed Hammer"},
            {3, "Two-Handed Sword"},
            {4, "Executioner Sword"},
            {5, "Halberd"},
            {6, "Mace & Shield"},
            {7, "Sword & Shield"},
            {8, "Mace & Sword"},
            {9, "Tuskgor Spear"},
            {10, "Brettonian Longsword"},
            {11, "Spear & Shield"},
            {12, "Brettonian Sword & Shield"},

            {32, "Blunderbuss" },
            {33, "Handgun" },
            {34, "Repeater Handgun" },
            {35, "Longbow" },
        };

        public static Dictionary<byte, string> WeaponsBardin = new Dictionary<byte, string>()
        {
            {0, "Axe"},
            {1, "Hammer"},
            {2, "Great Axe"},
            {3, "Two-Handed Hammer"},
            {4, "War Pick"},
            {5, "Axe & Shield"},
            {6, "Hammer & Shield"},
            {7, "Dual Hammers"},
            {8, "Dual Axes"},
            {9, "Cog Hammer"},

            {32, "Crossbow" },
            {33, "Drakefire Pistols" },
            {34, "Drakegun" },
            {35, "Handgun" },
            {36, "Grudge-Raker" },
            {37, "Throwing Axes" },
            {38, "Masterwork Pistol" },
            {39, "Trollhammer Torpedo" },
        };

        public static Dictionary<byte, string> WeaponsKerillian = new Dictionary<byte, string>()
        {
            {0, "Sword"},
            {1, "Glaive"},
            {2, "Two-Handed Sword"},
            {3, "Dual Daggers"},
            {4, "Sword & Dagger"},
            {5, "Dual Swords"},
            {6, "Spear"},
            {7, "Elven Axe"},
            {8, "Spear & Shield"},

            {32, "Longbow" },
            {33, "Swiftbow" },
            {34, "Hagbane Shortbow" },
            {35, "Moonfire Bow" },
            {36, "Javelin" },
            {37, "Volley Crossbow" },
            {38, "Deepwood Staff" },
        };

        public static Dictionary<byte, string> WeaponsVictor = new Dictionary<byte, string>()
        {
            {0, "Flail"},
            {1, "Axe"},
            {2, "Falchion"},
            {3, "Greatsword"},
            {4, "Rapier"},
            {5, "Axe & Falchion"},
            {6, "Billhook"},

            {32, "Brace of Pistols" },
            {33, "Crossbow" },
            {34, "Volley Crossbow" },
            {35, "Repeater Pistol" },
            {36, "Griffonfoot Pistols" },
        };

        public static Dictionary<byte, string> WeaponsSienna = new Dictionary<byte, string>()
        {
            {0, "Mace"},
            {1, "Dagger"},
            {2, "Fire Sword"},
            {3, "Sword"},
            {4, "Crowbill"},
            {5, "Flaming Flail"},

            {32, "Beam Staff" },
            {33, "Fireball Staff" },
            {34, "Flamestorm Staff" },
            {35, "Conflagration Staff" },
            {36, "Bolt Staff" },
            {37, "Coruscation Staff" },
        };

        public Dictionary<HERO, Dictionary<byte, string>> Weapons = new Dictionary<HERO, Dictionary<byte, string>>()
        {
            { HERO.Markus, WeaponsMarkus },
            { HERO.Bardin, WeaponsBardin },
            { HERO.Kerillian, WeaponsKerillian },
            { HERO.Victor, WeaponsVictor },
            { HERO.Sienna, WeaponsSienna },
        };
        #endregion

        #region Properties
        public HERO Hero { get; private set; }
        public byte WeaponId { get; private set; }
        public WEAPON_SLOT Slot { get; private set; }
        public float StartTime { get; set; }
        public double Duration { get; set; }
        public double DurationMinutes { get; set; }

        public string WeaponName => WeaponId == UNKNOWN_WEAPON ? "Unknown Weapon" : Weapons[Hero][WeaponId];
        public bool Ranged => WeaponId >= 32;

        public List<Event> Events { get; private set; } = new List<Event>();
        #endregion

        #region Constructor
        public WeaponData(HERO hero, WEAPON_SLOT slot, byte weaponId, float startTime)
        {
            Hero = hero;
            WeaponId = weaponId;
            Slot = slot;
            StartTime = startTime;
        }
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

        //#region Stagger Dealt
        //public double TotalStagger { get; private set; } = double.NaN;
        //public double StaggerPerMin { get; private set; } = double.NaN;
        //public double AvgStagger { get; private set; } = double.NaN;

        //#endregion

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
        public double HeadshotPercentage { get; private set; } = double.NaN;
        #endregion

        #region Crits
        public double AverageCritMultiplier { get; private set; } = double.NaN;
        public double CritPercentage { get; private set; } = double.NaN;
        #endregion
        #endregion

        #region Public Functions
        public void RecalculateStats()
        {
            Duration = Events.Last()?.Time - StartTime ?? 0;
            DurationMinutes = Duration / 60;

            var damageEvents = Events.Where(e => e is Damage_Dealt).Cast<Damage_Dealt>().Where(e => IsSourceThisWeapon(e.Source));
            var numDamageEvents = damageEvents.Count();
            TotalDamage = damageEvents.Sum(e => e.Damage);
            DamagePerMin = TotalDamage / DurationMinutes;
            AvgDamage = TotalDamage / numDamageEvents;

            var nonCritDamageEvents = damageEvents.Where(e => !e.Crit);
            var critDamageEvents = damageEvents.Where(e => e.Crit);
            if(nonCritDamageEvents.Any() && critDamageEvents.Any())
            {
                var averageNonCritDamage = nonCritDamageEvents.Average(e => e.Damage);
                var averageCritDamage = critDamageEvents.Average(e => e.Damage);
                AverageCritMultiplier = averageCritDamage / averageNonCritDamage;
            }
            var numCritDamageEvents = critDamageEvents.Count();
            CritPercentage = 100f * numCritDamageEvents / numDamageEvents;

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

            //var staggerEvents = Events.Where(e => e is Enemy_Staggered).Cast<Enemy_Staggered>().Where(e => IsSourceThisWeapon(e.Source));
            //var numStaggerEvents = staggerEvents.Count();
            //TotalStagger = staggerEvents.Sum(e => e.StaggerDuration);
            //StaggerPerMin = TotalStagger / DurationMinutes;
            //AvgStagger = TotalAllyDamage / numStaggerEvents;

            var killEvents = Events.Where(e => e is Enemy_Killed).Cast<Enemy_Killed>().Where(e => IsSourceThisWeapon(e.Source));
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
            HeadshotPercentage = 100f * (double)Headshots / numDamageEvents;
        }

        public WeaponData GetHeaderData() =>
            new WeaponData(Hero, Slot, WeaponId, StartTime);
        #endregion

        #region Helper Functions
        private bool IsSourceThisWeapon(DAMAGE_SOURCE source) => (byte)source == (byte)Slot;
        private bool IsSourceThisWeapon(STAGGER_SOURCE source) => (byte)source == (byte)Slot;
        #endregion
    }

}
