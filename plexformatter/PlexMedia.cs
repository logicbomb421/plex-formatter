using System.IO;

namespace PlexFormatter
{
    //interface is currently to support 'where T' clause in FormatterBase<T>
    public interface IPlexMedia
    {
        string Title { get; set; }
        string DestinationPath { get; set; }
        FileInfo SourceFile { get; set; }
        int Year { get; set; }
    }

    public class PlexMedia : IPlexMedia
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
