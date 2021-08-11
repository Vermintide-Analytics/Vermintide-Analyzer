using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public static class Extension
    {
        public static IEnumerable<Enum> GetFlags(this Enum flags)
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
        public static string ForDisplay(this Enum enumVal) => Enum.GetName(enumVal.GetType(), enumVal).Replace("_", " ");
        public static T FromDisplay<T>(this string str) where T : Enum => (T)Enum.Parse(typeof(T), str.Replace(" ", "_"));

        #region DIFFICULTY
        public static bool IsRecruit(this DIFFICULTY d) => d == DIFFICULTY.Recruit;
        public static bool IsVeteran(this DIFFICULTY d) => d == DIFFICULTY.Veteran;
        public static bool IsChampion(this DIFFICULTY d) => d == DIFFICULTY.Champion;
        public static bool IsLegend(this DIFFICULTY d) => d == DIFFICULTY.Legend;
        public static bool IsCataclysm(this DIFFICULTY d) => d == DIFFICULTY.Cataclysm;
        public static bool IsCataclysm2(this DIFFICULTY d) => d == DIFFICULTY.Cataclysm2;
        public static bool IsCataclysm3(this DIFFICULTY d) => d == DIFFICULTY.Cataclysm3;
        #endregion

        #region HERO
        public static string Name(this HERO c) => Enum.GetName(typeof(HERO), c);
        #endregion

        #region CAREER
        public static bool IsMercenary(this CAREER c) => c == CAREER.Mercenary;
        public static bool IsHuntsman(this CAREER c) => c == CAREER.Huntsman;
        public static bool IsFoot_Knight(this CAREER c) => c == CAREER.Foot_Knight;
        public static bool IsGrail_Knight(this CAREER c) => c == CAREER.Grail_Knight;

        public static bool IsRanger_Veteran(this CAREER c) => c == CAREER.Ranger_Veteran;
        public static bool IsIronbreaker(this CAREER c) => c == CAREER.Ironbreaker;
        public static bool IsSlayer(this CAREER c) => c == CAREER.Slayer;
        public static bool IsOutcast_Engineer(this CAREER c) => c == CAREER.Outcast_Engineer;

        public static bool IsWaystalker(this CAREER c) => c == CAREER.Waystalker;
        public static bool IsHandmaiden(this CAREER c) => c == CAREER.Handmaiden;
        public static bool IsShade(this CAREER c) => c == CAREER.Shade;
        public static bool IsSister_of_the_Thorn(this CAREER c) => c == CAREER.Sister_of_the_Thorn;

        public static bool IsWitch_Hunter_Captain(this CAREER c) => c == CAREER.Witch_Hunter_Captain;
        public static bool IsBounty_Hunter(this CAREER c) => c == CAREER.Bounty_Hunter;
        public static bool IsZealot(this CAREER c) => c == CAREER.Zealot;
        public static bool IsSaltzpyre_UNKNOWN(this CAREER c) => c == CAREER.Saltzpyre_UNKNOWN;

        public static bool IsBattle_Wizard(this CAREER c) => c == CAREER.Battle_Wizard;
        public static bool IsPyromancer(this CAREER c) => c == CAREER.Pyromancer;
        public static bool IsUnchained(this CAREER c) => c == CAREER.Unchained;
        public static bool IsSienna_UNKNOWN(this CAREER c) => c == CAREER.Sienna_UNKNOWN;

        public static HERO Hero(this CAREER career) =>
            (HERO)((byte)career / 4);
        #endregion
        
        #region CAMPAIGN
        public static int MissionEnumShift(this CAMPAIGN c)
        {
            switch (c)
            {
                case CAMPAIGN.Misc: return Enums.MISC_MISSION_SHIFT;
                case CAMPAIGN.Helmgart: return Enums.HELMGART_MISSION_SHIFT;
                case CAMPAIGN.Drachenfels: return Enums.DRACHENFELS_MISSION_SHIFT;
                case CAMPAIGN.Bogenhafen: return Enums.BOGENHAFEN_MISSION_SHIFT;
                case CAMPAIGN.Ubersreik: return Enums.UBERSREIK_MISSION_SHIFT;
                case CAMPAIGN.Winds_of_Magic: return Enums.WOM_MISSION_SHIFT;
                case CAMPAIGN.Chaos_Wastes: return Enums.CHAOS_WASTES_MISSION_SHIFT;
                case CAMPAIGN.Weave: return Enums.WEAVES_MISSION_SHIFT;
            }
            return 0;
        }
        #endregion

        #region MISSION
        public static CAMPAIGN Campaign(this MISSION m)
        {
            long l = (long)m;
            if (l >> Enums.WEAVES_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Weave;
            }
            if (l >> Enums.CHAOS_WASTES_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Chaos_Wastes;
            }
            if (l >> Enums.WOM_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Winds_of_Magic;
            }
            if (l >> Enums.UBERSREIK_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Ubersreik;
            }
            if (l >> Enums.BOGENHAFEN_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Bogenhafen;
            }
            if (l >> Enums.DRACHENFELS_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Drachenfels;
            }
            if (l >> Enums.HELMGART_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Helmgart;
            }
            if (l >> Enums.MISC_MISSION_SHIFT > 0)
            {
                return CAMPAIGN.Misc;
            }
            return CAMPAIGN.Unknown;
        }
        public static bool IsChaosWastes(this MISSION m) => m.Campaign() == CAMPAIGN.Chaos_Wastes;
        #endregion

        #region DAMAGE_TAKEN_SOURCE
        public static bool IsFriendlyFire(this DAMAGE_TAKEN_SOURCE source) => source == DAMAGE_TAKEN_SOURCE.Ally;
        #endregion

        #region ROUND_RESULT
        public static bool IsLoss(this ROUND_RESULT r) => r == ROUND_RESULT.Loss;
        public static bool IsQuit(this ROUND_RESULT r) => r == ROUND_RESULT.Quit;
        public static bool IsWin(this ROUND_RESULT r) => r == ROUND_RESULT.Win;
        #endregion

        #region ParseError
        public static bool HasError(this ParseError error) => error != ParseError.None;
        #endregion

        #region FileInfo
        public static string NameWithoutExtension(this FileInfo fi) => fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
        #endregion
    }
}
