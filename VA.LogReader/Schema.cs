using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public static class Schema
    {

        public static byte SCHEMA_VERSION_MAJOR => (byte)typeof(Schema).Assembly.GetName().Version.Major;
        public static byte SCHEMA_VERSION_MINOR = (byte)typeof(Schema).Assembly.GetName().Version.Minor;
        public static readonly string SCHEMA_VERSION = SCHEMA_VERSION_MAJOR + "." + SCHEMA_VERSION_MINOR;
    }
}
