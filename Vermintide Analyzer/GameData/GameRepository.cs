using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VA.LogReader;
using Microsoft.Win32;
using System.IO.Compression;
using Vermintide_Analyzer.Models;
using System.Globalization;

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

        private static readonly string ConsoleLogsDir = Environment.ExpandEnvironmentVariables(Const.CONSOLE_LOG_DIR);
        private static readonly string AppDataDir = Environment.ExpandEnvironmentVariables(Const.APP_DATA_DIR);
        private static readonly string AllGamesRootDir = Path.Combine(AppDataDir, Const.GAME_DIR);
        private static readonly string InvalidGamesDir = Path.Combine(AllGamesRootDir, "Invalid");
        private static readonly string DataDir = Path.Combine(AppDataDir, Const.DATA_DIR);
        public static readonly string TempDir = Path.Combine(AppDataDir, Const.TEMP_DIR);
        private static readonly string GameNotesFilePath = Path.Combine(DataDir, "Custom-Game-Notes.txt");
        private static readonly string GameFiltersFilePath = Path.Combine(DataDir, "Game-Filters.txt");
        private static readonly string LatestReadLogDateFilePath = Path.Combine(DataDir, "Latest-Read-Log.txt");

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

        public bool CheckDirectories() =>
            Directory.Exists(InvalidGamesDir) &&
            Directory.Exists(DataDir) &&
            Directory.Exists(TempDir);

        public void CreateDirectories()
        {
            Directory.CreateDirectory(InvalidGamesDir);
            Directory.CreateDirectory(DataDir);
            Directory.CreateDirectory(TempDir);
        }

        public IEnumerable<GameHeader> ReadExistingGameHeaders()
        {
            var filePaths = Directory.GetFiles(AllGamesRootDir, "*.VA", SearchOption.TopDirectoryOnly);
            var existingGameHeaders = filePaths.Select(path => GameHeader.FromGame(Game.FromFile(path)));

            existingGameHeaders = FilterInvalidGames(existingGameHeaders, InvalidGameStrategy.MoveToInvalid);

            GameHeaders.AddRange(existingGameHeaders);
            return existingGameHeaders;
        }

        public IEnumerable<GameHeader> ReadPreviouslyInvalidGameHeaders()
        {
            var filePaths = Directory.GetFiles(InvalidGamesDir, "*.VA");
            var existingGameHeaders = filePaths.Select(path => GameHeader.FromGame(Game.FromFile(path)));

            existingGameHeaders = FilterInvalidGames(existingGameHeaders.ToList(), InvalidGameStrategy.MoveFromInvalid);

            GameHeaders.AddRange(existingGameHeaders);
            return existingGameHeaders;
        }

        public string GenerateGameFileName(GameHeader gh) =>
            $"[{gh.DifficultyName}] {gh.CareerName} {gh.GameStart.ToString(Game.LOG_DATE_TIME_FORMAT)}.VA";

        public void FixNewGameData()
        {
            // Fix some mistakes that I make in the mod output
            var filePaths = Directory.GetFiles(TempDir, "*.VA", SearchOption.TopDirectoryOnly);
            foreach(var path in filePaths)
            {
                var text = File.ReadAllText(path);
                var fixedText = text.Replace("BounterHunter", "BountyHunter");

                File.WriteAllText(path, fixedText);
            }
        }

        public IEnumerable<GameHeader> ReadAndMoveNewGameHeaders()
        {
            var filePaths = Directory.GetFiles(TempDir, "*.VA", SearchOption.TopDirectoryOnly);
            var newGameHeaders = filePaths
                .Where(path => !(new FileInfo(path).IsLocked()))
                .Select(path => GameHeader.FromGame(Game.FromFile(path)))
                .ToList();

            newGameHeaders = FilterInvalidGames(newGameHeaders, InvalidGameStrategy.MoveToInvalid).ToList();

            // Remove empty games if user settings say to do so
            if(Settings.Current.AutoDeleteEmptyGames)
            {
                foreach (var gameHeader in newGameHeaders.Where(gh => gh.IsEmpty))
                {
                    File.Delete(gameHeader.FilePath);
                }
                newGameHeaders.RemoveAll(gh => gh.IsEmpty);
            }

            // Remove 'short' games (per user definition) if user settings say to do so
            if(Settings.Current.AutoDeleteShortGames)
            {
                double thresholdMinutes = Settings.Current.AutoDeleteShortThreshold / 60d;
                foreach (var gameHeader in newGameHeaders.Where(gh => gh.DurationMinutes < thresholdMinutes))
                {
                    File.Delete(gameHeader.FilePath);
                }
                newGameHeaders.RemoveAll(gh => gh.DurationMinutes < thresholdMinutes);
            }

            foreach (var gameHeader in newGameHeaders)
            {
                string fileName = GenerateGameFileName(gameHeader);
                string newLocation = Path.Combine(AllGamesRootDir, fileName);

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

        private const string LOG_TIME_FORMAT = "HH:mm:ss.fff";
        private const string TIMESPAN_FORMAT = @"hh\:mm\:ss\.fff";
        private static readonly int LOG_TIME_FORMAT_LENGTH = LOG_TIME_FORMAT.Length;
        private const string GAME_LOG_PREFIX = "[Lua] [MOD][Vermintide Analytics][INFO] ";
        private const string GAME_LOG_PREFIX_WITH_TIMESTAMP = "--:--:--.--- " + GAME_LOG_PREFIX;
        private static readonly int GAME_LOG_PREFIX_WITH_TIMESTAMP_LENGTH = GAME_LOG_PREFIX_WITH_TIMESTAMP.Length;
        /// <summary>
        /// Reads Vermintide 2's console logs and transfers relevant data into separate files for each game.
        /// </summary>
        /// <returns>List of errors</returns>
        public List<string> ReadAndMoveNewGameLogs()
        {
            List<string> errors = new List<string>();

            // Find out what the most recent log file we've read was the last time we came through this function
            DateTime latestReadLogDate = DateTime.MinValue;
            if(File.Exists(LatestReadLogDateFilePath))
            {
                if(long.TryParse(File.ReadAllText(LatestReadLogDateFilePath), out long ticks))
                {
                    latestReadLogDate = new DateTime(ticks);
                }
            }

            // Get all files that were created after the most recently read log was
            var filePaths = Directory.GetFiles(ConsoleLogsDir, "*.log", SearchOption.TopDirectoryOnly).Where(path => File.GetCreationTime(path) > latestReadLogDate);

            foreach(var file in filePaths)
            {
                DateTime fileCreationTime = File.GetCreationTime(file);
                TimeSpan firstTimestamp = TimeSpan.Zero;
                DateTime currentGameStart = DateTime.MinValue;

                List<string> analyticsLogLines = new List<string>();

                try
                {
                    // Keep as much unnecessary logic out of the main loop as possible
                    // Assume that the first UTC timestamp in the log file represents the same time as the file creation time
                    // So, store the first UTC timestamp, so we can compare later UTC timestamps relative to this one.
                    foreach (var line in File.ReadLines(file))
                    {
                        var timestampCharacters = line.Substring(0, LOG_TIME_FORMAT_LENGTH);
                        if (TimeSpan.TryParseExact(timestampCharacters, TIMESPAN_FORMAT, null, TimeSpanStyles.None, out firstTimestamp))
                        {
                            // Stop once we've found the first timestamp
                            break;
                        }
                    }
                }
                catch
                {
                    errors.Add($"Could not read file \"{file}\". Vermintide Analyzer may not have permission to read it.");
                    continue;
                }

                // If we didn't find any first timestamp, something has gone terribly wrong, and we ought to skip this file.
                if(firstTimestamp == TimeSpan.Zero)
                {
                    errors.Add($"Could not find timestamps in file \"{file}\". Contact ");
                    continue;
                }

                int previousTime = 0;
                int hoursToAdd = 0;
                foreach (var line in File.ReadLines(file))
                {
                    if(line.Length > 12)
                    {
                        // For every line, we still have a little bit of timestamp-related work to do
                        // Since the timestamps store hours but not days, we can't tell from a given
                        // timestamp whether it is before or after the beginning of the log. But if we 
                        string hourStr = line.Substring(0, 2);
                        string timeColon = line.Substring(2, 1);
                        string minuteStr = line.Substring(3, 2);
                        if (timeColon == ":" && int.TryParse(hourStr + minuteStr, out int logTime))
                        {
                            if (logTime < previousTime)
                            {
                                // If we sense a time rollover, everything this log and beyond is the next day, so
                                // add 24 hours onto the relative times.
                                hoursToAdd += 24;
                            }
                            previousTime = logTime;
                        }

                        // Collect relevant Analytics logs
                        if (line.Contains(GAME_LOG_PREFIX))
                        {
                            var lineNoPrefix = line.Substring(GAME_LOG_PREFIX_WITH_TIMESTAMP_LENGTH, line.Length - GAME_LOG_PREFIX_WITH_TIMESTAMP_LENGTH);
                            if (lineNoPrefix.StartsWith("$") ||
                                lineNoPrefix.StartsWith("SCHEMA VERSION") ||
                                lineNoPrefix.StartsWith("GAME VERSION") ||
                                lineNoPrefix.StartsWith("DEATHWISH") ||
                                lineNoPrefix.StartsWith("ONSLAUGHT") ||
                                lineNoPrefix.StartsWith("EMPOWERED"))
                            {

                                if (lineNoPrefix.Contains("RoundStart"))
                                {
                                    // If for some reason we encounter RoundStart before RoundEnd for a previous game,
                                    // Close out the previous game
                                    if (currentGameStart != DateTime.MinValue)
                                    {
                                        bool success = WriteOutLogs(ref currentGameStart, analyticsLogLines);
                                        if (!success)
                                        {
                                            errors.Add($"Could not write out logs for game \"{line}\"");
                                        }
                                    }

                                    // We can determine the start time of this game by comparing the timestamp of this log
                                    // with the first timestamp (keeping in mind UTC day rollovers).
                                    var timestampCharacters = line.Substring(0, LOG_TIME_FORMAT_LENGTH);
                                    if (!TimeSpan.TryParseExact(timestampCharacters, TIMESPAN_FORMAT, null, TimeSpanStyles.None, out TimeSpan gameStartTimeSpan))
                                    {
                                        // Error reading the start time, give up on this file because something is wrong
                                        errors.Add($"Could not parse timestamp of game start log \"{line}\"");
                                        continue;
                                    }
                                    currentGameStart = fileCreationTime + (gameStartTimeSpan - firstTimestamp) + TimeSpan.FromHours(hoursToAdd);
                                }

                                analyticsLogLines.Add(lineNoPrefix);

                                if (lineNoPrefix.Contains("RoundEnd"))
                                {
                                    if (currentGameStart != DateTime.MinValue)
                                    {
                                        // Split things up by game. A log containing RoundEnd will be the last log for a single game. So, split off
                                        // the collected logs and write to file
                                        bool success = WriteOutLogs(ref currentGameStart, analyticsLogLines);
                                        if (!success)
                                        {
                                            errors.Add($"Could not write out logs for game \"{line}\"");
                                        }
                                    }
                                    else
                                    {
                                        errors.Add($"Encountered Round-End log before a Round-Start \"{line}\"");
                                        analyticsLogLines.Clear();
                                    }
                                }
                            }
                        }
                    }
                }

                // If we have stored some lines and we are left with an unwritten game, write it out
                // This covers cases where Vermintide crashes mid-game, and thus we don't get a RoundEnd log.
                if(analyticsLogLines.Any() && currentGameStart != DateTime.MinValue)
                {
                    // Split things up by game. A log containing RoundEnd will be the last log for a single game. So, split off
                    // the collected logs and write to file
                    bool success = WriteOutLogs(ref currentGameStart, analyticsLogLines);
                    if (!success)
                    {
                        errors.Add($"Could not write out logs for game at end of \"{file}\"");
                    }
                }
            }

            if(filePaths.Any())
            {
                var newLatestReadLogDate = filePaths.Max(path => File.GetCreationTime(path));
                File.WriteAllText(LatestReadLogDateFilePath, newLatestReadLogDate.Ticks.ToString());
            }

            return errors;

            // Helper
            bool WriteOutLogs(ref DateTime gameStart, List<string> lines)
            {
                bool success = true;

                try
                {
                    File.WriteAllLines(Path.Combine(TempDir, $"{gameStart.ToString(Game.LOG_DATE_TIME_FORMAT)}.VA"), lines);
                }
                catch
                {
                    success = false;
                }

                lines.Clear();
                gameStart = DateTime.MinValue;
                return success;
            }
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

        public void RemoveTemporaryGameNotes(string specificFolder = null)
        {
            var path = specificFolder ?? TempDir;
            List<string> notePathsToRemove = new List<string>();
            foreach (var kvp in GameNotes)
            {
                if (kvp.Key.Contains(path))
                {
                    notePathsToRemove.Add(kvp.Key);
                }
            }
            foreach (var toRemove in notePathsToRemove)
            {
                GameNotes.Remove(toRemove);
            }

            WriteGameNotesToDisk();
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

        public bool ExportGame(GameHeader game, out string failReason)
        {
            failReason = null;

            string scrubbedPlayerName = "";
            if(Settings.Current.PlayerName != null)
            {
                scrubbedPlayerName = string.Join("_", Settings.Current.PlayerName.Split(Path.GetInvalidFileNameChars()));
            }

            var dirName = scrubbedPlayerName + " - " + game.GameStart.ToString(Game.LOG_DATE_TIME_FORMAT);
            var newTempDirPath = Path.Combine(TempDir, dirName);

            try
            {
                Directory.CreateDirectory(newTempDirPath);
            }
            catch
            {
                failReason = "Could not create temporary directory";
                return false;
            }

            FileInfo gameFile = new FileInfo(game.FilePath);
            File.Copy(gameFile.FullName, Path.Combine(newTempDirPath, gameFile.Name));

            if(!string.IsNullOrWhiteSpace(Settings.Current.PlayerName))
            {
                File.WriteAllText(Path.Combine(newTempDirPath, ImportedGameItem.PlayerNameFileName), Settings.Current.PlayerName);
            }
            if(Settings.Current.IncludeCustomNoteInExport && GameNotes.ContainsKey(game.FilePath))
            {
                File.WriteAllText(Path.Combine(newTempDirPath, ImportedGameItem.CustomNoteFileName), GameNotes[game.FilePath]);
            }
            File.WriteAllText(Path.Combine(newTempDirPath, ImportedGameItem.CareerFileName), game.CareerName);
            File.WriteAllText(Path.Combine(newTempDirPath, ImportedGameItem.MissionFileName), game.MissionName);

            var dlg = new SaveFileDialog()
            {
                DefaultExt = $"{ImportedGameItem.EXPORT_EXTENSION}",
                AddExtension = true,
                FileName = $"{scrubbedPlayerName}{(string.IsNullOrWhiteSpace(scrubbedPlayerName) ? "" : " - ")}{gameFile.NameWithoutExtension()}.{ImportedGameItem.EXPORT_EXTENSION}"
            };

            bool exported = false;
            if(dlg.ShowDialog() == true)
            {
                try
                {
                    ZipFile.CreateFromDirectory(newTempDirPath, dlg.FileName);
                    exported = true;
                }
                catch
                {
                    failReason = "Failed to export contents to specified destination";
                }
            }

            // Clean up
            try
            {
                Directory.Delete(newTempDirPath, true);
            }
            catch
            {
                if(failReason == null) failReason = "Could not delete temporary directory";
                return true;
            }

            return exported;
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

        // Enum to determine whether we ought to be moving games INTO the invalid
        // directory or OUT of the invalid directory
        private enum InvalidGameStrategy
        {
            MoveToInvalid,
            MoveFromInvalid
        }

        private IEnumerable<GameHeader> FilterInvalidGames(IEnumerable<GameHeader> games, InvalidGameStrategy strategy)
        {
            bool moveInvalid = strategy == InvalidGameStrategy.MoveToInvalid;
            bool moveValid = strategy == InvalidGameStrategy.MoveFromInvalid;

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
                    string fileName = GenerateGameFileName(gameHeader);
                    string newLocation = Path.Combine(AllGamesRootDir, fileName);

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
