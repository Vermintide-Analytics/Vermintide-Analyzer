﻿using System;
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
		public const int WEAVES_MISSION_SHIFT = 28;
	}


	public enum ParseError
	{ 
		None = 0,
		SchemaMismatch,
		BadHeader,
		NoStartEvent
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
		Chaos_Wastes = 7,
		Weave = 8
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
		Unquenchable_Thirst = 13 << Enums.CHAOS_WASTES_MISSION_SHIFT,

		//Weaves
		[Description("Primal totems can be found in the Weave, empowering enemies. Attack the totems to destroy them.")]
		Amber = 1 << Enums.WEAVES_MISSION_SHIFT,
		[Description("Slaying an enemy releases a vengeful spirit to stalk its killer for a time. Spirits deal heavy damage, so keep them at a distance.")]
		Amethyst = 2 << Enums.WEAVES_MISSION_SHIFT,
		[Description("All attacks inflict burning on the victim - whether delivered by heroes or enemies.")]
		Bright = 3 << Enums.WEAVES_MISSION_SHIFT,
		[Description("Watch out for the circles for warning of incoming lightning strikes. Lightning damages both heroes and enemies.")]
		Celestial = 4 << Enums.WEAVES_MISSION_SHIFT,
		[Description("Armoured enemies are tougher, but killing one grants a metalstorm aura that damages foes.")]
		Gold = 5 << Enums.WEAVES_MISSION_SHIFT,
		[Description("An enemy's identity is concealed until it gets close. While concealed, enemies take reduced damage.")]
		Grey = 6 << Enums.WEAVES_MISSION_SHIFT,
		[Description("Brambles form from the life Essence of slain enemies. Moving into the bramble inflicts damage and reduces your movement speed.")]
		Jade = 7 << Enums.WEAVES_MISSION_SHIFT,
		[Description("Hysh drives impurity from the soul, reducing maximum health over time but increasing attack speed through renewed zeal. Standing next to beacons reverses the purging's effects.")]
		Light = 8 << Enums.WEAVES_MISSION_SHIFT
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

	public enum TRAIT_SOURCE : byte
	{
		Unknown = 0,
	
		Weapon1 = 1,
		Weapon2 = 2,
		Necklace = 3,
		Charm = 4,
		Trinket = 5
	}

	public enum TRAIT : byte
	{
		Unknown = 0,

		[Description("Assisting an ally under attack restores 15 Temporary Health to both players")]
		Heroic_Intervention = 1,
		[Description("Blocking an attack increases the damage the attacker takes by 20% for 5 seconds")]
		Off_Balance = 2,
		[Description("Increases Push strength by 50% when used against an attacking enemy")]
		Opportunist = 3,
		[Description("Timed blocks reduce stamina cost by 100%")]
		Parry = 4,
		[Description("Melee critical strikes reduce the cooldown of your Career Skill by 5%. Effect can only trigger once every 4 seconds")]
		Resourceful_Combatant = 5,
		[Description("Critical Hits increase attack speed by 20% for 5 seconds")]
		Swift_Slaying = 6,
		[Description("Consecutive attacks against the same targets boosts attack power by 5.0% for 5 seconds")]
		Barrage = 7,
		[Description("Headshots replenish 1 ammo")]
		Conservative_Shooter = 8,
		[Description("Critical hits refund the overcharge cost of the attack")]
		Heat_Sink = 9,
		[Description("Critical hits increase attack power by 25% against targets with the same armor class for a short time")]
		Hunter = 10,
		[Description("Headshots restore stamina to nearby allies")]
		Inspirational_Shot = 11,
		[Description("Ranged critical hits reduce the cooldown of your Career Skill by 5%. Effect can only trigger once every 4 seconds")]
		Resourceful_Sharpshooter = 12,
		[Description("Critical hits restore 5% of maximum ammunition. Can trigger once per attack")]
		Scrounger = 13,
		[Description("Weapon generates 20.0% less overheat")]
		Thermal_Equalizer = 14,
		[Description("Taking damage reduces the damage you take from subsequent sources by 40% for 2 seconds. This effect can only trigger every 2 seconds")]
		Barkskin = 15,
		[Description("Increases effectiveness of healing on you by 30%")]
		Boon_of_Shallya = 16,
		[Description("Healing an ally with Medical Supplies also heals you for 50.0% of your missing health")]
		Hand_of_Shallya = 17,
		[Description("25.0% chance to not consume healing item on use")]
		Healers_Touch = 18,
		[Description("Passively regenerates 1 health every 5 seconds. Healing from First Aid Kits and Healing Draughts are converted to temporary health and cure any wound")]
		Natural_Bond = 19,
		[Description("Drinking a potion grants the effect of all other potions. Duration reduced by 50%")]
		Concoction = 20,
		[Description("Increased duration of potions by 50%")]
		Decanter = 21,
		[Description("25.0% chance to not consume potion on use")]
		Home_Brewer = 22,
		[Description("Consuming a potion spreads the effect to the nearest ally")]
		Proxy = 23,
		[Description("Increases grenade explosion radius by 50.0%")]
		Explosive_Ordinance = 24,
		[Description("25.0% chance to not consume grenade on use")]
		Grenadier = 25,
		[Description("Grenades cause hit enemies to take 20.0% increased damage for 10.0 seconds")]
		Shrapnel = 26,
		[Description("Critical hits heal 5 health (grants temporary health)")]
		Regrowth = 27,
		[Description("Killing an armoured enemy sends metal shards to damage other nearby enemies")]
		Shard_Strike = 28,
		[Description("Increase Attack Speed by 2% for every 5 kills (Up to 5 times). Resets if you do not kill anything for 30 sec")]
		Bloodthirst = 29,
		[Description("Damage taken is reduced to 20 damage or half of its original value, which ever is highest")]
		Divine_Shield = 30,
		[Description("Critical hits stagger nearby enemeis")]
		Shockwave = 31,
		[Description("Each headshot increases the damage of the next attack by 10%. (up to 20 times). If you hit an enemy anywhere other than the head, 1 stack gets removed")]
		Deadeye = 32,
		[Description("Taking damage grants movement speed for a short time")]
		Adrenaline_Rush = 33,
		[Description("Chance to not consume potion on use, at the cost of health")]
		Concentrated_Brew = 34
	}

	public enum PROPERTY_SOURCE : byte
	{
		Unknown = 0,

		Weapon1 = 1,
		Weapon2 = 2,
		Necklace = 3,
		Charm = 4,
		Trinket = 5,
		Chaos_Wastes
	}

	public enum PROPERTY : byte
	{
		Unknown = 0,

		Attack_Speed = 1,
		Crit_Chance = 2,
		Crit_Power = 3,
		Stamina = 4,
		Block_Cost_Reduction = 5,
		Push_Block_Angle = 6,
		Damage_Reduction_vs_Skaven = 7,
		Damage_Reduction_vs_Chaos = 8,
		Damage_Reduction_vs_Area = 9,
		Health = 10,
		Power_vs_Skaven = 11,
		Power_vs_Chaos = 12,
		Power_vs_Infantry = 13,
		Power_vs_Armored = 14,
		Power_vs_Berserkers = 15,
		Power_vs_Monsters = 16,
		Curse_Resistance = 17,
		Movement_Speed = 18,
		Cooldown_Reduction = 19,
		Respawn_Speed = 20,
		Revive_Speed = 21,
		Stamina_Recovery = 22,
		Pilgrims_Coins = 23,
		Ammo_Capacity = 24
	}

	public enum RARITY : byte
	{
		Gray = 0,
		White = 1,
		Green = 2,
		Blue = 3,
		Orange = 4,
		Red = 5,
		Magic = 6
	}

	public enum ROUND_RESULT : byte
	{
		Quit = 0,
		Loss = 1,
		Win = 2
	}
	#endregion
}
