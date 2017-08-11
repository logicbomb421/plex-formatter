using System.IO;

namespace PlexFormatter
{
    public class PlexMedia
    {
        public string Title { get; set; }
        public string DestinationPath { get; set; }
        public FileInfo SourceFile { get; set; }
        public int Year { get; set; } = -1;

        public PlexMedia(FileInfo sourceFile)
            => SourceFile = sourceFile;

        public PlexMedia(FileInfo sourceFile, string title)
        {
            SourceFile = sourceFile;
            Title = title;
        }
    }

    //TODO convert movie to use this derived class structure? it doesnt need any special sauce, but might be nice for clarity
    public class PlexTvMedia : PlexMedia
    {
        public int Season { get; set; }
        public int Episode { get; set; }
        public PlexTvMedia(FileInfo sourceFile)
            : base(sourceFile) { }
        public PlexTvMedia(FileInfo sourceFile, string title)
            : base(sourceFile, title) { }
    }
}
