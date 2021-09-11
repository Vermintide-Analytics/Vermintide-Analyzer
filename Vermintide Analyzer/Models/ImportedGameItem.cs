using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VA.LogReader;
using Vermintide_Analyzer.Statistics;

namespace Vermintide_Analyzer.Models
{
    public class ImportedGameItem
    {
        public const string EXPORT_EXTENSION = "VD";

        public const string MissionFileName = "Mission.txt";
        public const string CareerFileName = "Career.txt";
        public const string PlayerNameFileName = "PlayerName.txt";
        public const string CustomNoteFileName = "CustomNote.txt";

        public const string UnknownPlayerName = "[Unknown]";

        public string PathToFolder { get; private set; }

        public Game LoadedGame { get; set; }
        public GameStats Stats { get; set; }

        public string PlayerName { get; set; }
        public string CustomNote { get; set; }

        public MISSION Mission { get; set; }
        public string MissionName => Mission.ToString().Replace("_", " ");
        public CAREER Career { get; set; }
        public string CareerName => Career.ForDisplay();

        private ImportedGameItem(string pathToFolder)
        {
            PathToFolder = pathToFolder;

            // Mission is required
            Mission = ReadMetadataFileText(MissionFileName).Replace(" ", "_").FromDisplay<MISSION>();
            // Career is required
            Career = ReadMetadataFileText(CareerFileName).FromDisplay<CAREER>();

            // Player name is optional
            PlayerName = ReadMetadataFileText(PlayerNameFileName, UnknownPlayerName);
            // Custom note is optional
            CustomNote = ReadMetadataFileText(CustomNoteFileName, "");
        }

        public static ImportedGameItem ImportFromZip(string pathToZip, out string failReason)
        {
            failReason = null;

            string newDirectoryPath = Path.Combine(GameRepository.TempDir, $"Import_{DateTime.Now.Ticks}");
            try
            {
                Directory.CreateDirectory(newDirectoryPath);
                ZipFile.ExtractToDirectory(pathToZip, newDirectoryPath);
            }
            catch
            {
                failReason = $"Could not access {EXPORT_EXTENSION} contents";
                return null;
            }

            try
            {
                var imported = new ImportedGameItem(newDirectoryPath);
                return imported;
            }
            catch
            {
                failReason = $"Could not parse {EXPORT_EXTENSION} contents";
                return null;
            }
        }

        public bool LoadGame(out string failReason)
        {
            failReason = null;

            var gameFiles = Directory.GetFiles(PathToFolder, $"*.VA");
            if(gameFiles.Length == 0)
            {
                failReason = "No game data in imported file";
                return false;
            }
            if(gameFiles.Length > 1)
            {
                failReason = "Multiple game data in imported file";
                return false;
            }

            LoadedGame = Game.FromFile(gameFiles.First());
            Stats = new GameStats(LoadedGame);
            Stats.RecalculateStats();

            if(!string.IsNullOrWhiteSpace(CustomNote))
            {
                if(GameRepository.Instance.GameNotes.ContainsKey(gameFiles.First()))
                {
                    GameRepository.Instance.GameNotes[gameFiles.First()] = CustomNote;
                }
                else
                {
                    GameRepository.Instance.GameNotes.Add(gameFiles.First(), CustomNote);
                }
            }

            return true;
        }

        public void Cleanup()
        {
            if(PathToFolder.Contains(GameRepository.TempDir))
            {
                GameRepository.Instance.RemoveTemporaryGameNotes(PathToFolder);
                Directory.Delete(PathToFolder, true);
            }
        }

        private string ReadMetadataFileText(string fileName) =>
            File.ReadAllText(Path.Combine(PathToFolder, fileName));

        private string ReadMetadataFileText(string fileName, string defaultValue)
        {
            var fullPath = Path.Combine(PathToFolder, fileName);
            if (File.Exists(fullPath))
            {
                return File.ReadAllText(fullPath);
            }

            return defaultValue;
        }
    }
}
