using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexFormatter
{
    public class PlexMedia
    {
        public string Title { get; set; }
        public FileInfo File { get; set; }
        public string RegexMatch { get; set; }

        public PlexMedia(FileInfo file)
            => File = file;
    }
}
