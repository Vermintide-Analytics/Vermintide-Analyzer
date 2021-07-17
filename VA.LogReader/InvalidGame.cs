using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class InvalidGame
    {
        public string FilePath { get; set; }
        public ParseError Error { get; set; }

        public byte SchemaVersionMajor { get; set; }
        public byte SchemaVersionMinor { get; set; }

        public DateTime GameStart { get; set; }

        public InvalidGame(GameHeader gh)
        {
            FilePath = gh.FilePath;
            Error = gh.Error;
            SchemaVersionMajor = gh.SchemaVersionMajor;
            SchemaVersionMinor = gh.SchemaVersionMinor;
            GameStart = gh.GameStart;
        }
    }
}
