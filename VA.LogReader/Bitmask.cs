namespace VA.LogReader
{
    public class Bitmask
    {
        public const byte EVENT_TYPE = 0b_1111_1100;
        public const uint PAYLOAD = 0b_11_1111_1111_1111_1111_1111_1111;

        public const uint DIFFICULTY                = 0b_00_0000_0000_1110_0000_0000_0000;
        public const uint CAREER                    = 0b_00_0000_0000_0001_1111_0000_0000;
        public const uint CAMPAIGN                  = 0b_00_0000_0000_0000_0000_1111_0000;
        public const uint MISSION                   = 0b_00_0000_0000_0000_0000_0000_1111;

        public const uint WEAPON1_OWNER             = 0b_00_1110_0000_0000_0000_0000_0000;
        public const uint WEAPON2_OWNER             = 0b_00_0001_1100_0000_0000_0000_0000;
        public const uint WEAPON1_RARITY            = 0b_00_0000_0011_1000_0000_0000_0000;
        public const uint WEAPON2_RARITY            = 0b_00_0000_0000_0111_0000_0000_0000;
        public const uint WEAPON1                   = 0b_00_0000_0000_0000_1111_1100_0000;
        public const uint WEAPON2                   = 0b_00_0000_0000_0000_0000_0011_1111;

        public const uint TALENTS                   = 0b_00_0000_0011_1111_1111_1111_1111;

        public const uint PERMANENT_HEALTH          = 0b_00_0000_1111_1111_1100_0000_0000;
        public const uint TEMPORARY_HEALTH          = 0b_00_0000_0000_0000_0011_1111_1111;

        public const uint UNCAPPED_HEAL_INT         = 0b_00_1111_1111_1100_0000_0000_0000;
        public const uint UNCAPPED_HEAL_FRACTION    = 0b_00_0000_0000_0011_0000_0000_0000;
        public const uint CAPPED_HEAL_INT           = 0b_00_0000_0000_0000_1111_1111_1100;
        public const uint CAPPED_HEAL_FRACTION      = 0b_00_0000_0000_0000_0000_0000_0011;

        public const uint CRIT                      = 0b_00_0001_0000_0000_0000_0000_0000;
        public const uint HEADSHOT                  = 0b_00_0000_1000_0000_0000_0000_0000;
        public const uint TARGET                    = 0b_00_0000_0110_0000_0000_0000_0000;
        public const uint DAMAGE_SOURCE             = 0b_00_0000_0001_1000_0000_0000_0000;
        public const uint DAMAGE_TAKEN_SOURCE       = 0b_00_0000_0001_1000_0000_0000_0000;
        public const uint DAMAGE_INT                = 0b_00_0000_0000_0111_1111_1111_1100;
        public const uint DAMAGE_FRACTION           = 0b_00_0000_0000_0000_0000_0000_0011;

        public const uint STAGGER_LEVEL             = 0b_00_0000_1111_0000_0000_0000_0000;
        public const uint STAGGER_SOURCE            = 0b_00_0000_0000_1100_0000_0000_0000;
        public const uint STAGGER_DURATION          = 0b_00_0000_0000_0011_1111_1111_1111;

        public const uint PLAYER_STATE              = 0b_00_0000_0000_0000_0000_0000_0011;

        public const uint ENEMY_TYPE                = 0b_00_0000_0110_0000_0000_0000_0000;

        public const uint TRAIT_SOURCE              = 0b_00_0000_0000_0000_0001_1100_0000;
        public const uint TRAIT                     = 0b_00_0000_0000_0000_0000_0011_1111;
        public const uint PROPERTY_SOURCE           = 0b_00_0000_0111_0000_0000_0000_0000;
        public const uint PROPERTY                  = 0b_00_0000_0000_1111_1100_0000_0000;
        public const uint PROPERTY_VALUE            = 0b_00_0000_0000_0000_0011_1111_1111;

        public const uint ROUND_RESULT              = 0b_00_0000_0000_0000_0000_0000_0011;
    }

    public class Bitshift
    {
        private static int Calculate(uint bitmask)
        {
            int shift = 0;
            for (uint divisor = 2; divisor <= bitmask; divisor <<= 1)
            {
                if (bitmask % divisor > 0) break;
                shift++;
            }

            return shift;
        }

        public static readonly int EVENT_TYPE = Calculate(Bitmask.EVENT_TYPE);

        public static readonly int DIFFICULTY = Calculate(Bitmask.DIFFICULTY);
        public static readonly int CAREER = Calculate(Bitmask.CAREER);
        public static readonly int CAMPAIGN = Calculate(Bitmask.CAMPAIGN);
        public static readonly int MISSION = Calculate(Bitmask.MISSION);

        public static readonly int WEAPON1_OWNER = Calculate(Bitmask.WEAPON1_OWNER);
        public static readonly int WEAPON2_OWNER = Calculate(Bitmask.WEAPON2_OWNER);
        public static readonly int WEAPON1_RARITY = Calculate(Bitmask.WEAPON1_RARITY);
        public static readonly int WEAPON2_RARITY = Calculate(Bitmask.WEAPON2_RARITY);
        public static readonly int WEAPON1 = Calculate(Bitmask.WEAPON1);
        public static readonly int WEAPON2 = Calculate(Bitmask.WEAPON2);

        public static readonly int TALENTS = Calculate(Bitmask.TALENTS);

        public static readonly int PERMANENT_HEALTH = Calculate(Bitmask.PERMANENT_HEALTH);
        public static readonly int TEMPORARY_HEALTH = Calculate(Bitmask.TEMPORARY_HEALTH);

        public static readonly int UNCAPPED_HEAL_INT = Calculate(Bitmask.UNCAPPED_HEAL_INT);
        public static readonly int UNCAPPED_HEAL_FRACTION = Calculate(Bitmask.UNCAPPED_HEAL_FRACTION);
        public static readonly int CAPPED_HEAL_INT = Calculate(Bitmask.CAPPED_HEAL_INT);
        public static readonly int CAPPED_HEAL_FRACTION = Calculate(Bitmask.CAPPED_HEAL_FRACTION);

        public static readonly int CRIT = Calculate(Bitmask.CRIT);
        public static readonly int HEADSHOT = Calculate(Bitmask.HEADSHOT);
        public static readonly int TARGET = Calculate(Bitmask.TARGET);
        public static readonly int DAMAGE_SOURCE = Calculate(Bitmask.DAMAGE_SOURCE);
        public static readonly int DAMAGE_TAKEN_SOURCE = Calculate(Bitmask.DAMAGE_TAKEN_SOURCE);
        public static readonly int DAMAGE_INT = Calculate(Bitmask.DAMAGE_INT);
        public static readonly int DAMAGE_FRACTION = Calculate(Bitmask.DAMAGE_FRACTION);

        public static readonly int STAGGER_LEVEL = Calculate(Bitmask.STAGGER_LEVEL);
        public static readonly int STAGGER_SOURCE = Calculate(Bitmask.STAGGER_SOURCE);
        public static readonly int STAGGER_DURATION = Calculate(Bitmask.STAGGER_DURATION);

        public static readonly int PLAYER_STATE = Calculate(Bitmask.PLAYER_STATE);

        public static readonly int ENEMY_TYPE = Calculate(Bitmask.ENEMY_TYPE);

        public static readonly int TRAIT_SOURCE = Calculate(Bitmask.TRAIT_SOURCE);
        public static readonly int TRAIT = Calculate(Bitmask.TRAIT);
        public static readonly int PROPERTY_SOURCE = Calculate(Bitmask.PROPERTY_SOURCE);
        public static readonly int PROPERTY = Calculate(Bitmask.PROPERTY);
        public static readonly int PROPERTY_VALUE = Calculate(Bitmask.PROPERTY_VALUE);

        public static readonly int ROUND_RESULT = Calculate(Bitmask.ROUND_RESULT);
    }
}
