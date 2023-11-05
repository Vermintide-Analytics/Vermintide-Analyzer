using System.Collections.Generic;
using System.Linq;

namespace VA.LogReader
{
    public class WeaponData
    {
        public const byte UNKNOWN_WEAPON = 0;
        public const string UNKNOWN_WEAPON_NAME = "Unknown Weapon";
        #region Static Weapon Dictionaries
        public static Dictionary<WEAPON, string> WeaponsMarkus = new Dictionary<WEAPON, string>()
        {
            {WEAPON.es_1h_mace, "Mace"},
            {WEAPON.es_1h_sword, "Sword"},
            {WEAPON.es_2h_hammer, "Two-Handed Hammer"},
            {WEAPON.es_2h_sword, "Two-Handed Sword"},
            {WEAPON.es_2h_sword_executioner, "Executioner Sword"},
            {WEAPON.es_halberd, "Halberd"},
            {WEAPON.es_mace_shield, "Mace & Shield"},
            {WEAPON.es_sword_shield, "Sword & Shield"},
            {WEAPON.es_dual_wield_hammer_sword, "Mace & Sword"},
            {WEAPON.es_2h_heavy_spear, "Tuskgor Spear"},
            {WEAPON.es_bastard_sword, "Brettonian Longsword"},
            {WEAPON.es_deus_01, "Spear & Shield"},
            {WEAPON.es_sword_shield_breton, "Brettonian Sword & Shield"},
            
            {WEAPON.es_blunderbuss, "Blunderbuss" },
            {WEAPON.es_handgun, "Handgun" },
            {WEAPON.es_repeating_handgun, "Repeater Handgun" },
            {WEAPON.es_longbow, "Longbow" },
        };

        public static Dictionary<WEAPON, string> WeaponsBardin = new Dictionary<WEAPON, string>()
        {
            {WEAPON.dr_1h_axe, "Axe"},
            {WEAPON.dr_1h_hammer, "Hammer"},
            {WEAPON.dr_2h_axe, "Great Axe"},
            {WEAPON.dr_2h_hammer, "Two-Handed Hammer"},
            {WEAPON.dr_2h_pick, "War Pick"},
            {WEAPON.dr_shield_axe, "Axe & Shield"},
            {WEAPON.dr_shield_hammer, "Hammer & Shield"},
            {WEAPON.dr_dual_wield_hammers, "Dual Hammers"},
            {WEAPON.dr_dual_wield_axes, "Dual Axes"},
            {WEAPON.dr_2h_cog_hammer, "Cog Hammer"},

            {WEAPON.dr_crossbow, "Crossbow" },
            {WEAPON.dr_drake_pistol, "Drakefire Pistols" },
            {WEAPON.dr_drakegun, "Drakegun" },
            {WEAPON.dr_handgun, "Handgun" },
            {WEAPON.dr_rakegun, "Grudge-Raker" },
            {WEAPON.dr_1h_throwing_axes, "Throwing Axes" },
            {WEAPON.dr_steam_pistol, "Masterwork Pistol" },
            {WEAPON.dr_deus_01, "Trollhammer Torpedo" },
        };

        public static Dictionary<WEAPON, string> WeaponsKerillian = new Dictionary<WEAPON, string>()
        {
            {WEAPON.we_1h_sword, "Sword"},
            {WEAPON.we_2h_axe, "Glaive"},
            {WEAPON.we_2h_sword, "Two-Handed Sword"},
            {WEAPON.we_dual_wield_daggers, "Dual Daggers"},
            {WEAPON.we_dual_wield_sword_dagger, "Sword & Dagger"},
            {WEAPON.we_dual_wield_swords, "Dual Swords"},
            {WEAPON.we_spear, "Spear"},
            {WEAPON.we_1h_axe, "Elven Axe"},
            {WEAPON.we_1h_spears_shield, "Spear & Shield"},

            {WEAPON.we_longbow, "Longbow" },
            {WEAPON.we_shortbow, "Swiftbow" },
            {WEAPON.we_shortbow_hagbane, "Hagbane Shortbow" },
            {WEAPON.we_deus_01, "Moonfire Bow" },
            {WEAPON.we_javelin, "Javelin" },
            {WEAPON.we_crossbow_repeater, "Volley Crossbow" },
            {WEAPON.we_life_staff, "Deepwood Staff" },
        };

        public static Dictionary<WEAPON, string> WeaponsVictor = new Dictionary<WEAPON, string>()
        {
            {WEAPON.es_1h_flail, "Flail"},
            {WEAPON.wh_1h_axe, "Axe"},
            {WEAPON.wh_1h_falchion, "Falchion"},
            {WEAPON.wh_2h_sword, "Greatsword"},
            {WEAPON.wh_fencing_sword, "Rapier"},
            {WEAPON.wh_dual_wield_axe_falchion, "Axe & Falchion"},
            {WEAPON.wh_2h_billhook, "Billhook"},
            {WEAPON.wh_1h_hammer, "Hammer"},
            {WEAPON.wh_hammer_shield, "Hammer & Shield"},
            {WEAPON.wh_dual_hammer, "Dual Hammers"},
            {WEAPON.wh_2h_hammer, "Two-Handed Hammer"},
            {WEAPON.wh_flail_shield, "Flail & Shield"},
            {WEAPON.wh_hammer_book, "Hammer & Book"},

            {WEAPON.wh_brace_of_pistols, "Brace of Pistols" },
            {WEAPON.wh_crossbow, "Crossbow" },
            {WEAPON.wh_crossbow_repeater, "Volley Crossbow" },
            {WEAPON.wh_repeating_pistols, "Repeater Pistol" },
            {WEAPON.wh_deus_01, "Griffonfoot Pistols" },
        };

        public static Dictionary<WEAPON, string> WeaponsSienna = new Dictionary<WEAPON, string>()
        {
            {WEAPON.bw_1h_mace, "Mace"},
            {WEAPON.bw_dagger, "Dagger"},
            {WEAPON.bw_flame_sword, "Fire Sword"},
            {WEAPON.bw_sword, "Sword"},
            {WEAPON.bw_1h_crowbill, "Crowbill"},
            {WEAPON.bw_1h_flail_flaming, "Flaming Flail"},
            {WEAPON.bw_ghost_scythe, "Ensorcelled Reaper"},

            {WEAPON.bw_skullstaff_beam, "Beam Staff" },
            {WEAPON.bw_skullstaff_fireball, "Fireball Staff" },
            {WEAPON.bw_skullstaff_flamethrower, "Flamestorm Staff" },
            {WEAPON.bw_skullstaff_geiser, "Conflagration Staff" },
            {WEAPON.bw_skullstaff_spear, "Bolt Staff" },
            {WEAPON.bw_deus_01, "Coruscation Staff" },
            {WEAPON.bw_necromancy_staff, "Soulstealer Staff" },
        };

        public static Dictionary<HERO, Dictionary<WEAPON, string>> Weapons = new Dictionary<HERO, Dictionary<WEAPON, string>>()
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
        public WEAPON Weapon { get; private set; }
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

        public string WeaponName => Weapon == UNKNOWN_WEAPON ?
            UNKNOWN_WEAPON_NAME :
            Weapons[Hero].ContainsKey(Weapon) ?
                Weapons[Hero][Weapon] :
                UNKNOWN_WEAPON_NAME;

        public List<Event> Events { get; private set; } = new List<Event>();
        #endregion

        #region Constructor
        public WeaponData(HERO hero, WEAPON_SLOT slot, WEAPON weapon, RARITY rarity, float startTime)
        {
            Hero = hero;
            Weapon = weapon;
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
                if (Weapon != other.Weapon) return false;
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
            Duration = Events.LastOrDefault()?.Time - StartTime ?? 0;
            DurationMinutes = Duration / 60;
        }

        public WeaponData GetHeaderData() =>
            new WeaponData(Hero, Slot, Weapon, Rarity, StartTime);
        #endregion
    }

}
