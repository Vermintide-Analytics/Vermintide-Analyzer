using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class WeaponData
    {
        public const byte UNKNOWN_WEAPON = 0;
        #region Static Weapon Dictionaries
        public static Dictionary<byte, string> WeaponsMarkus = new Dictionary<byte, string>()
        {
            {1, "Mace"},
            {2, "Sword"},
            {3, "Two-Handed Hammer"},
            {4, "Two-Handed Sword"},
            {5, "Executioner Sword"},
            {6, "Halberd"},
            {7, "Mace & Shield"},
            {8, "Sword & Shield"},
            {9, "Mace & Sword"},
            {10, "Tuskgor Spear"},
            {11, "Brettonian Longsword"},
            {12, "Spear & Shield"},
            {13, "Brettonian Sword & Shield"},

            {33, "Blunderbuss" },
            {34, "Handgun" },
            {35, "Repeater Handgun" },
            {36, "Longbow" },
        };

        public static Dictionary<byte, string> WeaponsBardin = new Dictionary<byte, string>()
        {
            {1, "Axe"},
            {2, "Hammer"},
            {3, "Great Axe"},
            {4, "Two-Handed Hammer"},
            {5, "War Pick"},
            {6, "Axe & Shield"},
            {7, "Hammer & Shield"},
            {8, "Dual Hammers"},
            {9, "Dual Axes"},
            {10, "Cog Hammer"},

            {33, "Crossbow" },
            {34, "Drakefire Pistols" },
            {35, "Drakegun" },
            {36, "Handgun" },
            {37, "Grudge-Raker" },
            {38, "Throwing Axes" },
            {39, "Masterwork Pistol" },
            {40, "Trollhammer Torpedo" },
        };

        public static Dictionary<byte, string> WeaponsKerillian = new Dictionary<byte, string>()
        {
            {1, "Sword"},
            {2, "Glaive"},
            {3, "Two-Handed Sword"},
            {4, "Dual Daggers"},
            {5, "Sword & Dagger"},
            {6, "Dual Swords"},
            {7, "Spear"},
            {8, "Elven Axe"},
            {9, "Spear & Shield"},

            {33, "Longbow" },
            {34, "Swiftbow" },
            {35, "Hagbane Shortbow" },
            {36, "Moonfire Bow" },
            {37, "Javelin" },
            {38, "Volley Crossbow" },
            {39, "Deepwood Staff" },
        };

        public static Dictionary<byte, string> WeaponsVictor = new Dictionary<byte, string>()
        {
            {1, "Flail"},
            {2, "Axe"},
            {3, "Falchion"},
            {4, "Greatsword"},
            {5, "Rapier"},
            {6, "Axe & Falchion"},
            {7, "Billhook"},

            {33, "Brace of Pistols" },
            {34, "Crossbow" },
            {35, "Volley Crossbow" },
            {36, "Repeater Pistol" },
            {37, "Griffonfoot Pistols" },
        };

        public static Dictionary<byte, string> WeaponsSienna = new Dictionary<byte, string>()
        {
            {1, "Mace"},
            {2, "Dagger"},
            {3, "Fire Sword"},
            {4, "Sword"},
            {5, "Crowbill"},
            {6, "Flaming Flail"},

            {33, "Beam Staff" },
            {34, "Fireball Staff" },
            {35, "Flamestorm Staff" },
            {36, "Conflagration Staff" },
            {37, "Bolt Staff" },
            {38, "Coruscation Staff" },
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
        public TRAIT_SOURCE TraitSource =>
            Slot == WEAPON_SLOT.Weapon1 ?
                TRAIT_SOURCE.Weapon1 :
                TRAIT_SOURCE.Weapon2;
        public PROPERTY_SOURCE PropertySource =>
            Slot == WEAPON_SLOT.Weapon1 ?
                PROPERTY_SOURCE.Weapon1 :
                PROPERTY_SOURCE.Weapon2;

        public RARITY Rarity { get; private set; }
        public ItemDetails ItemDetails { get; private set; }

        public float StartTime { get; set; }
        public double Duration { get; set; }
        public double DurationMinutes { get; set; }

        public string WeaponName => WeaponId == UNKNOWN_WEAPON ? "Unknown Weapon" : Weapons[Hero][WeaponId];
        public bool Ranged => WeaponId > 32;

        public List<Event> Events { get; private set; } = new List<Event>();
        #endregion

        #region Constructor
        public WeaponData(HERO hero, WEAPON_SLOT slot, byte weaponId, RARITY rarity, float startTime)
        {
            Hero = hero;
            WeaponId = weaponId;
            Slot = slot;
            Rarity = rarity;
            StartTime = startTime;
        }
        #endregion

        #region Override
        public override bool Equals(object obj)
        {
            if(obj is WeaponData other)
            {
                if (Hero != other.Hero) return false;
                if (WeaponId != other.WeaponId) return false;
                if (Slot != other.Slot) return false;
                if (Rarity != other.Rarity) return false;
                if (ItemDetails != null)
                {
                    if (!ItemDetails.Equals(other.ItemDetails)) return false;
                }
                else
                {
                    if (other.ItemDetails != null) return false;
                }

                return true;
            }
            return false;
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
        public void CalculateItemDetails()
        {
            ItemDetails = new ItemDetails();

            var traitEvents = Events.Where(evt => evt is Trait_Gained).Cast<Trait_Gained>().Where(tg => tg.Source == TraitSource);
            var propertyEvents = Events.Where(evt => evt is Property_Gained).Cast<Property_Gained>().Where(tg => tg.Source == PropertySource);

            foreach(var traitEvent in traitEvents)
            {
                ItemDetails.Traits.Add(traitEvent.Trait);
            }
            foreach(var propertyEvent in propertyEvents)
            {
                ItemDetails.Properties.Add(new Property(propertyEvent.Property, propertyEvent.PropertyValue));
            }


        }

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
            new WeaponData(Hero, Slot, WeaponId, Rarity, StartTime);
        #endregion

        #region Helper Functions
        private bool IsSourceThisWeapon(DAMAGE_SOURCE source) => (byte)source == (byte)Slot;
        private bool IsSourceThisWeapon(STAGGER_SOURCE source) => (byte)source == (byte)Slot;
        #endregion
    }

}
