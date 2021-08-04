using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VA.LogReader;

namespace Vermintide_Analyzer.Models
{

    public class GameItem
    {
        public Game Game { get; set; }
        public bool HasCustomNotes => GameRepository.Instance.GameNotes.ContainsKey(Game.FilePath);
        public string CustomNotes => HasCustomNotes ? GameRepository.Instance.GameNotes[Game.FilePath] : string.Empty;

        public GameItem(Game g)
        {
            Game = g;
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
