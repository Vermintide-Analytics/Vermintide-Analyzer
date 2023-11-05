using System.ComponentModel;

namespace VA.LogReader
{
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
		Off = 0,
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
		FootKnight = 2,
		GrailKnight = 3,

		RangerVeteran = 4,
		Ironbreaker = 5,
		Slayer = 6,
		OutcastEngineer = 7,
	
		Waystalker = 8,
		Handmaiden = 9,
		Shade = 10,
		SisterOfTheThorn = 11,
	
		WitchHunterCaptain = 12,
		BountyHunter = 13,
		Zealot = 14,
		WarriorPriest = 15,
	
		BattleWizard = 16,
		Pyromancer = 17,
		Unchained = 18,
		Necromancer = 19,

		SiennaUNKNOWN = 39
	}

	public enum CAMPAIGN : byte
	{
		Unknown = 0,

		Misc = 1,
		Helmgart = 2,
		Drachenfels = 3,
		Bogenhafen = 4,
		Ubersreik = 5,
		WindsOfMagic = 6,
		ChaosWastes = 7,
		Weave = 8,
		KarakAzgaraz = 9,
		Treachery = 10
	}

	// All missions in a single enum
	// 4 bits (15 missions slots) given for each Campaign
	public enum MISSION : long
	{
		Unknown = 0,

		// Helmgart
		RighteousStand = 1,
		ConvocationOfDecay = 2,
		HungerInTheDark = 3,
		Halescourge = 4,

		AthelYenlui = 5,
		TheScreamingBell = 6,
		FortBrachsenbrucke = 7,
		IntoTheNest = 8,

		AgainstTheGrain = 9,
		EmpireInFlames = 10,
		FesteringGround = 11,
		TheWarCamp = 12,

		TheSkittergate = 13,

		// Drachenfels
		OldHaunts = 101,
		BloodInTheDarkness = 102,
		TheEnchantersLair = 103,

		// Bogenhafen
		ThePit = 201,
		TheBlightreaper = 202,

		// Ubersreik
		TheHornOfMagnus = 301,
		GardenOfMorr = 302,
		EnginesOfWar = 303,
		FortunesOfWar = 304,

		// Winds of Magic
		DarkOmens = 401,

		// Chaos Wastes
		NoCurse = 501,

		// Nurgle
		[Description("Hexed enemies deal poison damage and create poison clouds when they die.")]
		InfestedFoes = 502,
		[Description("The level is covered in a poisonous miasma that inflicts the Curse effect. Stand close to the Purifying Torch to quell the curse.")]
		MiasmaOfPestilence = 503,
		[Description("A malevolent spirit haunts this level. It cannot be slain, only driven off.")]
		SkulkingSorcerer = 504,

		// Khorne
		[Description("On death, enemies drop an exploding skull totem.")]
		SkullsOfWrath = 505,
		[Description("Beware of hexed enemies. Their fury enhances those who fight at their side.")]
		BloodGodsFury = 506,
		[Description("Blood tornados move randomly around the level, inflicting bleeding on those they touch.")]
		BloodTornadoes = 507,

		// Tzeentch
		[Description("Enemies have a chance to be reborn as smaller enemies upon death.")]
		TwinBlight = 508,
		[Description("Tzeentch launches arcane bolts from the skies, dealing damage to players and randomly transforming enemies.")]
		BoltOfChange = 509,
		[Description("Destroy the crystal eggs before they hatch, or face the fury of the beast within!")]
		CrystalEgg = 510,

		// Slaanesh
		[Description("Pickups are disabled on this map, but hexed enemies spawn items for the taking!")]
		GloryToGreed = 511,
		[Description("A portion of damage inflicted on a player is also inflicted on other players in close proximity.")]
		BalefulEmpathy = 512,
		[Description("Players suffer damage over time. Drinking potions restores health lost to Unquenchable Thirst.")]
		UnquenchableThirst = 513,

		//Weaves
		[Description("Primal totems can be found in the Weave, empowering enemies. Attack the totems to destroy them.")]
		Amber = 601,
		[Description("Slaying an enemy releases a vengeful spirit to stalk its killer for a time. Spirits deal heavy damage, so keep them at a distance.")]
		Amethyst = 602,
		[Description("All attacks inflict burning on the victim - whether delivered by heroes or enemies.")]
		Bright = 603,
		[Description("Watch out for the circles for warning of incoming lightning strikes. Lightning damages both heroes and enemies.")]
		Celestial = 604,
		[Description("Armoured enemies are tougher, but killing one grants a metalstorm aura that damages foes.")]
		Gold = 605,
		[Description("An enemy's identity is concealed until it gets close. While concealed, enemies take reduced damage.")]
		Grey = 606,
		[Description("Brambles form from the life Essence of slain enemies. Moving into the bramble inflicts damage and reduces your movement speed.")]
		Jade = 607,
		[Description("Hysh drives impurity from the soul, reducing maximum health over time but increasing attack speed through renewed zeal. Standing next to beacons reverses the purging's effects.")]
		Light = 608,

		// Karak Azgaraz
		MissionOfMercy = 701,
		AGrudgeServedCold = 702,
		KhazukanKazakitHa = 703,

		// A Treacherous Adventure
		TrailOfTreachery = 801,
		TowerOfTreachery = 802,

		// Misc
		AQuietDrink = 10001,
	}

	public enum HEAL_ITEM_TYPE : byte
	{
		Draught = 0,
		Bandage = 1
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
		HeroicIntervention = 1,
		[Description("Blocking an attack increases the damage the attacker takes by 20% for 5 seconds")]
		OffBalance = 2,
		[Description("Increases Push strength by 50% when used against an attacking enemy")]
		Opportunist = 3,
		[Description("Timed blocks reduce stamina cost by 100%")]
		Parry = 4,
		[Description("Melee critical strikes reduce the cooldown of your Career Skill by 5%. Effect can only trigger once every 4 seconds")]
		ResourcefulCombatant = 5,
		[Description("Critical Hits increase attack speed by 20% for 5 seconds")]
		SwiftSlaying = 6,
		[Description("Consecutive attacks against the same targets boosts attack power by 5.0% for 5 seconds")]
		Barrage = 7,
		[Description("Headshots replenish 1 ammo")]
		ConservativeShooter = 8,
		[Description("Critical hits refund the overcharge cost of the attack")]
		HeatSink = 9,
		[Description("Critical hits increase attack power by 25% against targets with the same armor class for a short time")]
		Hunter = 10,
		[Description("Headshots restore stamina to nearby allies")]
		InspirationalShot = 11,
		[Description("Ranged critical hits reduce the cooldown of your Career Skill by 5%. Effect can only trigger once every 4 seconds")]
		ResourcefulSharpshooter = 12,
		[Description("Critical hits restore 5% of maximum ammunition. Can trigger once per attack")]
		Scrounger = 13,
		[Description("Weapon generates 20.0% less overheat")]
		ThermalEqualizer = 14,
		[Description("Taking damage reduces the damage you take from subsequent sources by 40% for 2 seconds. This effect can only trigger every 2 seconds")]
		Barkskin = 15,
		[Description("Increases effectiveness of healing on you by 30%")]
		BoonOfShallya = 16,
		[Description("Healing an ally with Medical Supplies also heals you for 50.0% of your missing health")]
		HandOfShallya = 17,
		[Description("25.0% chance to not consume healing item on use")]
		HealersTouch = 18,
		[Description("Passively regenerates 1 health every 5 seconds. Healing from First Aid Kits and Healing Draughts are converted to temporary health and cure any wound")]
		NaturalBond = 19,
		[Description("Drinking a potion grants the effect of all other potions. Duration reduced by 50%")]
		Concoction = 20,
		[Description("Increased duration of potions by 50%")]
		Decanter = 21,
		[Description("25.0% chance to not consume potion on use")]
		HomeBrewer = 22,
		[Description("Consuming a potion spreads the effect to the nearest ally")]
		Proxy = 23,
		[Description("Increases grenade explosion radius by 50.0%")]
		ExplosiveOrdinance = 24,
		[Description("25.0% chance to not consume grenade on use")]
		Grenadier = 25,
		[Description("Grenades cause hit enemies to take 20.0% increased damage for 10.0 seconds")]
		Shrapnel = 26,
		[Description("Critical hits heal 5 health (grants temporary health)")]
		Regrowth = 27,
		[Description("Killing an armoured enemy sends metal shards to damage other nearby enemies")]
		ShardStrike = 28,
		[Description("Increase Attack Speed by 2% for every 5 kills (Up to 5 times). Resets if you do not kill anything for 30 sec")]
		Bloodthirst = 29,
		[Description("Damage taken is reduced to 20 damage or half of its original value, which ever is highest")]
		DivineShield = 30,
		[Description("Critical hits stagger nearby enemeis")]
		Shockwave = 31,
		[Description("Each headshot increases the damage of the next attack by 10%. (up to 20 times). If you hit an enemy anywhere other than the head, 1 stack gets removed")]
		Deadeye = 32,
		[Description("Taking damage grants movement speed for a short time")]
		AdrenalineRush = 33,
		[Description("Chance to not consume potion on use, at the cost of health")]
		ConcentratedBrew = 34
	}

	public enum PROPERTY_SOURCE : byte
	{
		Unknown = 0,

		Weapon1 = 1,
		Weapon2 = 2,
		Necklace = 3,
		Charm = 4,
		Trinket = 5,
		ChaosWastes
	}

	public enum PROPERTY : byte
	{
		Unknown = 0,

		AttackSpeed = 1,
		CritChance = 2,
		CritPower = 3,
		Stamina = 4,
		BlockCostReduction = 5,
		PushBlockAngle = 6,
		DamageReductionVsSkaven = 7,
		DamageReductionVsChaos = 8,
		DamageReductionVsArea = 9,
		Health = 10,
		PowerVsSkaven = 11,
		PowerVsChaos = 12,
		PowerVsInfantry = 13,
		PowerVsArmored = 14,
		PowerVsBerserkers = 15,
		PowerVsMonsters = 16,
		CurseResistance = 17,
		MovementSpeed = 18,
		CooldownReduction = 19,
		RespawnSpeed = 20,
		ReviveSpeed = 21,
		StaminaRecovery = 22,
		PilgrimsCoins = 23,
		AmmoCapacity = 24
	}

	public enum WEAPON
	{
		Unknown = 0,

		// Markus
		es_1h_mace = 1,
		es_1h_sword,
		es_2h_hammer,
		es_2h_sword,
		es_2h_sword_executioner,
		es_halberd,
		es_mace_shield,
		es_sword_shield,
		es_dual_wield_hammer_sword,
		es_2h_heavy_spear,
		es_bastard_sword,
		es_deus_01,
		es_sword_shield_breton,

		es_blunderbuss = 501,
		es_handgun,
		es_repeating_handgun,
		es_longbow,
	
	
		// Bardin
		dr_1h_axe = 1001,
		dr_1h_hammer,
		dr_2h_axe,
		dr_2h_hammer,
		dr_2h_pick,
		dr_shield_axe,
		dr_shield_hammer,
		dr_dual_wield_hammers,
		dr_dual_wield_axes,
		dr_2h_cog_hammer,

		dr_crossbow = 1501,
		dr_drake_pistol,
		dr_drakegun,
		dr_handgun,
		dr_rakegun,
		dr_1h_throwing_axes,
		dr_steam_pistol,
		dr_deus_01,
	
	
		// Kerillian
		we_1h_sword = 2001,
		we_2h_axe,
		we_2h_sword,
		we_dual_wield_daggers,
		we_dual_wield_sword_dagger,
		we_dual_wield_swords,
		we_spear,
		we_1h_axe,
		we_1h_spears_shield,

		we_longbow = 2501,
		we_shortbow,
		we_shortbow_hagbane,
		we_deus_01,
		we_javelin,
		we_crossbow_repeater,
		we_life_staff,
	
	
		// Victor
		es_1h_flail = 3001,
		wh_1h_axe,
		wh_1h_falchion,
		wh_2h_sword,
		wh_fencing_sword,
		wh_dual_wield_axe_falchion,
		wh_2h_billhook,
		wh_1h_hammer,
		wh_hammer_shield,
		wh_dual_hammer,
		wh_2h_hammer,
		wh_flail_shield,
		wh_hammer_book,

		wh_brace_of_pistols = 3501,
		wh_crossbow,
		wh_crossbow_repeater,
		wh_repeating_pistols,
		wh_deus_01,
	
	
		// Sienna
		bw_1h_mace = 4001,
		bw_dagger,
		bw_flame_sword,
		bw_sword,
		bw_1h_crowbill,
		bw_1h_flail_flaming,
		bw_ghost_scythe,

		bw_skullstaff_beam = 4501,
		bw_skullstaff_fireball,
		bw_skullstaff_flamethrower,
		bw_skullstaff_geiser,
		bw_skullstaff_spear,
		bw_deus_01,
		bw_necromancy_staff,
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
