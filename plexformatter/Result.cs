using System.Collections.Generic;
using System.IO;

namespace PlexFormatter
{
    public class PlexFormatterResult
    {
        public bool IsValid { get; set; } = false;
        public List<string> Log { get; set; } = new List<string>();
        public FileInfo[] InvalidFiles { get; set; } = { };

        public PlexFormatterResult(bool success)
        {
            IsValid = success;
        }

        public PlexFormatterResult(bool success, params string[] addToLog)
        {
            IsValid = success;
            Log.AddRange(addToLog);
        }

        public PlexFormatterResult(bool success, FileInfo[] invalidFiles, params string[] addToLog)
        {
            IsValid = success;
            InvalidFiles = invalidFiles;
            Log.AddRange(addToLog);
        }
    }
}
