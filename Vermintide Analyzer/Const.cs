using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vermintide_Analyzer
{
    public class Const
    {
        public const string NEW_LOG_DIR = "%appdata%\\Fatshark\\Vermintide 2";
        public const string APP_DATA_DIR = "%appdata%\\Vermintide Analyzer";
        public const string GAME_DIR = "Games";
        public const string DATA_DIR = "Data";

        public const char SILENT_SPACE = '\u200B'; // \u200B is a space that doesn't take up any space. This is useful for helping text wrapping
    }
}
