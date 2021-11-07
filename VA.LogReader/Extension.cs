using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        public static string ForDisplay(this Enum enumVal) => Enum.GetName(enumVal.GetType(), enumVal).SplitCamelCase();
        public static T FromDisplay<T>(this string str) where T : Enum => (T)Enum.Parse(typeof(T), str.Replace(" ", ""));
        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

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
        public static bool IsFootKnight(this CAREER c) => c == CAREER.FootKnight;
        public static bool IsGrailKnight(this CAREER c) => c == CAREER.GrailKnight;

        public static bool IsRangerVeteran(this CAREER c) => c == CAREER.RangerVeteran;
        public static bool IsIronbreaker(this CAREER c) => c == CAREER.Ironbreaker;
        public static bool IsSlayer(this CAREER c) => c == CAREER.Slayer;
        public static bool IsOutcastEngineer(this CAREER c) => c == CAREER.OutcastEngineer;

        public static bool IsWaystalker(this CAREER c) => c == CAREER.Waystalker;
        public static bool IsHandmaiden(this CAREER c) => c == CAREER.Handmaiden;
        public static bool IsShade(this CAREER c) => c == CAREER.Shade;
        public static bool IsSisterOfTheThorn(this CAREER c) => c == CAREER.SisterOfTheThorn;

        public static bool IsWitchHunterCaptain(this CAREER c) => c == CAREER.WitchHunterCaptain;
        public static bool IsBountyHunter(this CAREER c) => c == CAREER.BountyHunter;
        public static bool IsZealot(this CAREER c) => c == CAREER.Zealot;
        public static bool IsSaltzpyreUNKNOWN(this CAREER c) => c == CAREER.SaltzpyreUNKNOWN;

        public static bool IsBattleWizard(this CAREER c) => c == CAREER.BattleWizard;
        public static bool IsPyromancer(this CAREER c) => c == CAREER.Pyromancer;
        public static bool IsUnchained(this CAREER c) => c == CAREER.Unchained;
        public static bool IsSiennaUNKNOWN(this CAREER c) => c == CAREER.SiennaUNKNOWN;

        public static HERO Hero(this CAREER career) =>
            (HERO)((byte)career / 4);
        #endregion

        #region MISSION
        public static CAMPAIGN Campaign(this MISSION m)
        {
            long l = (long)m;
            if(l > 10000)
            {
                return CAMPAIGN.Misc;
            }
            if (l > 600)
            {
                return CAMPAIGN.Weave;
            }
            if (l > 500)
            {
                return CAMPAIGN.ChaosWastes;
            }
            if (l > 400)
            {
                return CAMPAIGN.WindsOfMagic;
            }
            if (l > 300)
            {
                return CAMPAIGN.Ubersreik;
            }
            if (l > 200)
            {
                return CAMPAIGN.Bogenhafen;
            }
            if (l > 100)
            {
                return CAMPAIGN.Drachenfels;
            }
            if (l > 0)
            {
                return CAMPAIGN.Helmgart;
            }
            return CAMPAIGN.Unknown;
        }
        public static bool IsChaosWastes(this MISSION m) => m.Campaign() == CAMPAIGN.ChaosWastes;
        #endregion

        #region DAMAGE_TAKEN_SOURCE
        public static bool IsFriendlyFire(this DAMAGE_TAKEN_SOURCE source) => source == DAMAGE_TAKEN_SOURCE.Ally;
        #endregion

        #region ROUND_RESULT
        public static bool IsLoss(this ROUND_RESULT r) => r == ROUND_RESULT.Loss;
        public static bool IsQuit(this ROUND_RESULT r) => r == ROUND_RESULT.Quit;
        public static bool IsWin(this ROUND_RESULT r) => r == ROUND_RESULT.Win;
        #endregion

        #region PROPERTY
        public static bool IsPercent(this PROPERTY property)
        {
            switch(property)
            {
                case PROPERTY.Stamina:
                    return false;
                default:
                    return true;
            }
        }
        #endregion

        #region ParseError
        public static bool HasError(this ParseError error) => error != ParseError.None;
        #endregion

        #region FileInfo
        public static string NameWithoutExtension(this FileInfo fi) => fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
        #endregion
    }
}
