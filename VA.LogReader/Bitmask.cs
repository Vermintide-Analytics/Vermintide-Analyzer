using System;
using System.Collections.Generic;
using System.Text;

namespace VA.LogReader
{
    public class Bitmask
    {
        public const byte EVENT_TYPE            =                        0b_1111_1100;
        public const uint PAYLOAD               = 0b_11_1111_1111_1111_1111_1111_1111;

        public const uint DIFFICULTY            = 0b_00_0000_0000_1110_0000_0000_0000;
        public const uint CAREER                = 0b_00_0000_0000_0001_1111_0000_0000;
        public const uint CAMPAIGN              = 0b_00_0000_0000_0000_0000_1111_0000;
        public const uint MISSION               = 0b_00_0000_0000_0000_0000_0000_1111;
        
        public const uint WEAPON1               = 0b_00_0000_0000_0000_1111_1100_0000;
        public const uint WEAPON2               = 0b_00_0000_0000_0000_0000_0011_1111;

        public const uint TALENTS               = 0b_00_0000_0011_1111_1111_1111_1111;

        public const uint PERMANENT_HEALTH      = 0b_00_0000_1111_1111_1100_0000_0000;
        public const uint TEMPORARY_HEALTH      = 0b_00_0000_0000_0000_0011_1111_1111;

        public const uint CRIT                  = 0b_00_0001_0000_0000_0000_0000_0000;
        public const uint HEADSHOT              = 0b_00_0000_1000_0000_0000_0000_0000;
        public const uint TARGET                = 0b_00_0000_0110_0000_0000_0000_0000;
        public const uint DAMAGE_SOURCE         = 0b_00_0000_0001_1000_0000_0000_0000;
        public const uint DAMAGE_TAKEN_SOURCE   = 0b_00_0000_0001_1000_0000_0000_0000;
        public const uint DAMAGE_INT            = 0b_00_0000_0000_0111_1111_1111_1100;
        public const uint DAMAGE_FRACTION       = 0b_00_0000_0000_0000_0000_0000_0011;

        public const uint STAGGER_LEVEL         = 0b_00_0000_1111_0000_0000_0000_0000;
        public const uint STAGGER_SOURCE        = 0b_00_0000_0000_1100_0000_0000_0000;
        public const uint STAGGER_DURATION      = 0b_00_0000_0000_0011_1111_1111_1111;

        public const uint PLAYER_STATE          = 0b_00_0000_0000_0000_0000_0000_0011;

        public const uint ENEMY_TYPE            = 0b_00_0000_0110_0000_0000_0000_0000;

        public const uint NEW_TALENT            = 0b_00_0000_0000_0000_0000_0001_1111;

        public const uint ROUND_RESULT          = 0b_00_0000_0000_0000_0000_0000_0011;
    }

    public class Bitshift
    {
        public const int EVENT_TYPE             = 2;

        public const int DIFFICULTY             = 13;
        public const int CAREER                 = 8;
        public const int CAMPAIGN               = 4;
        public const int MISSION                = 0;
                                          
        public const int WEAPON1                = 6;
        public const int WEAPON2                = 0;
                                          
        public const int TALENTS                = 0;

        public const int PERMANENT_HEALTH       = 10;
        public const int TEMPORARY_HEALTH       = 0;

        public const int CRIT                   = 20;
        public const int HEADSHOT               = 19;
        public const int TARGET                 = 17;
        public const int DAMAGE_SOURCE          = 15;
        public const int DAMAGE_TAKEN_SOURCE    = 15;
        public const int DAMAGE_INT             = 2;
        public const int DAMAGE_FRACTION        = 0;

        public const int STAGGER_LEVEL          = 16;
        public const int STAGGER_SOURCE         = 14;
        public const int STAGGER_DURATION       = 0;

        public const int PLAYER_STATE           = 0;
                                          
        public const int ENEMY_TYPE             = 17;
                                          
        public const int NEW_TALENT             = 0;
                                          
        public const int NEW_WEAPON             = 0;
                                          
        public const int ROUND_RESULT           = 0;
    }
}
