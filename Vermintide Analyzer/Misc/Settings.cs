using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        #endregion

        #region Constructor (set default values here)
        public Settings()
        {
            PlayerName = null;
            WatermarkScreenshots = true;
            ShowHealthWhenDowned = false;
            ConfirmDeleteGames = true;
            IncludeCustomNoteInExport = true;
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
                Current = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath));
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
