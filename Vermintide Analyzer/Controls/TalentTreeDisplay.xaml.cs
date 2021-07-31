using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VA.LogReader;

namespace Vermintide_Analyzer.Controls
{
    /// <summary>
    /// Interaction logic for TalentTreeDisplay.xaml
    /// </summary>
    public partial class TalentTreeDisplay : UserControl
    {
        #region DP Props
        public CAREER Career
        {
            get { return (CAREER)GetValue(CareerProperty); }
            set { SetValue(CareerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Career.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CareerProperty =
            DependencyProperty.Register("Career", typeof(CAREER), typeof(TalentTreeDisplay), new PropertyMetadata(CAREER.Mercenary,
                (o, e) => { ((TalentTreeDisplay)o).UpdateDisplay(); }));

        public TalentTree Talents
        {
            get { return (TalentTree)GetValue(TalentsProperty); }
            set { SetValue(TalentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Talents.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TalentsProperty =
            DependencyProperty.Register("Talents", typeof(TalentTree), typeof(TalentTreeDisplay), new PropertyMetadata(null,
                (o, e) => { ((TalentTreeDisplay)o).UpdateDisplay(); }));
        #endregion

        #region Icon Paths
        public string IconRoot => $"/Images/Talents/{Career.ForDisplay()}";
        public string TalentIcon(int row, int column)
        {
            int talentNum = (row-1) * 3 + column;
            var path = $"{IconRoot}/talent-{talentNum:D2}.png";

            return path;
        }

        public string R1C1Icon => TalentIcon(1, 1);
        public string R1C2Icon => TalentIcon(1, 2);
        public string R1C3Icon => TalentIcon(1, 3);

        public string R2C1Icon => TalentIcon(2, 1);
        public string R2C2Icon => TalentIcon(2, 2);
        public string R2C3Icon => TalentIcon(2, 3);

        public string R3C1Icon => TalentIcon(3, 1);
        public string R3C2Icon => TalentIcon(3, 2);
        public string R3C3Icon => TalentIcon(3, 3);

        public string R4C1Icon => TalentIcon(4, 1);
        public string R4C2Icon => TalentIcon(4, 2);
        public string R4C3Icon => TalentIcon(4, 3);

        public string R5C1Icon => TalentIcon(5, 1);
        public string R5C2Icon => TalentIcon(5, 2);
        public string R5C3Icon => TalentIcon(5, 3);

        public string R6C1Icon => TalentIcon(6, 1);
        public string R6C2Icon => TalentIcon(6, 2);
        public string R6C3Icon => TalentIcon(6, 3);
        #endregion

        #region Talent Names and Descriptions
        public string TalentName(int row, int column) =>
            TalentNamesAndDescriptions[(Career, row, column)].name;
        public string TalentDescription(int row, int column) =>
            TalentNamesAndDescriptions[(Career, row, column)].description;

        public string R1C1Name => TalentName(1, 1);
        public string R1C2Name => TalentName(1, 2);
        public string R1C3Name => TalentName(1, 3);
                          
        public string R2C1Name => TalentName(2, 1);
        public string R2C2Name => TalentName(2, 2);
        public string R2C3Name => TalentName(2, 3);
                          
        public string R3C1Name => TalentName(3, 1);
        public string R3C2Name => TalentName(3, 2);
        public string R3C3Name => TalentName(3, 3);
                          
        public string R4C1Name => TalentName(4, 1);
        public string R4C2Name => TalentName(4, 2);
        public string R4C3Name => TalentName(4, 3);
                          
        public string R5C1Name => TalentName(5, 1);
        public string R5C2Name => TalentName(5, 2);
        public string R5C3Name => TalentName(5, 3);
                          
        public string R6C1Name => TalentName(6, 1);
        public string R6C2Name => TalentName(6, 2);
        public string R6C3Name => TalentName(6, 3);


        public string R1C1Description => TalentDescription(1, 1);
        public string R1C2Description => TalentDescription(1, 2);
        public string R1C3Description => TalentDescription(1, 3);

        public string R2C1Description => TalentDescription(2, 1);
        public string R2C2Description => TalentDescription(2, 2);
        public string R2C3Description => TalentDescription(2, 3);

        public string R3C1Description => TalentDescription(3, 1);
        public string R3C2Description => TalentDescription(3, 2);
        public string R3C3Description => TalentDescription(3, 3);

        public string R4C1Description => TalentDescription(4, 1);
        public string R4C2Description => TalentDescription(4, 2);
        public string R4C3Description => TalentDescription(4, 3);

        public string R5C1Description => TalentDescription(5, 1);
        public string R5C2Description => TalentDescription(5, 2);
        public string R5C3Description => TalentDescription(5, 3);

        public string R6C1Description => TalentDescription(6, 1);
        public string R6C2Description => TalentDescription(6, 2);
        public string R6C3Description => TalentDescription(6, 3);

        #region Dictionary
        public const string HIT_THP = "Damaging multiple enemies in one swing grants temporary health based on the number of targets hit. Max 5 enemies.";
        public const string STAGGER_THP = "Staggering an enemy with a melee attack grants Temporary Health. Health gained is based on the strength of the stagger.";
        public const string CRIT_HEADSHOT_THP = "Melee critical strikes and headshots grant 2 Temporary Health. Critical headshots restores twice as much.";
        public const string KILL_THP = "Melee killing blows grants temporary health based on the health of the slain enemy.";
        public const string HEAL_SHARE = "Healing yourself with a First Aid Kit or a Healing Draught also heals your nearby allies for 20% of their maximum health. Removes any Wounds.";

        public const string SMITER = "Smiter";
        public const string BULWARK = "Bulwark";
        public const string MAINSTAY = "Mainstay";
        public const string ASSASSIN = "Assassin";
        public const string ENHANCED_POWER = "Enhanced Power";

        public const string DESCRIPTION_SMITER = "The first enemy hit always counts as staggered. Deal 20% more damage to staggered enemies. Each hit against a staggered enemy adds another count of stagger. Bonus damage is increased to 40% against enemies afflicted by more than one stagger effect.";
        public const string DESCRIPTION_BULWARK = "Enemies that you stagger take 10% more damage from melee attacks for 2 seconds. Deal 20% more damage to staggered enemies. Each hit against a staggered enemy adds another count of stagger. Bonus damage is increased to 40% against enemies afflicted by more than one stagger effect.";
        public const string DESCRIPTION_MAINSTAY = "Deal 40% more damage to staggered enemies. Each hit against a staggered enemy adds another count of stagger. Bonus damage is increased to 60% against enemies afflicted by more than one stagger effect.";
        public const string DESCRIPTION_ASSASSIN = "Deal 20% more damage to staggered enemies. Each hit against a staggered enemy adds another count of stagger. Headshots and critical hits instead inflict 40% bonus damage, as do strikes against enemies afflicted by more than one stagger effect.";
        public const string DESCRIPTION_ENHANCED_POWER = "Increases total Power Level by 7.5%. This is calculated before other buffs are applied.";

        public static readonly Dictionary<(CAREER, int, int), (string name, string description)> TalentNamesAndDescriptions = new Dictionary<(CAREER, int, int), (string, string)>()
        {
            {(CAREER.Mercenary, 1, 1), ("Drillmaster", HIT_THP) },
            {(CAREER.Mercenary, 1, 2), ("Mercenary’s Pride", KILL_THP) },
            {(CAREER.Mercenary, 1, 3), ("Captain’s Command", HEAL_SHARE) },
            {(CAREER.Mercenary, 2, 1), ("The More The Merrier!", "Increases Power by 5% for every nearby enemy and stacks up to 5 times.") },
            {(CAREER.Mercenary, 2, 2), ("Limb-Splitter", "Increases cleave power by 50%.") },
            {(CAREER.Mercenary, 2, 3), ("Helborg’s Tutelage", "Every 5 hit grants a guaranteed critical strike. Critical strikes can no longer occur randomly.") },
            {(CAREER.Mercenary, 3, 1), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Mercenary, 3, 2), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Mercenary, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Mercenary, 4, 1), ("Reikland Reaper", "Increases Power by 15% when Paced Strikes is active.") },
            {(CAREER.Mercenary, 4, 2), ("Enhanced Training", "Paced Strikes increases attack speed by 20%. Now requires hitting 4 targets with a single attack to trigger.") },
            {(CAREER.Mercenary, 4, 3), ("Strike Together", "Paced Strikes spreads to nearby allies.") },
            {(CAREER.Mercenary, 5, 1), ("Stand Clear", "Increases Dodge range by 20%.") },
            {(CAREER.Mercenary, 5, 2), ("Blade Barrier", "Reduces damage taken by 25% when Paced Strikes is active.") },
            {(CAREER.Mercenary, 5, 3), ("Black Market Supplies", "Increases max Ammunition by 30%.") },
            {(CAREER.Mercenary, 6, 1), ("Walk it Off", "Morale Boost also reduces damage taken by effected allies by 25% for 10 seconds.") },
            {(CAREER.Mercenary, 6, 2), ("Ready for Action", "Reduces cooldown of Morale Boost by 20%.") },
            {(CAREER.Mercenary, 6, 3), ("On Yer Feet, Mates!", "Morale Boost also Revives knocked down allies.") },

            {(CAREER.Huntsman, 1, 1), ("Taste of Victory", STAGGER_THP) },
            {(CAREER.Huntsman, 1, 2), ("Huntsman’s Tally", KILL_THP) },
            {(CAREER.Huntsman, 1, 3), ("Taal’s Bounty", HEAL_SHARE) },
            {(CAREER.Huntsman, 2, 1), ("Keep it Coming", "Every third ranged hit causes the next shot to consume no ammo.") },
            {(CAREER.Huntsman, 2, 2), ("Make 'Em Bleed", "Critical hits cause enemies to take 20% increased damage for a short duration (15 seconds). Does not stack with similar effects.") },
            {(CAREER.Huntsman, 2, 3), ("One in the Eye", "Increased headshot bonus damage by 50%.") },
            {(CAREER.Huntsman, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Huntsman, 3, 2), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Huntsman, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Huntsman, 4, 1), ("Thrill of the Hunt", "Ranged headshots Increase reload speed by 20% for 5 seconds.") },
            {(CAREER.Huntsman, 4, 2), ("Makin’ It Look Easy", "After scoring a ranged headshot Markus gains 25% increased critical hit chance.") },
            {(CAREER.Huntsman, 4, 3), ("Burst of Enthusiasm", "Scoring a ranged headshot or critical strike grants 2 temporary health. Critical headshots doubles the effect. Effect can trigger once per attack.") },
            {(CAREER.Huntsman, 5, 1), ("Shot Crafter", "Killing a special restores 10% ammunition.") },
            {(CAREER.Huntsman, 5, 2), ("Tough Hide", "Killing a Special or Elite enemy reduces damage taken by 10%. Stacks 4 times. Taking a hit removes one stack.") },
            {(CAREER.Huntsman, 5, 3), ("Longshanks", "Increases movement speed by 10%.") },
            {(CAREER.Huntsman, 6, 1), ("Blend In", "Reduces the cooldown of Prowl by 30%.") },
            {(CAREER.Huntsman, 6, 2), ("Concealed Strikes", "Attacking while under the effect of Prowl does not break stealth.") },
            {(CAREER.Huntsman, 6, 3), ("Head Down and Hidden", "Increases the duration of Prowl to 10 seconds.") },

            {(CAREER.Foot_Knight, 1, 1), ("Back Off, Ugly!", STAGGER_THP) },
            {(CAREER.Foot_Knight, 1, 2), ("Bloody Unstoppable!", HIT_THP) },
            {(CAREER.Foot_Knight, 1, 3), ("Templar’s Rally", HEAL_SHARE) },
            {(CAREER.Foot_Knight, 2, 1), ("Staggering Force", "Increases stagger power by 35%.") },
            {(CAREER.Foot_Knight, 2, 2), ("Have at Thee!", "Staggering an elite enemy increases power by 15% for 10 seconds.") },
            {(CAREER.Foot_Knight, 2, 3), ("Crowd Clearer", "Pushing an enemy increases attack speed by 15% for 3 seconds.") },
            {(CAREER.Foot_Knight, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Foot_Knight, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Foot_Knight, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Foot_Knight, 4, 1), ("Rock of the Reikland", "Protective Presence's size is doubled and also grants 20% block cost reduction.") },
            {(CAREER.Foot_Knight, 4, 2), ("That's Bloody Teamwork!", "Increases damage reduction from Protective Presence by 5% for each nearby ally.") },
            {(CAREER.Foot_Knight, 4, 3), ("Comrades in Arms", "Kruber gains 10% power. The closest ally to Kruber gain 50% damage reduction and 10% increased power. Passive aura no longer affects allies.") },
            {(CAREER.Foot_Knight, 5, 1), ("It’s Hero Time", "Resets the cooldown of Charge when an ally is incapacitated.") },
            {(CAREER.Foot_Knight, 5, 2), ("Counter-Punch", "Blocking an attack removes the stamina cost of pushing for 1 second.") },
            {(CAREER.Foot_Knight, 5, 3), ("Inspire Action", "Staggering an elite enemy increases the cooldown generation rate of nearby allies by 100.0% for 0.5 seconds.") },
            {(CAREER.Foot_Knight, 6, 1), ("Numb to Pain", "Valiant Charge grants invulnerability for 3 seconds.") },
            {(CAREER.Foot_Knight, 6, 2), ("Battering Ram", "Doubles the width of Valiant Charge and allows Kruber to charge through great foes.") },
            {(CAREER.Foot_Knight, 6, 3), ("Bull of Ostland!", "Each enemy hit with Valiant Charge grants 3% attack speed for 10 seconds. Stacks up to 10 times.") },

            {(CAREER.Grail_Knight, 1, 1), ("Lady's Generosity", STAGGER_THP) },
            {(CAREER.Grail_Knight, 1, 2), ("Lady's Wrath", KILL_THP) },
            {(CAREER.Grail_Knight, 1, 3), ("Gift of the Grail", HEAL_SHARE) },
            {(CAREER.Grail_Knight, 2, 1), ("Virtue of the Ideal", "Killing enemies increases power level by 10% for 10 seconds. Stacks up to 3 times.") },
            {(CAREER.Grail_Knight, 2, 2), ("Virtue of Knightly Temper", "Critical strikes instantly slay enemies if their current health is less than 4 times the amount of damage of the critical strike. Half effect versus Lords and monsters.") },
            {(CAREER.Grail_Knight, 2, 3), ("Virtue of Heroism", "Power level of heavy attacks increased by 25%.") },
            {(CAREER.Grail_Knight, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Grail_Knight, 3, 2), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Grail_Knight, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Grail_Knight, 4, 1), ("Virtue of Duty", "The Lady's Favour grants an additional Quest.") },
            {(CAREER.Grail_Knight, 4, 2), ("Virtue of Purity", "Increases the potency of the blessings rewarded upon completing a Quest by 50%.") },
            {(CAREER.Grail_Knight, 4, 3), ("Virtue of the Penitent", "The Lady's Favour grants a repeatable Quest that rewards a Potion of Strength to Markus upon completion. (Potion drops on the floor if Markus's potion slot is occupied)") },
            {(CAREER.Grail_Knight, 5, 1), ("Virtue of Stoicism", "50% of damage taken is regenerated as temporary health after 5 seconds.") },
            {(CAREER.Grail_Knight, 5, 2), ("Virtue of Discipline", "Timed blocks increase power level by 20% for 6 seconds.") },
            {(CAREER.Grail_Knight, 5, 3), ("Virtue of the Joust", "Increases push arc and stamina regeneration by 30%.") },
            {(CAREER.Grail_Knight, 6, 1), ("Virtue of Audacity", "Adds a second stab attack to Blessed Blade, dealing devastating single target damage.") },
            {(CAREER.Grail_Knight, 6, 2), ("Virtue of the Impetuous Knight", "Killing an enemy with Blessed Blade increases movement speed by 35% for 15 seconds.") },
            {(CAREER.Grail_Knight, 6, 3), ("Virtue of Confidence", "Change Blessed Blade to a horizontal slash that Cleaves through and staggers multiple enemies.") },

            {(CAREER.Ranger_Veteran, 1, 1), ("Roots Running Deep", STAGGER_THP) },
            {(CAREER.Ranger_Veteran, 1, 2), ("Ranger Reaper", HIT_THP) },
            {(CAREER.Ranger_Veteran, 1, 3), ("Hardy Heart", HEAL_SHARE) },
            {(CAREER.Ranger_Veteran, 2, 1), ("Last Resort", "Bardin gains a 25% Power increase when out of ammunition.") },
            {(CAREER.Ranger_Veteran, 2, 2), ("Master of Improvisation", "Reloading a weapon reduces the cooldown of Disengage by 2 seconds.") },
            {(CAREER.Ranger_Veteran, 2, 3), ("Foe-Feller", "Increases attack speed by 5%.") },
            {(CAREER.Ranger_Veteran, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Ranger_Veteran, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Ranger_Veteran, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Ranger_Veteran, 4, 1), ("Field Rations", "Killing a special grants 50% chance to drop a bottle of ale. Ale grants 3% attack speed and reduced damage taken by 4% for 5 minutes when consumed. Can stack 3 times.") },
            {(CAREER.Ranger_Veteran, 4, 2), ("Grugni’s Cunning", "Increases ammunition restored by Survivalist caches to 30%.") },
            {(CAREER.Ranger_Veteran, 4, 3), ("Scavenger", "Killing a special has a 20% chance to drop a Potion or Bomb instead of a Survivalist cache.") },
            {(CAREER.Ranger_Veteran, 5, 1), ("No Dawdling!", "Increases movement speed by 10%.") },
            {(CAREER.Ranger_Veteran, 5, 2), ("Exuberance", "Bardin takes 30% less damage from behind. Headshots grant this bonus to all damage for 5 seconds.") },
            {(CAREER.Ranger_Veteran, 5, 3), ("Firing Fury", "Hitting 2 enemies with one ranged attack increases speed of Bardin's next reload by 35%.") },
            {(CAREER.Ranger_Veteran, 6, 1), ("Exhilarating Vapours", "Allies inside Bardin's smoke gain 8% attack speed and generate 3 temporary health every second.") },
            {(CAREER.Ranger_Veteran, 6, 2), ("Surprise Guest", "Disengage's stealth does not break on moving beyond the smoke cloud.") },
            {(CAREER.Ranger_Veteran, 6, 3), ("Ranger’s Parting Gift", "Activating Disengage causes the next bomb Bardin throws within the duration of the ability to not be consumed.") },

            {(CAREER.Ironbreaker, 1, 1), ("Rock-Breaker", STAGGER_THP) },
            {(CAREER.Ironbreaker, 1, 2), ("Grudge-Borne", KILL_THP) },
            {(CAREER.Ironbreaker, 1, 3), ("Hearthguard", HEAL_SHARE) },
            {(CAREER.Ironbreaker, 2, 1), ("Rising Pressure", "Drake Fire damage increases from -80% to 120% and ranged attack speed reduces from 100% to 150% depending on overcharge. Removes overcharge slowdown.") },
            {(CAREER.Ironbreaker, 2, 2), ("Blood of Grimnir", "Each nearby ally increases power by 5%.") },
            {(CAREER.Ironbreaker, 2, 3), ("Rune-Etched Shield", "Blocking an attack increases the power of Bardin and his nearby allies by 2% for 10 seconds. Stacks up to 5 times.") },
            {(CAREER.Ironbreaker, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Ironbreaker, 3, 2), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Ironbreaker, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Ironbreaker, 4, 1), ("Vengeance", "Periodically generate stacks of Rising Anger every 7 seconds while Gromril is active. When Gromril is removed gain 10% attack speed per stack of Rising Anger.") },
            {(CAREER.Ironbreaker, 4, 2), ("Gromril Curse", "When Gromril Armour is removed all nearby enemies are knocked back.") },
            {(CAREER.Ironbreaker, 4, 3), ("Tunnel Fighter", "Reduces the cooldown of Gromril Armour to 10 seconds.") },
            {(CAREER.Ironbreaker, 5, 1), ("Dawi Defiance", "When Bardins guard is broken there is a 50% chance to instantly restore all stamina.") },
            {(CAREER.Ironbreaker, 5, 2), ("The Rolling Mountain", "Killing enemies with melee attacks while on full stamina reduces the cooldown of Impenetrable by 2%.") },
            {(CAREER.Ironbreaker, 5, 3), ("Miner’s Rythm", "After landing a charged attack Bardin recovers stamina 40% faster for 2 seconds.") },
            {(CAREER.Ironbreaker, 6, 1), ("Drengbarazi Oath", "Impenetrable increases power of nearby allies by 20% for 10 seconds.") },
            {(CAREER.Ironbreaker, 6, 2), ("Oi! Wazzok!", "Impenetrable taunt now forces monsters to attack Bardin.") },
            {(CAREER.Ironbreaker, 6, 3), ("Booming Taunt", "Increases the radius of Impenetrables taunt by 15%. Increases the duration of Impenetrable to 15 seconds.") },

            {(CAREER.Slayer, 1, 1), ("Doomseeker", HIT_THP) },
            {(CAREER.Slayer, 1, 2), ("Slayer’s Fury", KILL_THP) },
            {(CAREER.Slayer, 1, 3), ("Infectious Fortitude", HEAL_SHARE) },
            {(CAREER.Slayer, 2, 1), ("A Thousand Cuts", "Wielding one-handed weapons in both slots increases attack speed by 10%. Dual weapons count as one-handed. (Throwing Axes does not count as one-handed)") },
            {(CAREER.Slayer, 2, 2), ("Skull-Splitter", "Wielding two-handed weapons in both slots increases power by 15%.(Throwing Axes does not count as two-handed)") },
            {(CAREER.Slayer, 2, 3), ("Hack and Slash", "Increases critical hit chance by 5%.") },
            {(CAREER.Slayer, 3, 1), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Slayer, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Slayer, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Slayer, 4, 1), ("Impatience", "Each stack of Trophy Hunter increases movement speed by 10%.") },
            {(CAREER.Slayer, 4, 2), ("High Tally", "Increases maximum stacks of Trophy Hunter by 1.") },
            {(CAREER.Slayer, 4, 3), ("Adrenaline Surge", "On max stacks, Trophy Hunter grants cooldown reduction for Leap.") },
            {(CAREER.Slayer, 5, 1), ("Oblivious to Pain", "Damage taken from Elite enemies or Monsters is reduced to 10 damage or half of its original value whichever is highest.") },
            {(CAREER.Slayer, 5, 2), ("Grimnir’s Focus", "Hitting an enemy with a charged attack reduces damage taken by 40% for 5 seconds.") },
            {(CAREER.Slayer, 5, 3), ("Barge", "Effective dodges pushes nearby small enemies out of the way.") },
            {(CAREER.Slayer, 6, 1), ("Crunch", "Increases stagger effect when landing on enemies using Leap by 100%") },
            {(CAREER.Slayer, 6, 2), ("Dawi-Drop", "Increases attack damage while airborne during Leap by 150%.") },
            {(CAREER.Slayer, 6, 3), ("No Escape", "Leap’s attack speed buff also increases movement speed by 25% for the duration.") },

            {(CAREER.Outcast_Engineer, 1, 1), ("Engineer's Resolve", STAGGER_THP) },
            {(CAREER.Outcast_Engineer, 1, 2), ("Morgrim's Enthusiasm", HIT_THP) },
            {(CAREER.Outcast_Engineer, 1, 3), ("Pouch of the Good Stuff", HEAL_SHARE) },
            {(CAREER.Outcast_Engineer, 2, 1), ("Leading Shots", "Every 4 Ranged Attack is a guaranteed Critical Hit.") },
            {(CAREER.Outcast_Engineer, 2, 2), ("Armour Piercing Slugs", "Ranged Attacks pierce 1 additional enemy.") },
            {(CAREER.Outcast_Engineer, 2, 3), ("Scavenged Shot", "Melee Power is increased by 10%. Every 5 Melee kill makes Bardin's next Ranged Attack consume no Ammo.") },
            {(CAREER.Outcast_Engineer, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Outcast_Engineer, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Outcast_Engineer, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Outcast_Engineer, 4, 1), ("Full Head of Steam", "Upon reaching 5 stacks of Pressure, Bardin gains 15.0% Power for 10 seconds.") },
            {(CAREER.Outcast_Engineer, 4, 2), ("Experimental Steam Capacitors", "Pressure stacks are no longer removed upon reaching full charge or firing the Steam-Assisted Crank Gun (Mk II).") },
            {(CAREER.Outcast_Engineer, 4, 3), ("Superior Gaskets", "Pressure is not lost over time. Each stack of Pressure grants 2.5% Attack Speed. Max stacks reduced by 1.") },
            {(CAREER.Outcast_Engineer, 5, 1), ("Ablative Armour", "Every 5 seconds Bardin gains a stack of Damage Reduction. Stacks up to 5 times. Each stack reduces damage taken by 5%. Taking damage removes a stack.") },
            {(CAREER.Outcast_Engineer, 5, 2), ("Bombardier", "Bardin's Bombs gain the effect of both regular Bombs and Incendiary Bombs.") },
            {(CAREER.Outcast_Engineer, 5, 3), ("Piston Power", "Every 15 seconds Bardin gains a buff that grants immense Stagger to his next charged attack.") },
            {(CAREER.Outcast_Engineer, 6, 1), ("Gromril-Plated Shot", "Reduces the Steam-Assisted Crank Gun (Mk II)'s rate of fire, but increases Damage and Armour Penetration.") },
            {(CAREER.Outcast_Engineer, 6, 2), ("Linked Compression Chamber", "The Steam-Assisted Crank Gun (Mk II) starts firing at full speed, rather than taking time to ramp up.") },
            {(CAREER.Outcast_Engineer, 6, 3), ("Innovative Ammo Hoppers", "Increases Bardin's Ability Bar by 50%. Killing a Special makes the Steam-Assisted Crank Gun (Mk II) not consume the Ability Bar for the next 4 seconds.") },

            {(CAREER.Waystalker, 1, 1), ("Weavebond", CRIT_HEADSHOT_THP) },
            {(CAREER.Waystalker, 1, 2), ("Dryad’s Thirst", HIT_THP) },
            {(CAREER.Waystalker, 1, 3), ("Ariel’s Boon", HEAL_SHARE) },
            {(CAREER.Waystalker, 2, 1), ("Blood Shot", "After killing an enemy with a melee attack Kerillian fires an additional arrow with her next ranged attack made within 10 seconds. (The second arrow does not consume ammo.)") },
            {(CAREER.Waystalker, 2, 2), ("Serrated Shots", "Enemies hit by non-poisonous ranged attacks bleed for extra damage. (Instances of this bleed do stack)") },
            {(CAREER.Waystalker, 2, 3), ("Drakira’s Alacrity", "Ranged headshots increases attack speed by 15% for 5 seconds.") },
            {(CAREER.Waystalker, 3, 1), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Waystalker, 3, 2), (ASSASSIN, DESCRIPTION_ASSASSIN) },
            {(CAREER.Waystalker, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Waystalker, 4, 1), ("Isha’s Embrace", "Increases Kerillian’s health regenerated from Amaranthe by 50%.") },
            {(CAREER.Waystalker, 4, 2), ("Spirit Arrows", "Amaranthe reduces the cooldown of Trueflight Volley by 5% every tick. No longer restores health.") },
            {(CAREER.Waystalker, 4, 3), ("Rejuvenating Locus", "Amaranthe also affects the other members of the party.") },
            {(CAREER.Waystalker, 5, 1), ("Fervent Huntress", "Killing a special or elite increases movement speed by 15% for 10 seconds.") },
            {(CAREER.Waystalker, 5, 2), ("Ricochet", "Kerillians arrows now richochet, bouncing up to 3 times or until it hits an enemy.") },
            {(CAREER.Waystalker, 5, 3), ("Asrai Focus", "Reduces the cooldown of Trueflight Volley by 20%.") },
            {(CAREER.Waystalker, 6, 1), ("Piercing Shot", "Trueflight Volley fires one piercing shot dealing heavy damage. Headshot refunds 100% cooldown.") },
            {(CAREER.Waystalker, 6, 2), ("Loaded Bow", "Trueflight Volley fires an additional arrow.") },
            {(CAREER.Waystalker, 6, 3), ("Kurnous’ Reward", "Killing a special or elite enemy with Trueflight Volley restores 30% ammunition.") },

            {(CAREER.Handmaiden, 1, 1), ("Spirit Echo", HIT_THP) },
            {(CAREER.Handmaiden, 1, 2), ("Martial Blessing", KILL_THP) },
            {(CAREER.Handmaiden, 1, 3), ("Eternal Blossom", HEAL_SHARE) },
            {(CAREER.Handmaiden, 2, 1), ("Focused Spirit", "After not taking damage for 10 seconds, increases Kerillians power by 15%. Reset upon taking damage.") },
            {(CAREER.Handmaiden, 2, 2), ("Oak Stance", "Increases critical strike chance by 5%.") },
            {(CAREER.Handmaiden, 2, 3), ("Asrai Alacrity", "Pushing or blocking an attack grants the next two strikes 30% attacks speed and 10% power.") },
            {(CAREER.Handmaiden, 3, 1), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Handmaiden, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Handmaiden, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Handmaiden, 4, 1), ("Willow Stance", "Dodging grants 5% attack speed for 6 seconds. Stacks up to 3 times.") },
            {(CAREER.Handmaiden, 4, 2), ("Dance of Blades", "Dodging while blocking increases dodge range by 20%. Dodging while not blocking increase the power 10% for 1 second.") },
            {(CAREER.Handmaiden, 4, 3), ("Wraith-Walk", "Kerillians dodges can now pass through enemies.") },
            {(CAREER.Handmaiden, 5, 1), ("Heart of Oak", "Increases max health by 15%.") },
            {(CAREER.Handmaiden, 5, 2), ("Birch Stance", "Reduces block cost by 30%.") },
            {(CAREER.Handmaiden, 5, 3), ("Quiver of Plenty", "Increases ammunition amount by 40%.") },
            {(CAREER.Handmaiden, 6, 1), ("Gift of Ladrielle", "Kerillian disappears from enemy perception for 2 seconds after using Dash. (Invisibility allows Kerillian to freely move through enemies)") },
            {(CAREER.Handmaiden, 6, 2), ("Bladedancer", "Dashing through an enemy causes them to bleed for significant damage over time.") },
            {(CAREER.Handmaiden, 6, 3), ("Power from Pain", "Each enemy hit with Dash grants 5% critical strike chance for 15 seconds. Stacks up to 5 times.") },

            {(CAREER.Shade, 1, 1), ("Bleak Vigour", CRIT_HEADSHOT_THP) },
            {(CAREER.Shade, 1, 2), ("Khaine’s Thirst", KILL_THP) },
            {(CAREER.Shade, 1, 3), ("Blood Kin", HEAL_SHARE) },
            {(CAREER.Shade, 2, 1), ("Cruelty", "Increases critical strike damage bonus by 50%.") },
            {(CAREER.Shade, 2, 2), ("Exploit Weakness", "Increases damage by 20% to poisoned or bleeding enemies.") },
            {(CAREER.Shade, 2, 3), ("Exquisite Huntress", "Headshots increases headshot damage bonus by 10% for 5 seconds. Stacks up to 10 times.") },
            {(CAREER.Shade, 3, 1), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Shade, 3, 2), (ASSASSIN, DESCRIPTION_ASSASSIN) },
            {(CAREER.Shade, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Shade, 4, 1), ("Ereth Khial’s Herald", "Assassin’s Blade is increased to 75% additional damage when attacking enemies from behind.") },
            {(CAREER.Shade, 4, 2), ("Vanish", "Killing an enemy with a backstab grants stealth for 3 seconds.") },
            {(CAREER.Shade, 4, 3), ("Bloodfletcher", "Backstabs return 1 bolt or arrow. (2 second internal cooldown)") },
            {(CAREER.Shade, 5, 1), ("Blood Drinker", "Critical hits reduces damage taken by 20% for 5 seconds.") },
            {(CAREER.Shade, 5, 2), ("Spring-Heeled Assassin", "Critical hits increases movement speed by 20% for 5 seconds.") },
            {(CAREER.Shade, 5, 3), ("Gladerunner", "Increases movement speed by 10%.") },
            {(CAREER.Shade, 6, 1), ("Cloak of Mist", "Infiltrate cooldown reduced by 45%. After leaving stealth gains 100% melee critical strike chance for 4 seconds. No longer grants a damage bonus on attacking.") },
            {(CAREER.Shade, 6, 2), ("Shadowstep", "Infiltrate causes Kerillian to blink forward, passing through enemies.") },
            {(CAREER.Shade, 6, 3), ("Cloak of Pain", "Hitting an enemy while under the effect of Infiltrate does not break stealth. Second attack does not grant bonus damage. Can only trigger once.") },

            {(CAREER.Sister_of_the_Thorn, 1, 1), ("Weavebond", CRIT_HEADSHOT_THP) },
            {(CAREER.Sister_of_the_Thorn, 1, 2), ("Martial Blessing", KILL_THP) },
            {(CAREER.Sister_of_the_Thorn, 1, 3), ("Eternal Blossom", HEAL_SHARE) },
            {(CAREER.Sister_of_the_Thorn, 2, 1), ("Surge of Malice", "While above 90% health Kerillian gains 15% attack speed.") },
            {(CAREER.Sister_of_the_Thorn, 2, 2), ("Atharti's Delight", "Critical Strikes make the target Bleed.") },
            {(CAREER.Sister_of_the_Thorn, 2, 3), ("Isha's Bounty", "Gaining health grants 5% Power for 8 seconds. Stacks up to 3 times.") },
            {(CAREER.Sister_of_the_Thorn, 3, 1), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Sister_of_the_Thorn, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Sister_of_the_Thorn, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Sister_of_the_Thorn, 4, 1), ("Incandescence", "Radiance can stack 2 times.") },
            {(CAREER.Sister_of_the_Thorn, 4, 2), ("Hekarti's Cruel Bargain", "For each Elite enemy slain near Kerillian, the cooldown of Radiance decreases by 1 second.") },
            {(CAREER.Sister_of_the_Thorn, 4, 3), ("Radiant Inheritance", "Consuming Radiance grants Kerillian vastly increased combat proficiency for 10 seconds.") },
            {(CAREER.Sister_of_the_Thorn, 5, 1), ("The Pale Queen's Choosing", "Every 8 seconds, Kerillian's next Ranged Attack consumes no resource and restores 3 permanent health.") },
            {(CAREER.Sister_of_the_Thorn, 5, 2), ("Morai-Heg's Doomsight", "Gain 3 guaranteed Critical Strikes each time a career skill is used.") },
            {(CAREER.Sister_of_the_Thorn, 5, 3), ("Repel", "Pushing at full Stamina increase the strength and range of the push by 100%.") },
            {(CAREER.Sister_of_the_Thorn, 6, 1), ("Ironbark Thicket", "Increase the Duration of the Thorn Wall to 10 seconds.") },
            {(CAREER.Sister_of_the_Thorn, 6, 2), ("Bloodrazor Thicket", "Increase the Thorn Wall's eruption damage and makes it apply Bleed, but lowers both size and duration.") },
            {(CAREER.Sister_of_the_Thorn, 6, 3), ("Blackvenom Thicket", "When the Thorn Wall expires, poisonous thorns explode outward, causing nearby enemies to take 20% increased damage for 10 seconds.") },

            {(CAREER.Witch_Hunter_Captain, 1, 1), ("Hunter’s Ardour", CRIT_HEADSHOT_THP) },
            {(CAREER.Witch_Hunter_Captain, 1, 2), ("Walking Judgement", HIT_THP) },
            {(CAREER.Witch_Hunter_Captain, 1, 3), ("Disciplinarian", HEAL_SHARE) },
            {(CAREER.Witch_Hunter_Captain, 2, 1), ("Riposte", "Blocking just as an enemy attack is about to hit causes your next melee attack to be a guaranteed critical hit.") },
            {(CAREER.Witch_Hunter_Captain, 2, 2), ("Deathknell", "Increases headshot damage bonus by 50%.") },
            {(CAREER.Witch_Hunter_Captain, 2, 3), ("Flense", "Enemies hit by melee attacks bleed for extra damage.") },
            {(CAREER.Witch_Hunter_Captain, 3, 1), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Witch_Hunter_Captain, 3, 2), (ASSASSIN, DESCRIPTION_ASSASSIN) },
            {(CAREER.Witch_Hunter_Captain, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Witch_Hunter_Captain, 4, 1), ("Templar’s Knowledge", "Witch Hunt causes enemies to take an additional 5% damage.") },
            {(CAREER.Witch_Hunter_Captain, 4, 2), ("Heretic Sighted", "Tagging an enemy increases attack speed by 10% for 15 seconds.") },
            {(CAREER.Witch_Hunter_Captain, 4, 3), ("Wild Fervour", "Witch-Hunt grants 5% increased critical hit chance to the entire party for 5 seconds when taggable enemies die.") },
            {(CAREER.Witch_Hunter_Captain, 5, 1), ("Charmed Life", "Increases dodge range by 20%.") },
            {(CAREER.Witch_Hunter_Captain, 5, 2), ("Cast Away", "Pushing an enemy increases stamina regeneration by 40% for 2 seconds.") },
            {(CAREER.Witch_Hunter_Captain, 5, 3), ("Always Prepared", "Increases max ammunition by 30%.") },
            {(CAREER.Witch_Hunter_Captain, 6, 1), ("I Shall Judge You All!", "Applies Witch Hunt to enemies hit by Animosity.") },
            {(CAREER.Witch_Hunter_Captain, 6, 2), ("Fervency", "Animosity grants Victor guaranteed melee critical strikes for the duration.") },
            {(CAREER.Witch_Hunter_Captain, 6, 3), ("The Unending Hunt", "Hitting 10 or more enemies with Animosity refunds 40% cooldown.") },

            {(CAREER.Bounty_Hunter, 1, 1), ("Blood for Money", CRIT_HEADSHOT_THP) },
            {(CAREER.Bounty_Hunter, 1, 2), ("Tithetaker", KILL_THP) },
            {(CAREER.Bounty_Hunter, 1, 3), ("Paymaster", HEAL_SHARE) },
            {(CAREER.Bounty_Hunter, 2, 1), ("Open Wounds", "Critical hits cause enemies to take 20% increased damage for 15 seconds. Does not stack with similar effects.") },
            {(CAREER.Bounty_Hunter, 2, 2), ("Steel Crescendo", "Upon firing the last shot, gain 15% attack speed and 15% power for 15 seconds.") },
            {(CAREER.Bounty_Hunter, 2, 3), ("Weight of Fire", "Ranged weapon magazine size increases ranged power level by 1% for each Ammunition available.") },
            {(CAREER.Bounty_Hunter, 3, 1), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Bounty_Hunter, 3, 2), (ASSASSIN, DESCRIPTION_ASSASSIN) },
            {(CAREER.Bounty_Hunter, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Bounty_Hunter, 4, 1), ("Blessed Combat", "Melee strikes makes up to the next 6 ranged shots deal 15% more damage. Ranged hits makes up to the next 6 melee strikes deal 15% more damage. Melee kills reset the cooldown of Blessed Shots.") },
            {(CAREER.Bounty_Hunter, 4, 2), ("Cruel Fortune", "Reduces cooldown on Blessed Shots to 6 seconds.") },
            {(CAREER.Bounty_Hunter, 4, 3), ("Prize Bounty", "Shots affected by Blessed Shots consume no ammunition.") },
            {(CAREER.Bounty_Hunter, 5, 1), ("Rile the Mob", "Ranged critical hits grants Victor and his allies 10% movement speed for 10 seconds.") },
            {(CAREER.Bounty_Hunter, 5, 2), ("Dual Action", "Killing an elite while out of ammunition restores 20% of max ammunition. Melee kills reloads Victor's ranged weapon (1 bullet/bolt)") },
            {(CAREER.Bounty_Hunter, 5, 3), ("Job Well Done", "Killing an elite or special enemy grants 1% damage reduction buff, stacking up to 30 times. Lasts until end of mission or death.") },
            {(CAREER.Bounty_Hunter, 6, 1), ("Just Reward", "Ranged critical hits reduce cooldown of Locked and Loaded by 20%. Can only trigger once every 10 seconds.") },
            {(CAREER.Bounty_Hunter, 6, 2), ("Double-shotted", "Modifies Victor’s sidearm to fire two powerful bullets in a straight line. Scoring a headshot with this attack reduces the cooldown of Locked and Loaded by 40%.") },
            {(CAREER.Bounty_Hunter, 6, 3), ("Indiscriminate Blast", "Modifies Victor’s sidearm to fire two blasts of shield penetrating pellets in a devastating cone. Each kill with the blast increases the amount of pellets in the next blast. (Max 20 additional pellets)") },

            {(CAREER.Zealot, 1, 1), ("Sigmar’s Herald", HIT_THP) },
            {(CAREER.Zealot, 1, 2), ("Repent! Repent!", KILL_THP) },
            {(CAREER.Zealot, 1, 3), ("Font of Zeal", HEAL_SHARE) },
            {(CAREER.Zealot, 2, 1), ("Castigate", "Increases attack speed by 10% while below 50% health. Double effect while below 20% health.") },
            {(CAREER.Zealot, 2, 2), ("Smite", "Every 5 hits grants a guaranteed critical strike. Critical strikes can no longer occur randomly.") },
            {(CAREER.Zealot, 2, 3), ("Unbending Purpose", "Increases power by 5%.") },
            {(CAREER.Zealot, 3, 1), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Zealot, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Zealot, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Zealot, 4, 1), ("Crusade", "Each stack of Fiery Faith also increases movement speed by 5%.") },
            {(CAREER.Zealot, 4, 2), ("Holy Fortitude", "Each stack of Fiery Faith also increases healing recieved by 15%.") },
            {(CAREER.Zealot, 4, 3), ("Armour of Faith", "Each stack of Fiery Faith also reduces damage taken by 5%.") },
            {(CAREER.Zealot, 5, 1), ("Devotion", "Taking damage increases movement speed by 30% for 2 seconds. Getting attacked no longer slows movement speed.") },
            {(CAREER.Zealot, 5, 2), ("Redemption through Blood", "Taking damage restores stamina to full.") },
            {(CAREER.Zealot, 5, 3), ("Calloused Without and Within", "Damage taken reduced by 10%.") },
            {(CAREER.Zealot, 6, 1), ("Faith’s Flurry", "Attacks during Holy Fervour increases power by 2% for 5 seconds. Stacks up to 10 times.") },
            {(CAREER.Zealot, 6, 2), ("Feel Nothing", "Health can’t be reduced below 1 for the duration of Holy Fervour.") },
            {(CAREER.Zealot, 6, 3), ("Flagellant’s Zeal", "Each hit during Holy Fervour grants a stack up to 10. Each stack reduces the cooldown of Holy Fervour by 5% when the buff fades.") },

            {(CAREER.Saltzpyre_UNKNOWN, 1, 1), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 1, 2), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 1, 3), ("Name", HEAL_SHARE) },
            {(CAREER.Saltzpyre_UNKNOWN, 2, 1), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 2, 2), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 2, 3), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 3, 1), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 3, 2), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Saltzpyre_UNKNOWN, 4, 1), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 4, 2), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 4, 3), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 5, 1), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 5, 2), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 5, 3), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 6, 1), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 6, 2), ("Name", "Description") },
            {(CAREER.Saltzpyre_UNKNOWN, 6, 3), ("Name", "Description") },

            {(CAREER.Battle_Wizard, 1, 1), ("Confound", STAGGER_THP) },
            {(CAREER.Battle_Wizard, 1, 2), ("Spark Thief", KILL_THP) },
            {(CAREER.Battle_Wizard, 1, 3), ("Flame-Fettled", HEAL_SHARE) },
            {(CAREER.Battle_Wizard, 2, 1), ("Volcanic Force", "Fully charging spells increases its power by 50%.") },
            {(CAREER.Battle_Wizard, 2, 2), ("Famished Flames", "Burning damage over time is increased by 150%. All non-burn damage is reduced by 30%.") },
            {(CAREER.Battle_Wizard, 2, 3), ("Lingering Flames", "Sienna’s burning effects now last until the affected enemy dies. Burning effects does not stack.") },
            {(CAREER.Battle_Wizard, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Battle_Wizard, 3, 2), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Battle_Wizard, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Battle_Wizard, 4, 1), ("Unusually Calm", "Tranquility cooldown is reduced to 3 seconds.") },
            {(CAREER.Battle_Wizard, 4, 2), ("Rechannel", "When Tranquillity is active, Sienna’s ranged charge time is reduced by 40%.") },
            {(CAREER.Battle_Wizard, 4, 3), ("Centred", "Increases the venting effect of Tranquillity by 100%.") },
            {(CAREER.Battle_Wizard, 5, 1), ("Soot Shield", "Igniting an enemy reduces damage taken by 10% for 5 seconds. Stacks up to 3 times.") },
            {(CAREER.Battle_Wizard, 5, 2), ("Fires from Ash", "Killing a burning enemy reduces the cooldown of Fire Walk by 3%.") },
            {(CAREER.Battle_Wizard, 5, 3), ("Immersive Immolation", "Hitting 4 or more enemies with one attack grants 20% increased attack speed for 5 seconds.") },
            {(CAREER.Battle_Wizard, 6, 1), ("Volans' Quickening", "Reduces the cooldown of Fire Walk by 30%.") },
            {(CAREER.Battle_Wizard, 6, 2), ("Kaboom!", "Fire Walk explosion radius and damage increased. No longer leaves a burning trail.") },
            {(CAREER.Battle_Wizard, 6, 3), ("Burnout", "Fire Walk can be activated a second time within 10 seconds.") },

            {(CAREER.Pyromancer, 1, 1), ("Spark Smith", HIT_THP) },
            {(CAREER.Pyromancer, 1, 2), ("Spirit-Binding", KILL_THP) },
            {(CAREER.Pyromancer, 1, 3), ("Fiery Fortitude", HEAL_SHARE) },
            {(CAREER.Pyromancer, 2, 1), ("Ride the Fire Wind", "Increases ranged power level by 1% every second up to a maximum of 20 stacks. Upon reaching maximum stacks the effect diminishes then starts over.") },
            {(CAREER.Pyromancer, 2, 2), ("Martial Study", "Increases attack speed by 5%.") },
            {(CAREER.Pyromancer, 2, 3), ("Spirit-Casting", "Increases critical strike chance by 10% while above 80% health.") },
            {(CAREER.Pyromancer, 3, 1), (SMITER, DESCRIPTION_SMITER) },
            {(CAREER.Pyromancer, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Pyromancer, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Pyromancer, 4, 1), ("Deathly Dissipation", "Killing a special stops your spells from generating overcharge for 10 seconds.") },
            {(CAREER.Pyromancer, 4, 2), ("On the Precipice", "Increases ranged power by 15% when at or above critical overcharge.") },
            {(CAREER.Pyromancer, 4, 3), ("One with the Flame", "Critical Mass also increases attack speed by 2% per 6 overcharge and stacks up to 5 times.") },
            {(CAREER.Pyromancer, 5, 1), ("Soul Siphon", "Reduces damage taken by 10% for 10 seconds after killing a special or elite enemy. Stacks up to 3 times.") },
            {(CAREER.Pyromancer, 5, 2), ("The Volans Doctrine", "No longer slowed from being overcharged.") },
            {(CAREER.Pyromancer, 5, 3), ("Fleetflame", "Critical hits increases movement speed by 5% for 5 seconds. Stacks up to 3 times.") },
            {(CAREER.Pyromancer, 6, 1), ("Exhaust", "The Burning Head also removes all overcharge.") },
            {(CAREER.Pyromancer, 6, 2), ("Bonded Flame", "The Burning Head grants 35 temporary health when used.") },
            {(CAREER.Pyromancer, 6, 3), ("Blazing Echo", "The Burning Head critical hits refunds its cooldown.") },

            {(CAREER.Unchained, 1, 1), ("Soul Quench", STAGGER_THP) },
            {(CAREER.Unchained, 1, 2), ("Reckless Rampage", HIT_THP) },
            {(CAREER.Unchained, 1, 3), ("Burn-Bloom", HEAL_SHARE) },
            {(CAREER.Unchained, 2, 1), ("Frenzied Flame", "Increases attack speed by 15% while at or above high Overcharge.") },
            {(CAREER.Unchained, 2, 2), ("Outburst", "Pushing an enemy ignites them with a dot (damage over time for 4 seconds). Heavy attacks make the next push arc 70% wider.") },
            {(CAREER.Unchained, 2, 3), ("Chain Reaction", "Burning enemies have a small chance (40%) to explode on death.") },
            {(CAREER.Unchained, 3, 1), (BULWARK, DESCRIPTION_BULWARK) },
            {(CAREER.Unchained, 3, 2), (MAINSTAY, DESCRIPTION_MAINSTAY) },
            {(CAREER.Unchained, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Unchained, 4, 1), ("Dissipate", "Block cost is reduced by 50% when Overcharged and blocking attacks vent Overcharge.") },
            {(CAREER.Unchained, 4, 2), ("Conduit", "Increases rate of venting overcharge by 30% and reduces damage taken from venting by 50%.") },
            {(CAREER.Unchained, 4, 3), ("Numb to Pain", "Reduces damage taken by 5% and overcharge generated by Blood Magic by 16.6% for 15 seconds after venting. Stacks up to 3 times.") },
            {(CAREER.Unchained, 5, 1), ("Enfeebling Flames", "Burning enemies deal 30% less damage.") },
            {(CAREER.Unchained, 5, 2), ("Abandon", "When Sienna overcharges she starts rapidly exchanging health to ability cooldown. (6% hp to 10% cooldown)") },
            {(CAREER.Unchained, 5, 3), ("Natural Talent", "Reduces overcharge generated by 10%.") },
            {(CAREER.Unchained, 6, 1), ("Fuel for the Fire", "Each enemy hit by Living Bomb increases power by 5% for 15 seconds. Stacks up to 5 times.") },
            {(CAREER.Unchained, 6, 2), ("Wildfire", "Living Bomb grants Sienna a scorching aura that ignites enemies for 10 seconds. Increases the stagger power of Living Bomb.") },
            {(CAREER.Unchained, 6, 3), ("Bomb Balm", "Living Bomb restores 30 temporary health to allies.") },

            {(CAREER.Sienna_UNKNOWN, 1, 1), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 1, 2), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 1, 3), ("Name", HEAL_SHARE) },
            {(CAREER.Sienna_UNKNOWN, 2, 1), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 2, 2), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 2, 3), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 3, 1), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 3, 2), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 3, 3), (ENHANCED_POWER, DESCRIPTION_ENHANCED_POWER) },
            {(CAREER.Sienna_UNKNOWN, 4, 1), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 4, 2), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 4, 3), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 5, 1), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 5, 2), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 5, 3), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 6, 1), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 6, 2), ("Name", "Description") },
            {(CAREER.Sienna_UNKNOWN, 6, 3), ("Name", "Description") },
        };
        #endregion
        #endregion

        public Dictionary<(int, int), Rectangle> ShadingRectangles = new Dictionary<(int, int), Rectangle>();
        public Dictionary<(int, int), Popup> TalentPopups = new Dictionary<(int, int), Popup>();

        public TalentTreeDisplay()
        {
            InitializeComponent();

            // Collect shading rectangles
            foreach(var rect in MainGrid.FindLogicalChildren<Rectangle>())
            {
                var row = Grid.GetRow(rect) + 1;
                var column = Grid.GetColumn(rect) + 1;

                ShadingRectangles.Add((row, column), rect);
            }

            // Collect popups
            foreach(var border in MainGrid.FindLogicalChildren<Border>().Where(b => b.Parent == MainGrid))
            {
                var row = Grid.GetRow(border) + 1;
                var column = Grid.GetColumn(border) + 1;

                var popup = border.FindLogicalChildren<Popup>().FirstOrDefault();

                TalentPopups.Add((row, column), popup);
            }
        }

        public void UpdateDisplay()
        {
            foreach(var image in MainGrid.FindLogicalChildren<Image>())
            {
                image.GetBindingExpression(Image.SourceProperty).UpdateTarget();
            }

            foreach (var textBlock in MainGrid.FindLogicalChildren<TextBlock>())
            {
                textBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            }

            // Reset each shading rectangle to be visible
            foreach (var rect in MainGrid.FindLogicalChildren<Rectangle>())
            {
                rect.Visibility = Visibility.Visible;
            }

            if(Talents != null)
            {
                // Hide shading rectangle for each allocated talent
                foreach (var talentChoice in Talents.AllocatedTalents())
                {
                    ShadingRectangles[(talentChoice.Row, talentChoice.Column)].Visibility = Visibility.Hidden;
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is Border b)
            {
                var row = Grid.GetRow(b) + 1;
                var column = Grid.GetColumn(b) + 1;

                if (TalentPopups[(row, column)] != null)
                {
                    TalentPopups[(row, column)].IsOpen = true;
                }
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border b)
            {
                var row = Grid.GetRow(b) + 1;
                var column = Grid.GetColumn(b) + 1;

                if(TalentPopups[(row, column)] != null)
                {
                    TalentPopups[(row, column)].IsOpen = false;
                }
            }
        }
    }
}
