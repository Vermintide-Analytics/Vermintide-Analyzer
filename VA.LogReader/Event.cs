using System;
using System.Collections.Generic;

namespace VA.LogReader
{
    public abstract class Event
    {
        public const int BYTES = 6;

        public float Time { get; set; }

        public static Event CreateEvent(string timestamp, string eventName, string payload)
        {
            string[] payloadValues = payload.Split(',');

            Event result = null;
            if(EventFactories.TryGetValue(eventName, out var factory))
            {
                result = factory.Invoke(payloadValues);
            }

            if(result != null)
            {
                result.Time = float.Parse(timestamp) / 100.0f;
            }

            return result;
        }

        public static readonly Dictionary<string, Func<string[], Event>> EventFactories = new Dictionary<string, Func<string[], Event>>()
        {
            { "RoundStart", Round_Start.Create },
            { "RoundEnd", Round_End.Create },
            { "WeaponSet", Weapon_Set.Create },
            { "TalentTree", Talent_Tree.Create },
            { "PlayerState", Player_State.Create },
            { "CurrentHealth", Current_Health.Create },
            { "DamageDealt", Damage_Dealt.Create },
            { "EnemyStaggered", Enemy_Staggered.Create },
            { "EnemyKilled", Enemy_Killed.Create },
            { "DamageTaken", Damage_Taken.Create },
            { "TempHPGained", Temp_HP_Gained.Create },
            { "TraitGained", Trait_Gained.Create },
            { "PropertyGained", Property_Gained.Create },
        };
    }
}
