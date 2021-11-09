using VA.LogReader;
using Vermintide_Analyzer.Statistics;

namespace Vermintide_Analyzer.Models
{
    public class GameItem
    {
        public Game Game { get; set; }
        public GameStats Stats { get; set; }
        public bool HasCustomNotes => GameRepository.Instance.GameNotes.ContainsKey(Game.FilePath);
        public string CustomNotes => HasCustomNotes ? GameRepository.Instance.GameNotes[Game.FilePath] : string.Empty;

        public GameItem(Game g)
        {
            Game = g;
            Stats = new GameStats(Game);
            Stats.RecalculateStats();
        }
    }

    public class GameHeaderItem
    {
        public GameHeader GameHeader { get; set; }
        public bool HasCustomNotes => GameRepository.Instance.GameNotes.ContainsKey(GameHeader.FilePath);
        public string CustomNotes => HasCustomNotes ? GameRepository.Instance.GameNotes[GameHeader.FilePath] : string.Empty;
        public bool IsNew => GameRepository.Instance.NewGameHeaders.Contains(GameHeader);

        public GameHeaderItem(GameHeader header)
        {
            GameHeader = header;
        }
    }
}
