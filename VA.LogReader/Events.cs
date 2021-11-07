using System;

namespace VA.LogReader
{
    public class Round_Start : Event
    {
        public DIFFICULTY Difficulty { get; private set; }
        public CAREER Career { get; private set; }
        public CAMPAIGN Campaign { get; private set; }
        public MISSION Mission { get; private set; }

        private Round_Start(string[] payload)
        {
            Difficulty = (DIFFICULTY)Enum.Parse(typeof(DIFFICULTY), payload[0]);
            Career = (CAREER)Enum.Parse(typeof(CAREER), payload[1]);
            
            if(Enum.TryParse(payload[2], out CAMPAIGN camp))
            {
                Campaign = camp;
            }
            else
            {
                Campaign = CAMPAIGN.Unknown;
            }

            if (Enum.TryParse(payload[3], out MISSION mission))
            {
                Mission = mission;
            }
            else
            {
                Mission = MISSION.Unknown;
            }
        }

        public static Event Create(string[] payload) => new Round_Start(payload);
    }

    public class Player_State : Event
    {
        public PLAYER_STATE State { get; private set; }

        private Player_State(string[] payload)
        {
            State = (PLAYER_STATE)Enum.Parse(typeof(PLAYER_STATE), payload[0]);
        }

        public static Event Create(string[] payload) => new Player_State(payload);
    }


    public class Current_Health : Event
    {
        public float PermanentHealth { get; private set; }
        public float TemporaryHealth { get; private set; }

        private Current_Health(string[] payload)
        {
            PermanentHealth = float.Parse(payload[0]);
            TemporaryHealth = float.Parse(payload[1]);
        }

        public static Event Create(string[] payload) => new Current_Health(payload);
    }
    
    public class Damage_Dealt : Event
    {
        public bool Crit { get; private set; }
        public bool Headshot { get; private set; }
        public DAMAGE_TARGET Target { get; private set; }
        public DAMAGE_SOURCE Source { get; private set; }
        public float Damage { get; private set; }

        private Damage_Dealt(string[] payload)
        {
            Crit = payload[0] == "Crit";
            Headshot = payload[1] == "Headshot";
            Target = (DAMAGE_TARGET)Enum.Parse(typeof(DAMAGE_TARGET), payload[2]);
            Source = (DAMAGE_SOURCE)Enum.Parse(typeof(DAMAGE_SOURCE), payload[3]);
            Damage = float.Parse(payload[4]);
        }
        public static Event Create(string[] payload) => new Damage_Dealt(payload);
    }

    public class Temp_HP_Gained : Event
    {
        public float UncappedHeal { get; private set; }
        public float CappedHeal { get; private set; }

        private Temp_HP_Gained(string[] payload)
        {
            UncappedHeal = float.Parse(payload[0]);
            CappedHeal = float.Parse(payload[1]);
        }

        public static Event Create(string[] payload) => new Temp_HP_Gained(payload);
    }

    public class Damage_Taken : Event
    {
        public DAMAGE_TAKEN_SOURCE Source { get; private set; }
        public float Damage { get; private set; }

        private Damage_Taken(string[] payload)
        {
            Source = (DAMAGE_TAKEN_SOURCE)Enum.Parse(typeof(DAMAGE_TAKEN_SOURCE), payload[0]);
            Damage = float.Parse(payload[1]);
        }

        public static Event Create(string[] payload) => new Damage_Taken(payload);
    }

    public class Enemy_Killed : Event
    {
        public ENEMY_TYPE EnemyType { get; private set; }
        public DAMAGE_SOURCE Source { get; private set; }
        public float OverkillDamage { get; private set; }

        private Enemy_Killed(string[] payload)
        {
            EnemyType = (ENEMY_TYPE)Enum.Parse(typeof(ENEMY_TYPE), payload[0]);
            Source = (DAMAGE_SOURCE)Enum.Parse(typeof(DAMAGE_SOURCE), payload[1]);
            OverkillDamage = float.Parse(payload[2]);
        }

        public static Event Create(string[] payload) => new Enemy_Killed(payload);
    }

    public class Enemy_Staggered : Event
    {
        public byte StaggerLevel { get; private set; }
        public STAGGER_SOURCE Source { get; private set; }
        public float StaggerDuration { get; private set; }

        private Enemy_Staggered(string[] payload)
        {
            StaggerLevel = byte.Parse(payload[0]);
            Source = (STAGGER_SOURCE)Enum.Parse(typeof(STAGGER_SOURCE), payload[1]);
            StaggerDuration = float.Parse(payload[2]) / 1000f;
        }

        public static Event Create(string[] payload) => new Enemy_Staggered(payload);
    }

    public class Talent_Tree : Event
    {
        public bool R1C1 => TalentsArray[0];
        public bool R1C2 => TalentsArray[1];
        public bool R1C3 => TalentsArray[2];

        public bool R2C1 => TalentsArray[3];
        public bool R2C2 => TalentsArray[4];
        public bool R2C3 => TalentsArray[5];

        public bool R3C1 => TalentsArray[6];
        public bool R3C2 => TalentsArray[7];
        public bool R3C3 => TalentsArray[8];

        public bool R4C1 => TalentsArray[9];
        public bool R4C2 => TalentsArray[10];
        public bool R4C3 => TalentsArray[11];

        public bool R5C1 => TalentsArray[12];
        public bool R5C2 => TalentsArray[13];
        public bool R5C3 => TalentsArray[14];

        public bool R6C1 => TalentsArray[15];
        public bool R6C2 => TalentsArray[16];
        public bool R6C3 => TalentsArray[17];


        private bool[] TalentsArray = new bool[6*3];

        private Talent_Tree(string[] payload)
        {
            for(int count = 0; count < TalentsArray.Length; count++)
            {
                TalentsArray[count] = false;
            }

            foreach(var talent in payload)
            {
                TalentsArray[int.Parse(talent)] = true;
            }
        }

        public static Event Create(string[] payload) => new Talent_Tree(payload);
    }

    public class Weapon_Set : Event
    {
        public HERO Weapon1Owner { get; private set; }
        public HERO Weapon2Owner { get; private set; }
        
        public RARITY Weapon1Rarity { get; private set; }
        public RARITY Weapon2Rarity { get; private set; }

        public WEAPON Weapon1 { get; private set; }
        public WEAPON Weapon2 { get; private set; }

        private Weapon_Set(string[] payload)
        {
            Weapon1Owner = (HERO)Enum.Parse(typeof(HERO), payload[0]);
            Weapon2Owner = (HERO)Enum.Parse(typeof(HERO), payload[1]);

            Weapon1Rarity = (RARITY)Enum.Parse(typeof(RARITY), payload[2]);
            Weapon2Rarity = (RARITY)Enum.Parse(typeof(RARITY), payload[3]);

            Weapon1 = (WEAPON)Enum.Parse(typeof(WEAPON), payload[4]);
            Weapon2 = (WEAPON)Enum.Parse(typeof(WEAPON), payload[5]);
        }

        public static Event Create(string[] payload) => new Weapon_Set(payload);
    }

    public class Trait_Gained : Event
    {
        public TRAIT_SOURCE Source { get; private set; }
        public TRAIT Trait { get; private set; }

        private Trait_Gained(string[] payload)
        {
            Source = (TRAIT_SOURCE)Enum.Parse(typeof(TRAIT_SOURCE), payload[0]);
            Trait = (TRAIT)Enum.Parse(typeof(TRAIT), payload[1]);
        }

        public static Event Create(string[] payload) => new Trait_Gained(payload);
    }

    public class Property_Gained : Event
    {
        public PROPERTY_SOURCE Source { get; private set; }
        public PROPERTY Property { get; private set; }
        public float PropertyValue { get; private set; }

        private Property_Gained(string[] payload)
        {
            Source = (PROPERTY_SOURCE)Enum.Parse(typeof(PROPERTY_SOURCE), payload[0]);
            Property = (PROPERTY)Enum.Parse(typeof(PROPERTY), payload[1]);
            PropertyValue = float.Parse(payload[2]) / 10f;
        }

        public static Event Create(string[] payload) => new Property_Gained(payload);
    }

    public class Round_End : Event
    {
        public ROUND_RESULT Result { get; private set; }

        private Round_End(string[] payload)
        {
            Result = (ROUND_RESULT)Enum.Parse(typeof(ROUND_RESULT), payload[0]);
        }

        public static Event Create(string[] payload) => new Round_End(payload);
    }

}
