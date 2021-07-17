using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class GameHeader
    {
        public string FilePath { get; set; }
        public DateTime GameStart { get; set; }

        public ParseError Error { get; private set; } = ParseError.None;

        public byte SchemaVersionMajor { get; private set; }
        public byte SchemaVersionMinor { get; private set; }

        public byte GameVersionMajor { get; private set; }
        public byte GameVersionMinor { get; private set; }

        public bool Deathwish { get; private set; }
        public ONSLAUGHT_TYPE Onslaught { get; private set; }
        public bool Empowered { get; private set; }

        public DIFFICULTY Difficulty { get; private set; }
        public CAREER Career { get; private set; }
        public CAMPAIGN Campaign { get; private set; }
        public MISSION Mission { get; private set; }

        public ROUND_RESULT Result { get; private set; }


        public List<WeaponData> Weapon1Datas { get; private set; }
        public List<WeaponData> Weapon2Datas { get; private set; }

        public List<TalentTree> TalentTrees { get; private set; }

        #region For Display
        public bool IsNew => GameRepository.Instance.NewGameHeaders.Contains(this);
        public string GameVersion => $"{GameVersionMajor}.{GameVersionMinor}";

        public string CampaignName => Campaign.ForDisplay();

        public string MissionName => Mission.ToString().Replace("_", " ");

        public string MissionTooltip => Mission.GetAttributeOfType<DescriptionAttribute>()?.Description ?? "";

        public string CareerName => Career.ForDisplay();
        public string DifficultyName => Difficulty.ForDisplay();

        public string GameStartDate => GameStart.ToShortDateString();
        public string GameStartTime => GameStart.ToShortTimeString();

        public WeaponData StartingWeapon1 => Weapon1Datas.FirstOrDefault();
        public WeaponData StartingWeapon2 => Weapon2Datas.FirstOrDefault();
        #endregion

        #region From Game
        public static GameHeader FromGame(Game g) =>
            new GameHeader()
            {
                FilePath = g.FilePath,
                GameStart = g.GameStart,
                Error = g.Error,
                SchemaVersionMajor = g.SchemaVersionMajor,
                SchemaVersionMinor = g.SchemaVersionMinor,
                GameVersionMajor = g.GameVersionMajor,
                GameVersionMinor = g.GameVersionMinor,
                Deathwish = g.Deathwish,
                Onslaught = g.Onslaught,
                Empowered = g.Empowered,
                Difficulty = g.Difficulty,
                Career = g.Career,
                Campaign = g.Campaign,
                Mission = g.Mission,
                Result = g.Result,
                Weapon1Datas = g.Weapon1Datas.Select(d => d.GetHeaderData()).ToList(),
                Weapon2Datas = g.Weapon2Datas.Select(d => d.GetHeaderData()).ToList(),
                TalentTrees = g.TalentTrees
            };
        #endregion

        #region To Game
        public Game ToGame() => Game.FromFile(FilePath);
        #endregion
    }
}
