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
        public const string UNKNOWN_WEAPON_NAME = "Unknown Weapon";
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

        public static Dictionary<HERO, Dictionary<byte, string>> Weapons = new Dictionary<HERO, Dictionary<byte, string>>()
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

        public string WeaponName => WeaponId == UNKNOWN_WEAPON ?
            UNKNOWN_WEAPON_NAME :
            Weapons[Hero].ContainsKey(WeaponId) ?
                Weapons[Hero][WeaponId] :
                UNKNOWN_WEAPON_NAME;
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
        }

        public WeaponData GetHeaderData() =>
            new WeaponData(Hero, Slot, WeaponId, Rarity, StartTime);
        #endregion
    }

}
