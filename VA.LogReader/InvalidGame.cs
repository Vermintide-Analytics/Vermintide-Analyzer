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

        public string SchemaVersion => $"{SchemaVersionMajor}.{SchemaVersionMinor}";

        public DateTime GameStart { get; set; }

        public string GameStartDate => GameStart.ToShortDateString();
        public string GameStartTime => GameStart.ToShortTimeString();

        public string RecommendedAction
        {
            get
            {
                switch (Error)
                {
                    case ParseError.SchemaMismatch:
                        string val;
                        if (SchemaVersionMajor < Schema.SCHEMA_VERSION_MAJOR)
                        {
                            val = "an older";
                        }
                        else if(SchemaVersionMajor > Schema.SCHEMA_VERSION_MAJOR)
                        {
                            val = "a newer";
                        }
                        else
                        {
                            if(SchemaVersionMinor < Schema.SCHEMA_VERSION_MINOR)
                            {
                                val = "an older";
                            }
                            else
                            {
                                val = "a newer";
                            }
                        }
                        return $"Please use {val} version of the Analyzer to see this game. (Required schema version: {SchemaVersion})";
                    case ParseError.BadHeader:
                    case ParseError.NoStartEvent:
                    default:
                        return "Contact developer";
                }
            }
        }

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
