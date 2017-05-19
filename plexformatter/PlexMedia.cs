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
        public string DestinationPath { get; set; }
        public FileInfo SourceFile { get; set; }
        public string Year { get; set; }

        public PlexMedia(FileInfo sourceFile, string title)
        {
            SourceFile = sourceFile;
            Title = title;
        }
    }
}
