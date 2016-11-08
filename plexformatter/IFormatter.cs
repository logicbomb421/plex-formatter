using System.Collections.Generic;
using System.IO;

namespace PlexFormatter
{
    interface IFormatter
    {
        string PlexRootDirectory { get; }
        string Name { get; set; }
        IEnumerable<FileInfo> Files { get; set; }

        PlexFormatterResult Validate();
        PlexFormatterResult FormatAndImport();
    }
}
