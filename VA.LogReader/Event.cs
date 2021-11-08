using System;
using System.Collections.Generic;
using System.Globalization;

namespace VA.LogReader
{
    public abstract class Event
    {
        // The data from the mod contains numbers in en-US culture, that is, commas are
        // optional thousands separators and periods separate integer and decimal portion.
        // We need to specify that we parse these numbers with this culture, else we read
        // numbers in wrong.
        public static readonly CultureInfo ParseCulture = CultureInfo.CreateSpecificCulture("en-US");

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

        #region Parse Utility
        protected byte GetByte(string input) => byte.Parse(input, ParseCulture);
        protected T GetEnum<T>(string input) => (T)Enum.Parse(typeof(T), input);
        protected float GetFloat(string input) => float.Parse(input, ParseCulture);
        protected int GetInt(string input) => int.Parse(input, ParseCulture);
        #endregion
    }
}
