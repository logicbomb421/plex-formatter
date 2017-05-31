using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace PlexFormatter
{
    public abstract class FormatterBase : IFormatter
    {
        protected bool _deleteSourceFiles = false;
        protected BackgroundWorker _worker = null;

        public abstract string PlexRootDirectory { get; set; }
        //public string Title { get; set; }
        public List<PlexMedia> Media { get; set; } = new List<PlexMedia>();
        public bool IsValidated { get; set; } = false;

        protected static Regex InvalidPathChars = new Regex(@"([^\p{L}\s\d\-_~,;\[\]\(\).'])", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //private IEnumerable<FileInfo> _files = null;
        //public IEnumerable<FileInfo> Files
        //{
        //    get
        //    {
        //        if (_files == null)
        //            return new FileInfo[0];
        //        return _files;
        //    }
        //    set
        //    {
        //        _files = value;
        //    }
        //}

        public abstract PlexFormatterResult Validate();
        public abstract PlexFormatterResult Format();
        public abstract PlexFormatterResult Import();
    }
}
