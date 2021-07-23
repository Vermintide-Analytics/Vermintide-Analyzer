using System;
using System.Collections.Generic;
using System.Text;

namespace VA.LogReader
{
    public class Round_Start : Event
    {
        public DIFFICULTY Difficulty { get; private set; }
        public CAREER Career { get; private set; }
        public CAMPAIGN Campaign { get; private set; }
        public byte Mission { get; private set; }

        private Round_Start(uint payload)
        {
            Difficulty = (DIFFICULTY)((payload & Bitmask.DIFFICULTY) >> Bitshift.DIFFICULTY);
            Career = (CAREER)((payload & Bitmask.CAREER) >> Bitshift.CAREER);
            byte campaignVal = (byte)((payload & Bitmask.CAMPAIGN) >> Bitshift.CAMPAIGN);
            if(Enum.IsDefined(typeof(CAMPAIGN), campaignVal))
            {
                Campaign = (CAMPAIGN)campaignVal;
            }
            else
            {
                Campaign = CAMPAIGN.Unknown;
            }

            Mission = (byte)((payload & Bitmask.MISSION) >> Bitshift.MISSION);
        }

        public static Event Create(uint payload) => new Round_Start(payload);
    }

    public class Player_State : Event
    {
        public PLAYER_STATE State { get; private set; }

        private Player_State(uint payload)
        {
            State = (PLAYER_STATE)((payload & Bitmask.PLAYER_STATE) >> Bitshift.PLAYER_STATE);
        }

        public static Event Create(uint payload) => new Player_State(payload);
    }


    public class Current_Health : Event
    {
        public float PermanentHealth { get; private set; }
        public float TemporaryHealth { get; private set; }

        private Current_Health(uint payload)
        {
            PermanentHealth = ((payload & Bitmask.PERMANENT_HEALTH) >> Bitshift.PERMANENT_HEALTH);
            TemporaryHealth = ((payload & Bitmask.TEMPORARY_HEALTH) >> Bitshift.TEMPORARY_HEALTH);
        }

        public static Event Create(uint payload) => new Current_Health(payload);
    }
    
    public class Damage_Dealt : Event
    {
        public bool Crit { get; private set; }
        public bool Headshot { get; private set; }
        public DAMAGE_TARGET Target { get; private set; }
        public DAMAGE_SOURCE Source { get; private set; }
        public float Damage { get; private set; }

        private Damage_Dealt(uint payload)
        {
            Crit = ((payload & Bitmask.CRIT) >> Bitshift.CRIT) == 1;
            Headshot = ((payload & Bitmask.HEADSHOT) >> Bitshift.HEADSHOT) == 1;
            Target = (DAMAGE_TARGET)((payload & Bitmask.TARGET) >> Bitshift.TARGET);
            Source = (DAMAGE_SOURCE)((payload & Bitmask.DAMAGE_SOURCE) >> Bitshift.DAMAGE_SOURCE);
            Damage = ((payload & Bitmask.DAMAGE_INT) >> Bitshift.DAMAGE_INT) + ((payload & Bitmask.DAMAGE_FRACTION) >> Bitshift.DAMAGE_FRACTION) / 4.0f;
        }
        public static Event Create(uint payload) => new Damage_Dealt(payload);
    }

    public class Temp_HP_Gained : Event
    {
        public float UncappedHeal { get; private set; }
        public float CappedHeal { get; private set; }

        private Temp_HP_Gained(uint payload)
        {
            UncappedHeal = ((payload & Bitmask.UNCAPPED_HEAL_INT) >> Bitshift.UNCAPPED_HEAL_INT) + ((payload & Bitmask.UNCAPPED_HEAL_FRACTION) >> Bitshift.UNCAPPED_HEAL_FRACTION) / 4.0f;
            CappedHeal = ((payload & Bitmask.CAPPED_HEAL_INT) >> Bitshift.CAPPED_HEAL_INT) + ((payload & Bitmask.CAPPED_HEAL_FRACTION) >> Bitshift.CAPPED_HEAL_FRACTION) / 4.0f;
        }

        public static Event Create(uint payload) => new Temp_HP_Gained(payload);
    }

    public class Damage_Taken : Event
    {
        public DAMAGE_TAKEN_SOURCE Source { get; private set; }
        public float Damage { get; private set; }

        private Damage_Taken(uint payload)
        {
            Source = (DAMAGE_TAKEN_SOURCE)((payload & Bitmask.DAMAGE_TAKEN_SOURCE) >> Bitshift.DAMAGE_TAKEN_SOURCE);
            Damage = ((payload & Bitmask.DAMAGE_INT) >> Bitshift.DAMAGE_INT) + ((payload & Bitmask.DAMAGE_FRACTION) >> Bitshift.DAMAGE_FRACTION) / 4.0f;
        }

        public static Event Create(uint payload) => new Damage_Taken(payload);
    }

    public class Enemy_Killed : Event
    {
        public ENEMY_TYPE EnemyType { get; private set; }
        public DAMAGE_SOURCE Source { get; private set; }
        public float OverkillDamage { get; private set; }

        private Enemy_Killed(uint payload)
        {
            EnemyType = (ENEMY_TYPE)((payload & Bitmask.ENEMY_TYPE) >> Bitshift.ENEMY_TYPE);
            Source = (DAMAGE_SOURCE)((payload & Bitmask.DAMAGE_SOURCE) >> Bitshift.DAMAGE_SOURCE);
            OverkillDamage = ((payload & Bitmask.DAMAGE_INT) >> Bitshift.DAMAGE_INT) + ((payload & Bitmask.DAMAGE_FRACTION) >> Bitshift.DAMAGE_FRACTION) / 4.0f;
        }

        public static Event Create(uint payload) => new Enemy_Killed(payload);
    }

    public class Enemy_Staggered : Event
    {
        public byte StaggerLevel { get; private set; }
        public STAGGER_SOURCE Source { get; private set; }
        public float StaggerDuration { get; private set; }

        private Enemy_Staggered(uint payload)
        {
            StaggerLevel = (byte)((payload & Bitmask.STAGGER_LEVEL) >> Bitshift.STAGGER_LEVEL);
            Source = (STAGGER_SOURCE)((payload & Bitmask.STAGGER_SOURCE) >> Bitshift.STAGGER_SOURCE);
            StaggerDuration = ((payload & Bitmask.STAGGER_DURATION) >> Bitshift.STAGGER_DURATION) / 1000f;
        }

        public static Event Create(uint payload) => new Enemy_Staggered(payload);
    }

    public class Talent_Tree : Event
    {
        public bool R1C1 { get; private set; }
        public bool R1C2 { get; private set; }
        public bool R1C3 { get; private set; }

        public bool R2C1 { get; private set; }
        public bool R2C2 { get; private set; }
        public bool R2C3 { get; private set; }

        public bool R3C1 { get; private set; }
        public bool R3C2 { get; private set; }
        public bool R3C3 { get; private set; }

        public bool R4C1 { get; private set; }
        public bool R4C2 { get; private set; }
        public bool R4C3 { get; private set; }

        public bool R5C1 { get; private set; }
        public bool R5C2 { get; private set; }
        public bool R5C3 { get; private set; }

        public bool R6C1 { get; private set; }
        public bool R6C2 { get; private set; }
        public bool R6C3 { get; private set; }

        private Talent_Tree(uint payload)
        {
            R1C1 = payload % 2 > 0; payload >>= 1;
            R1C2 = payload % 2 > 0; payload >>= 1;
            R1C3 = payload % 2 > 0; payload >>= 1;

            R2C1 = payload % 2 > 0; payload >>= 1;
            R2C2 = payload % 2 > 0; payload >>= 1;
            R2C3 = payload % 2 > 0; payload >>= 1;

            R3C1 = payload % 2 > 0; payload >>= 1;
            R3C2 = payload % 2 > 0; payload >>= 1;
            R3C3 = payload % 2 > 0; payload >>= 1;

            R4C1 = payload % 2 > 0; payload >>= 1;
            R4C2 = payload % 2 > 0; payload >>= 1;
            R4C3 = payload % 2 > 0; payload >>= 1;

            R5C1 = payload % 2 > 0; payload >>= 1;
            R5C2 = payload % 2 > 0; payload >>= 1;
            R5C3 = payload % 2 > 0; payload >>= 1;

            R6C1 = payload % 2 > 0; payload >>= 1;
            R6C2 = payload % 2 > 0; payload >>= 1;
            R6C3 = payload % 2 > 0;
        }

        public static Event Create(uint payload) => new Talent_Tree(payload);
    }

    public class Weapon_Set : Event
    {
        public byte Weapon1 { get; private set; }
        public byte Weapon2 { get; private set; }

        private Weapon_Set(uint payload)
        {
            Weapon2 = (byte)((payload & Bitmask.WEAPON2) >> Bitshift.WEAPON2);
            Weapon1 = (byte)((payload & Bitmask.WEAPON1) >> Bitshift.WEAPON1);
        }

        public static Event Create(uint payload) => new Weapon_Set(payload);
    }

    public class Round_End : Event
    {
        public ROUND_RESULT Result { get; private set; }

        private Round_End(uint payload)
        {
            Result = (ROUND_RESULT)((payload & Bitmask.ROUND_RESULT) >> Bitshift.ROUND_RESULT);
        }

        public static Event Create(uint payload) => new Round_End(payload);
    }

}
