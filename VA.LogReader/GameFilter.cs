using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class GameFilter
    {
        private List<string> mGameVersion = new List<string>();
        public List<string> GameVersion
        {
            get => mGameVersion;
            set
            {
                mGameVersion = value;
                OnFilterChange?.Invoke(nameof(GameVersion));
            }
        }

        private List<DIFFICULTY> mDifficulty = new List<DIFFICULTY>();
        public List<DIFFICULTY> Difficulty
        {
            get => mDifficulty;
            set
            {
                mDifficulty = value;
                OnFilterChange?.Invoke(nameof(Difficulty));
            }
        }

        private List<CAREER> mCareer = new List<CAREER>();
        public List<CAREER> Career
        {
            get => mCareer;
            set
            {
                mCareer = value;
                OnFilterChange?.Invoke(nameof(Career));
            }
        }

        private bool? mDeathwish = null;
        public bool? Deathwish
        {
            get => mDeathwish;
            set
            {
                mDeathwish = value;
                OnFilterChange?.Invoke(nameof(Deathwish));
            }
        }

        private List<ONSLAUGHT_TYPE> mOnslaught = new List<ONSLAUGHT_TYPE>();
        public List<ONSLAUGHT_TYPE> Onslaught
        {
            get => mOnslaught;
            set
            {
                mOnslaught = value;
                OnFilterChange?.Invoke(nameof(Onslaught));
            }
        }

        private bool? mEmpowered = null;
        public bool? Empowered
        {
            get => mEmpowered;
            set
            {
                mEmpowered = value;
                OnFilterChange?.Invoke(nameof(Empowered));
            }
        }

        private List<MISSION> mMission = new List<MISSION>();
        public List<MISSION> Mission
        {
            get => mMission;
            set
            {
                mMission = value;
                OnFilterChange?.Invoke(nameof(Mission));
            }
        }

        public delegate void FilterChanged(string propName);
        public event FilterChanged OnFilterChange;

        public IEnumerable<GameHeader> Filter(IEnumerable<GameHeader> games) => games.Where(gh => IsMatch(gh));

        public bool IsMatch(GameHeader gh) =>
            MatchGameVersion(gh) &&
            MatchDifficulty(gh) &&
            MatchDeathwish(gh) &&
            MatchOnslaught(gh) &&
            MatchEmpowered(gh) &&
            MatchCareer(gh) &&
            MatchMission(gh);

        private bool MatchGameVersion(GameHeader gh) => GameVersion.Contains(gh.GameVersion);
        private bool MatchDifficulty(GameHeader gh) => Difficulty.Contains(gh.Difficulty);
        private bool MatchCareer(GameHeader gh) => Career.Contains(gh.Career);
        private bool MatchMission(GameHeader gh) => Mission.Contains(gh.Mission);
        private bool MatchOnslaught(GameHeader gh) => Onslaught.Contains(gh.Onslaught);

        private bool MatchDeathwish(GameHeader gh) =>
            !Deathwish.HasValue || gh.Deathwish == Deathwish.Value;

        private bool MatchEmpowered(GameHeader gh) =>
            !Empowered.HasValue || gh.Empowered == Empowered.Value;

        private const string ALL_GAMES = "ALL GAMES";
        private const string ENABLED = "enabled";
        private const string DISABLED = "disabled";

        public override string ToString()
        {
            List<string> output = new List<string>();
            var version = GameVersionToString();
            if (version != null)
            {
                output.Add(version);
            }
            var diff = DifficultyToString();
            if (diff != null)
            {
                output.Add(diff);
            }
            var career = CareerToString();
            if (career != null)
            {
                output.Add(career);
            }
            var mission = MissionToString();
            if (mission != null)
            {
                output.Add(mission);
            }
            var dw = DeathwishToString();
            if (dw != null)
            {
                output.Add(dw);
            }
            var ons = OnslaughtToString();
            if (ons != null)
            {
                output.Add(ons);
            }
            var emp = EmpoweredToString();
            if (emp != null)
            {
                output.Add(emp);
            }

            if(!output.Any())
            {
                output.Add(ALL_GAMES);
            }

            return string.Join(" AND ", output);
        }

        public static Regex FilterRegex { get; } = new Regex(@"^((?:(?:\w+ in \(\S+\)|\w+ (?:en|dis)abled|NO \w+]) AND )+(?:\w+ in \(\S+\)|\w+ (?:en|dis)abled|NO \w+])|ALL GAMES)$");

        public static GameFilter FromString(string input)
        {
            return new GameFilter()
            {
                GameVersion = ReadGameVersion(input),
                Difficulty = ReadDifficulty(input),
                Career = ReadCareer(input),
                Mission = ReadMission(input),
                Deathwish = ReadDeathwish(input),
                Onslaught = ReadOnslaught(input),
                Empowered = ReadEmpowered(input)
            };
        }

        public void UpdateFromString(string input)
        {
            GameVersion.Clear();
            GameVersion.AddRange(ReadGameVersion(input));

            Difficulty.Clear();
            Difficulty.AddRange(ReadDifficulty(input));

            Career.Clear();
            Career.AddRange(ReadCareer(input));

            Mission.Clear();
            Mission.AddRange(ReadMission(input));

            Deathwish = ReadDeathwish(input);

            Onslaught.Clear();
            Onslaught.AddRange(ReadOnslaught(input));

            Empowered = ReadEmpowered(input);
        }

        private string GameVersionToString()
        {
            if (IsFilterSetAll(GameVersion, GameRepository.Instance.GameVersions)) return null;
            if (!GameVersion.Any()) return "NO GAME VERSION";

            return $"Game Version in ({GetFilterSetString(GameVersion)})";
        }

        private string DifficultyToString() => EnumListToString(nameof(Difficulty), Difficulty);
        private string CareerToString() => EnumListToString(nameof(Career), Career);
        private string MissionToString() => EnumListToString(nameof(Mission), Mission);
        private string DeathwishToString() => NullableBoolToString(nameof(Deathwish), Deathwish);
        private string OnslaughtToString() => EnumListToString(nameof(Onslaught), Onslaught);
        private string EmpoweredToString() => NullableBoolToString(nameof(Empowered), Empowered);

        private string EnumListToString<T>(string propertyName, List<T> propertyValue) where T : Enum
        {
            if (IsFilterSetAll(propertyValue)) return null;
            if (!propertyValue.Any()) return $"NO {propertyName.ToUpper()}";

            return $"{propertyName} in ({GetFilterSetString(propertyValue)})";
        }

        private string NullableBoolToString(string propertyName, bool? propertyValue)
        {
            if (!propertyValue.HasValue) return null;
            if (propertyValue.Value) return $"{propertyName} {ENABLED}";
            return $"{propertyName} {DISABLED}";
        }

        private static List<string> ReadGameVersion(string filterString)
        {
            if (filterString.Contains($"NO GAME VERSION"))
            {
                return new List<string>();
            }
            var match = Regex.Match(filterString, $@"Game Version in \((\S+)\)");
            if (match == null || match.Groups.Count < 2 || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                return GameRepository.Instance.GameVersions.ToList();
            }

            var result = new List<string>();
            var matchString = match.Groups[1].Value;
            var valueStrings = matchString.Split(new string[] { $"," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var val in valueStrings)
            {
                if (!string.IsNullOrEmpty(val))
                {
                    if(GameRepository.Instance.GameVersions.Contains(val))
                    {
                        result.Add(val);
                    }
                }
            }
            return result;
        }
        private static List<DIFFICULTY> ReadDifficulty(string filterString) => ReadEnumList<DIFFICULTY>(filterString, nameof(Difficulty));
        private static List<CAREER> ReadCareer(string filterString) => ReadEnumList<CAREER>(filterString, nameof(Career));
        private static List<MISSION> ReadMission(string filterString) => ReadEnumList<MISSION>(filterString, nameof(Mission));
        private static bool? ReadDeathwish(string filterString) => ReadNullableBool(filterString, nameof(Deathwish));
        private static List<ONSLAUGHT_TYPE> ReadOnslaught(string filterString) => ReadEnumList<ONSLAUGHT_TYPE>(filterString, nameof(Onslaught));
        private static bool? ReadEmpowered(string filterString) => ReadNullableBool(filterString, nameof(Empowered));

        private static List<T> ReadEnumList<T>(string filterString, string propertyName) where T : Enum
        {
            if(filterString.Contains($"NO {propertyName.ToUpper()}"))
            {
                return new List<T>();
            }
            var match = Regex.Match(filterString, $@"{propertyName} in \((\S+)\)");
            if(match == null || match.Groups.Count < 2 || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                return Enum.GetValues(typeof(T)).Cast<T>().ToList();
            }

            var result = new List<T>();
            var matchString = match.Groups[1].Value;
            var valueStrings = matchString.Split(new string[] { $"," }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var val in valueStrings)
            {
                var parsed = Enum.Parse(typeof(T), val);
                if (parsed != null && Enum.IsDefined(typeof(T), parsed))
                {
                    result.Add((T)parsed);
                }
            }
            return result;
        }

        private static bool? ReadNullableBool(string filterString, string propertyName)
        {
            if (filterString.Contains($"{propertyName} {ENABLED}"))
            {
                return true;
            }
            if (filterString.Contains($"{propertyName} {DISABLED}"))
            {
                return false;
            }

            return null;
        }

        private bool IsFilterSetAll<T>(IEnumerable<T> selectorVal, IEnumerable<T> allValues)
        {
            foreach(var val in allValues)
            {
                if(!selectorVal.Contains(val))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsFilterSetAll<T>(List<T> selectorVal) where T : Enum
        {
            foreach(var val in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if(!selectorVal.Contains(val))
                {
                    return false;
                }
            }
            return true;
        }

        private string GetFilterSetString<T>(List<T> selectorVal)
        {
            string set = string.Empty;
            foreach (var flag in selectorVal)
            {
                set += $"{flag},";
            }
            return set.Trim(',');
        }

        public static IEnumerable<string> FilterOptions(Type enumType, params Enum[] exclusions)
        {
            var list = Enum.GetValues(enumType)
                        .Cast<Enum>()
                        .Where(val => !exclusions.Contains(val))
                        .Select(val => val.ForDisplay());

            return list;
        }
    }
}
