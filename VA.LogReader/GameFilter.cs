using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override string ToString()
        {
            List<string> output = new List<string>();
            var version = GameVersionString;
            if (version != null)
            {
                output.Add(version);
            }
            var diff = DifficultyString;
            if (diff != null)
            {
                output.Add(diff);
            }
            var career = CareerString;
            if (career != null)
            {
                output.Add(career);
            }
            var mission = MissionString;
            if (mission != null)
            {
                output.Add(mission);
            }
            var dw = DeathwishString;
            if (dw != null)
            {
                output.Add(dw);
            }
            var ons = OnslaughtString;
            if (ons != null)
            {
                output.Add(ons);
            }
            var emp = EmpoweredString;
            if (emp != null)
            {
                output.Add(emp);
            }

            if(!output.Any())
            {
                output.Add("ALL GAMES");
            }

            return string.Join(" AND ", output);
        }

        private string GameVersionString
        {
            get
            {
                if (IsFilterSetAll(GameVersion, GameRepository.Instance.GameVersions)) return null;
                if (!GameVersion.Any()) return "NO GAME VERSION";

                return $"Game Version in ({GetFilterSetString(GameVersion)})";
            }
        }

        private string DifficultyString
        {
            get
            {
                if (IsFilterSetAll(Difficulty)) return null;
                if (!Difficulty.Any()) return "NO DIFFICULTY";

                return $"Difficulty in ({GetFilterSetString(Difficulty)})";
            }
        }

        private string CareerString
        {
            get
            {
                if (IsFilterSetAll(Career)) return null;
                if (!Career.Any()) return "NO CAREER";

                return $"Career in ({GetFilterSetString(Career)})";
            }
        }
        private string MissionString
        {
            get
            {
                if (IsFilterSetAll(Mission)) return null;
                if (!Mission.Any()) return "NO MISSION";

                return $"Mission in ({GetFilterSetString(Mission)})";
            }
        }

        private string DeathwishString
        {
            get
            {
                if (!Deathwish.HasValue) return null;
                if (Deathwish.Value) return "Deathwish enabled";
                return $"Deathwish disabled";
            }
        }

        private string OnslaughtString
        {
            get
            {
                if (IsFilterSetAll(Onslaught)) return null;
                if (!Onslaught.Any()) return "No Onslaught";

                return $"Onslaught in ({GetFilterSetString(Onslaught)})";
            }
        }

        private string EmpoweredString
        {
            get
            {
                if (!Empowered.HasValue) return null;
                if (Empowered.Value) return "Empowered enabled";
                return $"Empowered disabled";
            }
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
                set += $"{flag},\u200B"; // \u200B is a space that doesn't take up any space. This is useful for helping text wrapping
            }
            return set.Trim(',', '\u200B');
        }
    }
}
