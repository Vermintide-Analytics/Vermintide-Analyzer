﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VA.LogReader;

namespace Vermintide_Analyzer
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
        private static readonly string GameNotesFilePath = Path.Combine(DataDir, "Custom-Game-Notes.txt");
        private static readonly string GameFiltersFilePath = Path.Combine(DataDir, "Game-Filters.txt");

        public List<GameHeader> GameHeaders { get; private set; } = new List<GameHeader>();
        public List<GameHeader> NewGameHeaders { get; private set; } = new List<GameHeader>();

        public IEnumerable<string> GameVersions => GameHeaders
            .OrderBy(gh => gh.GameVersionMajor)
            .ThenBy(gh => gh.GameVersionMinor)
            .Select(gh => gh.GameVersion)
            .Distinct();

        public List<InvalidGame> InvalidGames { get; private set; } = new List<InvalidGame>();

        public Dictionary<string, string> GameNotes { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> GameFilters { get; private set; } = new Dictionary<string, string>();

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

        public void WriteGameNotesToDisk() => File.WriteAllLines(GameNotesFilePath, GameNotes.Select(kvp => $"{kvp.Key},{kvp.Value}"));

        public void ReadGameNotesFromDisk()
        {
            GameNotes.Clear();

            if(!File.Exists(GameNotesFilePath))
            {
                return;
            }

            foreach(var line in File.ReadAllLines(GameNotesFilePath))
            {
                var lineSegments = line.Split(new char[] { ',' }, 2);
                if(lineSegments.Length > 1)
                {
                    GameNotes.Add(lineSegments[0], lineSegments[1]);
                }
            }
        }

        public void WriteGameFiltersToDisk() => File.WriteAllLines(GameFiltersFilePath, GameFilters.Select(kvp => $"{kvp.Key},{kvp.Value}"));

        public void ReadGameFiltersFromDisk()
        {
            GameFilters.Clear();

            if (!File.Exists(GameFiltersFilePath))
            {
                return;
            }

            foreach (var line in File.ReadAllLines(GameFiltersFilePath))
            {
                var lineSegments = line.Split(new char[] { ',' }, 2);
                if (lineSegments.Length > 1)
                {
                    GameFilters.Add(lineSegments[0], lineSegments[1]);
                }
            }
        }

        public void DeleteGame(GameHeader game, bool doMiscIO = true)
        {
            if(GameNotes.ContainsKey(game.FilePath))
            {
                GameNotes.Remove(game.FilePath);
                if(doMiscIO)
                {
                    WriteGameNotesToDisk();
                }
            }
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
                DeleteGame(gh, false);
            }
            WriteGameNotesToDisk();
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