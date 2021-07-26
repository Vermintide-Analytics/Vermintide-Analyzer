using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class GameRepository
    {
        #region Singleton
        private static GameRepository mInstance;
        public static GameRepository Instance
        {
            get
            {
                if(mInstance == null)
                {
                    mInstance = new GameRepository();
                }
                return mInstance;
            }
        }
        #endregion

        private static readonly string NewGamesDir = Environment.ExpandEnvironmentVariables(Const.NEW_LOG_DIR);
        private static readonly string AppDataDir = Environment.ExpandEnvironmentVariables(Const.APP_DATA_DIR);
        private static readonly string AllGamesRootDir = Path.Combine(AppDataDir, Const.GAME_DIR);
        private static readonly string LocalGamesRootDir = Path.Combine(AllGamesRootDir, Schema.SCHEMA_VERSION);
        private static readonly string InvalidGamesDir = Path.Combine(AllGamesRootDir, "Invalid");
        private static readonly string DataDir = Path.Combine(AppDataDir, Const.DATA_DIR);

        public List<GameHeader> GameHeaders { get; private set; } = new List<GameHeader>();
        public List<GameHeader> NewGameHeaders { get; private set; } = new List<GameHeader>();

        public IEnumerable<string> GameVersions => GameHeaders
            .OrderBy(gh => gh.GameVersionMajor)
            .ThenBy(gh => gh.GameVersionMinor)
            .Select(gh => gh.GameVersion)
            .Distinct();

        public List<InvalidGame> InvalidGames { get; private set; } = new List<InvalidGame>();

        public bool CheckDirectories()
        {
            foreach (var difficultyName in Enum.GetNames(typeof(DIFFICULTY)))
            {
                foreach (var careerName in Enum.GetNames(typeof(CAREER)))
                {
                    if (!Directory.Exists(Path.Combine(LocalGamesRootDir, difficultyName, careerName)))
                    {
                        return false;
                    }
                }
            }
            if(!Directory.Exists(InvalidGamesDir))
            {
                return false;
            }
            if(!Directory.Exists(DataDir))
            {
                return false;
            }
            return true;
        }

        public void CreateDirectories()
        {
            // Initialize directories for game logs
            foreach (var difficultyName in Enum.GetNames(typeof(DIFFICULTY)))
            {
                foreach (var careerName in Enum.GetNames(typeof(CAREER)))
                {
                    Directory.CreateDirectory(Path.Combine(LocalGamesRootDir, difficultyName, careerName));
                }
            }
            Directory.CreateDirectory(InvalidGamesDir);
            Directory.CreateDirectory(DataDir);
        }


        public IEnumerable<GameHeader> ReadExistingGameHeaders()
        {
            var filePaths = Directory.GetFiles(LocalGamesRootDir, "*.VA", SearchOption.AllDirectories);
            var existingGameHeaders = filePaths.Select(path => GameHeader.FromGame(Game.FromFile(path)));

            existingGameHeaders = FilterInvalidGames(existingGameHeaders, true);

            GameHeaders.AddRange(existingGameHeaders);
            return existingGameHeaders;
        }

        public IEnumerable<GameHeader> ReadPreviouslyInvalidGameHeaders()
        {
            var filePaths = Directory.GetFiles(InvalidGamesDir, "*.VA");
            var existingGameHeaders = filePaths.Select(path => GameHeader.FromGame(Game.FromFile(path)));

            existingGameHeaders = FilterInvalidGames(existingGameHeaders.ToList(), false, true);

            GameHeaders.AddRange(existingGameHeaders);
            return existingGameHeaders;
        }

        public string GetTargetFolder(GameHeader gh) =>
            Path.Combine(
                LocalGamesRootDir,
                Enum.GetName(typeof(DIFFICULTY), gh.Difficulty),
                Enum.GetName(typeof(CAREER), gh.Career));

        public IEnumerable<GameHeader> ReadAndMoveNewGameHeaders()
        {
            var filePaths = Directory.GetFiles(NewGamesDir, "*.VA", SearchOption.TopDirectoryOnly);
            var newGameHeaders = filePaths
                .Where(path => !(new FileInfo(path).IsLocked()))
                .Select(path => GameHeader.FromGame(Game.FromFile(path)))
                .ToList();

            newGameHeaders = FilterInvalidGames(newGameHeaders, true).ToList();

            foreach (var gameHeader in newGameHeaders)
            {
                string fileName = gameHeader.GameStart.ToString(Game.LOG_DATE_TIME_FORMAT) + ".VA";

                string newLocation =
                    Path.Combine(
                        GetTargetFolder(gameHeader),
                        fileName);

                try
                {
                    File.Move(gameHeader.FilePath, newLocation);
                    gameHeader.FilePath = newLocation;
                }
                catch { }
            }

            GameHeaders.AddRange(newGameHeaders);
            NewGameHeaders.AddRange(newGameHeaders);

            return newGameHeaders;
        }

        public void DeleteAllGames()
        {
            foreach (var path in Directory.GetFiles(
                Environment.ExpandEnvironmentVariables(Path.Combine(Const.APP_DATA_DIR, Const.GAME_DIR)),
                "*.VA",
                SearchOption.AllDirectories))
            {
                new FileInfo(path).Delete();
            }

            GameHeaders.Clear();
            NewGameHeaders.Clear();
        }

        public void DeleteGame(GameHeader game)
        {
            new FileInfo(game.FilePath).Delete();
            GameHeaders.Remove(game);
            NewGameHeaders.Remove(game);
        }

        public void DeleteGames(IEnumerable<GameHeader> games)
        {
            // Make a list to avoid any possible concurrent modification exceptions
            var list = games.ToList();
            foreach(var gh in list)
            {
                DeleteGame(gh);
            }
        }

        public void DeleteInvalidGame(InvalidGame g)
        {
            InvalidGames.Remove(g);
            new FileInfo(g.FilePath).Delete();
        }

        public void DeleteInvalidGames()
        {
            foreach(var invalid in InvalidGames)
            {
                new FileInfo(invalid.FilePath).Delete();
            }

            InvalidGames.Clear();
        }

        private IEnumerable<GameHeader> FilterInvalidGames(IEnumerable<GameHeader> games, bool moveInvalid, bool moveValid = false)
        {
            var invalidGames = games.Where(g => g.Error.HasError());
            var valids = games.Except(invalidGames);
            var invalids = invalidGames.Select(gh => new InvalidGame(gh));

            if (moveInvalid)
            {
                foreach (var invalid in invalids)
                {
                    var fileInfo = new FileInfo(invalid.FilePath);
                    var fileName = fileInfo.Name;
                    var newFilePath = Path.Combine(InvalidGamesDir, fileName);
                    File.Move(invalid.FilePath, newFilePath);
                    invalid.FilePath = newFilePath;
                }
            }

            if(moveValid)
            {
                // Move valid items to their correct location
                foreach (var gameHeader in valids)
                {
                    string fileName = gameHeader.GameStart.ToString(Game.LOG_DATE_TIME_FORMAT) + ".VA";

                    string newLocation =
                        Path.Combine(
                            GetTargetFolder(gameHeader),
                            fileName);

                    try
                    {
                        File.Move(gameHeader.FilePath, newLocation);
                        gameHeader.FilePath = newLocation;
                    }
                    catch { }
                }
            }

            InvalidGames.AddRange(invalids);
            return games.Where(g => !g.Error.HasError());
        }
    }
}
