using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PlexFormatter.Defaults;

namespace PlexFormatter
{
    public class MovieFormatter : FormatterBase
    {
        private Regex rgx_yearKey
        {
            get
            {
                if (_rgx_yearKey == null)
                {
                    var now = DateTime.Now.Year.ToString();
                    var x = now[now.Length - 2];
                    _rgx_yearKey = new Regex($@"\b(19\d\d|20[0-{x}][0-9])\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                return _rgx_yearKey;
            }
        }
        private Regex _rgx_yearKey = null;

        private string _plexRootDirectory = null;
        public override string PlexRootDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_plexRootDirectory))
                    _plexRootDirectory = PLEX_ROOT_MOVIE;
                return _plexRootDirectory;
            }
            set { _plexRootDirectory = value; }
        }

        public string Year { get; set; } = null;

        public MovieFormatter(string source, string movieTitle, string year = null, string plexRootDirectory = null)
        {
            if (!Directory.Exists(source) && !File.Exists(source))
                throw new DirectoryNotFoundException($"Could not find source directory: {source}"); //TODO custom exception

            _plexRootDirectory = PlexRootDirectory;
            Year = year;

            //directory or file
            if ((File.GetAttributes(source) & FileAttributes.Directory) == FileAttributes.Directory)
            {
                new DirectoryInfo(source)
                .EnumerateFiles()
                .Where(f => VideoExtensions.Contains(f.Extension.Replace(".", "")))
                .ToList()
                .ForEach(f => Media.Add(new PlexMedia(f, movieTitle)));
            }
            else
            {
                Media.Add(new PlexMedia(new FileInfo(source), movieTitle));
            }
        }

        public override PlexFormatterResult Validate()
        {
            var log = new List<string>();
            foreach (var movie in Media)
            {
                if (!string.IsNullOrEmpty(Year))
                {
                    movie.Year = Year;
                }
                else
                {
                    //TODO return something more informative in case the implementer wants to prevent choices of the correct year to the user.
                    var matches = rgx_yearKey.Matches(movie.SourceFile.Name);
                    if (matches.Count == 0)
                        log.Add($"'{movie.SourceFile.Name}' missing year identifier.");
                    else if (matches.Count > 1)
                        log.Add($"Found multiple year identifiers in '{movie.SourceFile.Name}'.");
                    else
                        movie.Year = matches[0].Value; 
                }
            }

            var result = new PlexFormatterResult();
            if (log.Count > 0)
            {
                result.Status = PlexFormatterResult.ResultStatus.Failed;
                result.Log.AddRange(log);
            }

            IsValidated = true;
            return result;
        }

        public override PlexFormatterResult Format()
        {
            if (!IsValidated)
            {
                var vr = Validate();
                if (vr.Status != PlexFormatterResult.ResultStatus.Success)
                    return vr;
            }

            var result = new PlexFormatterResult();
            foreach (var movie in Media)
            {
                if (string.IsNullOrEmpty(movie.DestinationPath))
                {
                    var removed_chars = new List<char>();
                    var title = $"{movie.Title} ({movie.Year})";
                    var folderName = title;
                    if (InvalidPathChars.IsMatch(folderName))
                    {
                        var matches = InvalidPathChars.Matches(folderName);
                        for (int i = 0; i < matches.Count; i++)
                            removed_chars.Add(matches[i].Value[0]);
                        folderName = InvalidPathChars.Replace(folderName, string.Empty);
                    }
                    var fileName = title + movie.SourceFile.Extension;
                    if (InvalidPathChars.IsMatch(fileName))
                    {
                        var matches = InvalidPathChars.Matches(fileName);
                        for (int i = 0; i < matches.Count; i++)
                            removed_chars.Add(matches[i].Value[0]);
                        fileName = InvalidPathChars.Replace(fileName, string.Empty);
                    }
                    if (removed_chars.Count > 0)
                    {
                        result.Log.Add($"Removed invalid chars {string.Join(" ", removed_chars.Distinct())} from {title}");
                    }
                    var dbg = Path.Combine(PlexRootDirectory, folderName, fileName);
                    movie.DestinationPath = Path.Combine(PlexRootDirectory, folderName, fileName);
                }
            }
            return result.Finalize(PlexFormatterResult.ResultStatus.Success); 
        }

        public override PlexFormatterResult Import()
        {
            var result = new PlexFormatterResult();
            foreach (var movie in Media)
            {
                try
                {
                    Directory.CreateDirectory(movie.DestinationPath.Substring(0, movie.DestinationPath.LastIndexOf('\\')));
                    File.Copy(movie.SourceFile.FullName, movie.DestinationPath);
                    //movie.SourceFile.CopyTo(movie.DestinationPath);
                }
                catch (Exception ex)
                {
                    return result.Finalize(PlexFormatterResult.ResultStatus.Failed, $"Unable to copy file. The error was: {ex.Message}");
                }
            }
            return result.Finalize(PlexFormatterResult.ResultStatus.Success);
        }
    }
}
