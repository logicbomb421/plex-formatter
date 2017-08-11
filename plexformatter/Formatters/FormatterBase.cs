using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace PlexFormatter.Formatters
{
    public abstract class FormatterBase : IFormatter
    {
        protected bool _deleteSourceFiles = false;

        public abstract string PlexRootDirectory { get; set; }
        public List<PlexMedia> Media { get; set; } = new List<PlexMedia>();
        public bool IsValidated { get; set; } = false;
        public bool IsFormatted { get; set; } = false;

        protected static Regex InvalidPathChars 
            = new Regex(@"([^\p{L}\s\d\-_~,;\[\]\(\).'])", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public abstract Result Validate();
        public abstract Result Format();
        public abstract Result Import();
    }
}
