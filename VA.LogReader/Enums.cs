using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VA.LogReader
{
	public static class Enums
	{
		public const int MISC_MISSION_SHIFT = 0;
		public const int HELMGART_MISSION_SHIFT = 4;
		public const int DRACHENFELS_MISSION_SHIFT = 8;
		public const int BOGENHAFEN_MISSION_SHIFT = 12;
		public const int UBERSREIK_MISSION_SHIFT = 16;
		public const int WOM_MISSION_SHIFT = 20;
		public const int CHAOS_WASTES_MISSION_SHIFT = 24;
	}


	public enum ParseError
	{ 
		None = 0,
		SchemaMismatch,
		BadHeader,
		NoStartEvent,
		NoWeapon1Data,
		NoWeapon2Data,
		NoTalentData,
	}


    #region SCHEMA
    public enum DIFFICULTY : byte
    {
		Recruit = 0,
		Veteran = 1,
		Champion = 2,
		Legend = 3,
		Cataclysm = 4,
		Cataclysm2 = 5,
		Cataclysm3 = 6,
		Cataclysm4 = 7
	}

	public enum ONSLAUGHT_TYPE : byte
	{
		None = 0,
		Onslaught = 1,
		OnslaughtPlus = 2,
		OnslaughtSquared = 3,
		SpicyOnslaught = 4
	}

	public enum WEAPON_SLOT : byte
	{
		Weapon1 = 0,
		Weapon2 = 1
	}

	public enum HERO : byte
	{
		Markus = 0,
		Bardin = 1,
		Kerillian = 2,
		Victor = 3,
		Sienna = 4
	}

	public enum CAREER : byte
	{
		Mercenary = 0,
		Huntsman = 1,
		Foot_Knight = 2,
		Grail_Knight = 3,

		Ranger_Veteran = 4,
		Ironbreaker = 5,
		Slayer = 6,
		Outcast_Engineer = 7,
	
		Waystalker = 8,
		Handmaiden = 9,
		Shade = 10,
		Sister_of_the_Thorn = 11,
	
		Witch_Hunter_Captain = 12,
		Bounty_Hunter = 13,
		Zealot = 14,
		Saltzpyre_UNKNOWN = 15,
	
		Battle_Wizard = 16,
		Pyromancer = 17,
		Unchained = 18,
		Sienna_UNKNOWN = 19
	}

	public enum CAMPAIGN : byte
	{
		Unknown = 0,

		Misc = 1,
		Helmgart = 2,
		Drachenfels = 3,
		Bogenhafen = 4,
		Ubersreik = 5,
		Winds_of_Magic = 6,
		Chaos_Wastes = 7
	}

	// All missions in a single enum
	// 4 bits (15 missions slots) given for each Campaign
	public enum MISSION : long
	{
		Unknown = 0,

		// Misc
		A_Quiet_Drink = 1 << Enums.MISC_MISSION_SHIFT,

		// Helmgart
		Righteous_Stand = 1 << Enums.HELMGART_MISSION_SHIFT,
		Convocation_of_Decay = 2 << Enums.HELMGART_MISSION_SHIFT,
		Hunger_in_the_Dark = 3 << Enums.HELMGART_MISSION_SHIFT,
		Halescourge = 4 << Enums.HELMGART_MISSION_SHIFT,

		Athel_Yenlui = 5 << Enums.HELMGART_MISSION_SHIFT,
		The_Screaming_Bell = 6 << Enums.HELMGART_MISSION_SHIFT,
		Fort_Brachsenbrucke = 7 << Enums.HELMGART_MISSION_SHIFT,
		Into_the_Nest = 8 << Enums.HELMGART_MISSION_SHIFT,

		Against_the_Grain = 9 << Enums.HELMGART_MISSION_SHIFT,
		Empire_in_Flames = 10 << Enums.HELMGART_MISSION_SHIFT,
		Festering_Ground = 11 << Enums.HELMGART_MISSION_SHIFT,
		The_War_Camp = 12 << Enums.HELMGART_MISSION_SHIFT,

		The_Skittergate = 13 << Enums.HELMGART_MISSION_SHIFT,

		// Drachenfels
		Old_Haunts = 1 << Enums.DRACHENFELS_MISSION_SHIFT,
		Blood_in_the_Darkness = 2 << Enums.DRACHENFELS_MISSION_SHIFT,
		The_Enchanters_Lair = 3 << Enums.DRACHENFELS_MISSION_SHIFT,

		// Bogenhafen
		The_Pit = 1 << Enums.BOGENHAFEN_MISSION_SHIFT,
		The_Blightreaper = 2 << Enums.BOGENHAFEN_MISSION_SHIFT,

		// Ubersreik
		The_Horn_of_Magnus = 1 << Enums.UBERSREIK_MISSION_SHIFT,
		Garden_of_Morr = 2 << Enums.UBERSREIK_MISSION_SHIFT,
		Engines_of_War = 3 << Enums.UBERSREIK_MISSION_SHIFT,
		Fortunes_of_War = 4 << Enums.UBERSREIK_MISSION_SHIFT,

		// Winds of Magic
		Dark_Omens = 1 << Enums.WOM_MISSION_SHIFT,

		// Chaos Wastes
		No_Curse = 1 << Enums.CHAOS_WASTES_MISSION_SHIFT,

		// Nurgle
		[Description("Hexed enemies deal poison damage and create poison clouds when they die.")]
		Infested_Foes = 2 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("The level is covered in a poisonous miasma that inflicts the Curse effect. Stand close to the Purifying Torch to quell the curse.")]
		Miasma_of_Pestilence = 3 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("A malevolent spirit haunts this level. It cannot be slain, only driven off.")]
		Skulking_Sorcerer = 4 << Enums.CHAOS_WASTES_MISSION_SHIFT,

		// Khorne
		[Description("On death, enemies drop an exploding skull totem.")]
		Skulls_of_Wrath = 5 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("Beware of hexed enemies. Their fury enhances those who fight at their side.")]
		Blood_Gods_Fury = 6 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("Blood tornados move randomly around the level, inflicting bleeding on those they touch.")]
		Blood_Tornadoes = 7 << Enums.CHAOS_WASTES_MISSION_SHIFT,

		// Tzeentch
		[Description("Enemies have a chance to be reborn as smaller enemies upon death.")]
		Twin_Blight = 8 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("Tzeentch launches arcane bolts from the skies, dealing damage to players and randomly transforming enemies.")]
		Bolt_of_Change = 9 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("Destroy the crystal eggs before they hatch, or face the fury of the beast within!")]
		Crystal_Egg = 10 << Enums.CHAOS_WASTES_MISSION_SHIFT,

		// Slaanesh
		[Description("Pickups are disabled on this map, but hexed enemies spawn items for the taking!")]
		Glory_to_Greed = 11 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("A portion of damage inflicted on a player is also inflicted on other players in close proximity.")]
		Baleful_Empathy = 12 << Enums.CHAOS_WASTES_MISSION_SHIFT,
		[Description("Players suffer damage over time. Drinking potions restores health lost to Unquenchable Thirst.")]
		Unquenchable_Thirst = 13 << Enums.CHAOS_WASTES_MISSION_SHIFT
	}

	public enum HITZONE : byte
	{
		Bodyshot = 0,
		Headshot = 1
	}

	public enum DAMAGE_TARGET : byte
	{
		Enemy = 0,
		Monster = 1,
		Ally = 2
	}

	public enum DAMAGE_SOURCE : byte
	{
		Weapon1 = 0,
		Weapon2 = 1,
		Career = 2,
		Other = 3
	}
	public enum STAGGER_SOURCE : byte
	{
		Push = 0,
		Other = 1
	}

	public enum DAMAGE_TAKEN_SOURCE : byte
	{
		Enemy = 0,
		Special = 1,
		Monster = 2,
		Ally = 3
	}

	public enum PLAYER_STATE : byte
	{
		Alive = 0,
		Downed = 1,
		Dead = 2
	}

	public enum ENEMY_TYPE : byte
	{
		Normal = 0,
		Elite = 1,
		Special = 2
	}

	public enum ROUND_RESULT : byte
	{
		Quit = 0,
		Loss = 1,
		Win = 2
	}
	#endregion
}
