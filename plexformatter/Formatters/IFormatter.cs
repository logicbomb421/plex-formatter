using System.Collections.Generic;

namespace PlexFormatter.Formatters
{
    public interface IFormatter<T>
    {
        string PlexRootDirectory { get; set; }
        List<T> Media { get; set; }
        bool IsValidated { get; set; }
        bool IsFormatted { get; set; }
        Result Validate();
        Result Format();
        Result Import();
    }
}
