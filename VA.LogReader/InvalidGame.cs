using System;

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
                        return $"Please use a newer version of the Analyzer to see this game. (Required schema version: {SchemaVersion})";
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
