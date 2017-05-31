using System.Collections.Generic;

namespace PlexFormatter
{
    public interface IFormatter
    {
        string PlexRootDirectory { get; set; }
        List<PlexMedia> Media { get; set; }
        bool IsValidated { get; set; }
        PlexFormatterResult Validate();
        PlexFormatterResult Format();
        PlexFormatterResult Import();
    }
}
