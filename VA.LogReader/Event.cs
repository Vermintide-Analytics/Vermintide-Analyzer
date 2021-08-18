using System;
using System.Collections.Generic;
using System.IO;

namespace VA.LogReader
{
    public abstract class Event
    {
        public const int BYTES = 6;

        public float RawTime { get; private set; }
        public float Time { get; set; }

        public EventType Type { get; private set; }

        public static EventType? GetEventType(byte data)
        {
            byte rawEventType = (byte)((data & Bitmask.EVENT_TYPE) >> Bitshift.EVENT_TYPE);
            if (Enum.IsDefined(typeof(EventType), rawEventType))
            {
                return (EventType)rawEventType;
            }

            return null;
        }

        public static Event CreateEvent(EventType eventType, byte[] bytes)
        {
            uint payload = (uint)((bytes[2] << 24) + (bytes[3] << 16) + (bytes[4] << 8) + (bytes[5]));
            payload &= Bitmask.PAYLOAD;

            Event result = null;
            switch (eventType)
            {
                case EventType.Current_Health:
                    result = Current_Health.Create(payload);
                    break;
                case EventType.Damage_Dealt:
                    result = Damage_Dealt.Create(payload);
                    break;
                case EventType.Enemy_Staggered:
                    result = Enemy_Staggered.Create(payload);
                    break;
                case EventType.Temp_HP_Gained:
                    result = Temp_HP_Gained.Create(payload);
                    break;
                case EventType.Enemy_Killed:
                    result = Enemy_Killed.Create(payload);
                    break;
                case EventType.Damage_Taken:
                    result = Damage_Taken.Create(payload);
                    break;
                case EventType.Player_State:
                    result = Player_State.Create(payload);
                    break;
                case EventType.Trait_Gained:
                    result = Trait_Gained.Create(payload);
                    break;
                case EventType.Property_Gained:
                    result = Property_Gained.Create(payload);
                    break;
                case EventType.Talent_Tree:
                    result = Talent_Tree.Create(payload);
                    break;
                case EventType.Weapon_Set:
                    result = Weapon_Set.Create(payload);
                    break;
                case EventType.Round_Start:
                    result = Round_Start.Create(payload);
                    break;
                case EventType.Round_End:
                    result = Round_End.Create(payload);
                    break;
            }

            if(result != null)
            {
                result.RawTime = ((bytes[0] << 8) + bytes[1]) / 100.0f;
                result.Type = eventType;
            }

            return result;
        }
    }

    public enum EventType : byte
    {
        Round_Start = 0,
        Round_End = 1,
        Weapon_Set = 2,
        Talent_Tree = 3,
        Player_State = 4,
        Current_Health = 5,
        Damage_Dealt = 6,
        Enemy_Staggered = 7,
        Enemy_Killed = 8,
        Damage_Taken = 9,
        Temp_HP_Gained = 10,
        Trait_Gained = 11,
        Property_Gained = 12,

        // 13 through 63 UNUSED
    }
}
