﻿using System.Collections.Generic;

namespace PlexFormatter.Formatters
{
    public interface IFormatter
    {
        string PlexRootDirectory { get; set; }
        List<PlexMedia> Media { get; set; }
        bool IsValidated { get; set; }
        bool IsFormatted { get; set; }
        Result Validate();
        Result Format();
        Result Import();
    }
}
