using System;
using System.Collections.Generic;
using System.IO;

namespace VA.LogReader
{
    public abstract class Event
    {
        public const int BYTES = 5;

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
            uint payload = (uint)((bytes[2] << 16) + (bytes[3] << 8) + (bytes[4]));
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
                case EventType.Enemy_Killed:
                    result = Enemy_Killed.Create(payload);
                    break;
                case EventType.Damage_Taken:
                    result = Damage_Taken.Create(payload);
                    break;
                case EventType.Player_State:
                    result = Player_State.Create(payload);
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
        Player_State = 1,
        Current_Health = 2,
        Damage_Dealt = 3,
        Damage_Taken = 4,
        Enemy_Killed = 5,
        Talent_Tree = 6,
        Weapon_Set = 7,
        Enemy_Staggered = 8,
        // UNUSED = 9,
        // UNUSED = 10,
        // UNUSED = 11,
        // UNUSED = 12,
        // UNUSED = 13,
        // UNUSED = 14,
        Round_End = 15
    }
}
