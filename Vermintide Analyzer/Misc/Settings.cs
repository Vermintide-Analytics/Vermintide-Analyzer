using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Vermintide_Analyzer
{
    [Serializable]
    public class Settings
    {
        #region Singleton
        public static Settings Current { get; set; }
        #endregion

        #region Misc
        public static readonly string FilePath = Path.Combine(Environment.ExpandEnvironmentVariables(Const.APP_DATA_DIR), Const.DATA_DIR, "Settings.json");
        #endregion

        #region Properties
        public string PlayerName { get; set; }
        public bool WatermarkScreenshots { get; set; }
        public bool ShowHealthWhenDowned { get; set; }
        public bool ConfirmDeleteGames { get; set; }
        public bool IncludeCustomNoteInExport { get; set; }
        public bool AutoDeleteEmptyGames { get; set; }
        public bool AutoDeleteShortGames { get; set; }
        public uint AutoDeleteShortThreshold { get; set; }
        #endregion

        #region Constructor (set default values here)
        public Settings()
        {
            PlayerName = null;
            WatermarkScreenshots = true;
            ShowHealthWhenDowned = false;
            ConfirmDeleteGames = true;
            IncludeCustomNoteInExport = true;
            AutoDeleteEmptyGames = true;
            AutoDeleteShortGames = false;
            AutoDeleteShortThreshold = 3 * 60; // 3 minutes
        }
        #endregion

        #region Methods
        public static bool Save()
        {
            if (Current is null) return false;

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Current, Formatting.Indented));
            return true;
        }

        public static void Load()
        {
            if(File.Exists(FilePath))
            {
                JsonSerializerSettings deserializeSettings = new JsonSerializerSettings();
                deserializeSettings.Error = (sender, errArgs) =>
                {
                    Util.SafeInvoke(() =>
                    {
                        bool ok = Util.OkCancelErrorDialog($"Failed to load user settings.\n\n{errArgs.ErrorContext.Error.Message}\n\nVermintide Analyzer must close. You can try manually editing settings and restarting the application, or just contact the developer.\n\nPress 'OK' to open settings file, or 'Cancel' otherwise.");
                        if(ok)
                        {
                            // Open JSON file in default editor
                            Process.Start(Settings.FilePath);
                        }
                        Application.Current.Shutdown();
                    });
                };

                Current = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath), deserializeSettings);
            }
            else
            {
                Current = new Settings();
                Save();
            }
        }
        #endregion
    }
}
