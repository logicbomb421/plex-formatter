using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PlexFormatter.Defaults;

namespace PlexFormatter.Formatters
{
    public class MovieFormatter : FormatterBase
    {
        private static Regex _rgx_yearKey = null;
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

        public int? Year { get; set; } = null;

        public PlexMedia Movie
        {
            get => Media.FirstOrDefault();
            set
            {
                Media.Clear();
                Media.Add(value);
            }
        }

        public MovieFormatter(BackgroundWorker worker, string source, string movieTitle, bool deleteSourceFiles, string plexRootDirectory, int? year = null)
            : this(source, movieTitle, deleteSourceFiles, plexRootDirectory, year)
        {
            _worker = worker;
        }

        public MovieFormatter(string source, string movieTitle, bool deleteSourceFiles, string plexRootDirectory, int? year = null)
        {
            if (!File.Exists(source))
                throw new FileNotFoundException($"Could not find source file: {source}"); //TODO custom exception

            _deleteSourceFiles = deleteSourceFiles;
            _plexRootDirectory = PlexRootDirectory;
            Year = year;

            Media.Add(new PlexMedia(new FileInfo(source), movieTitle));
        }

        public override PlexFormatterResult Validate()
        {
            var log = new List<string>();

            if (string.IsNullOrEmpty(Movie.Title))
            {
                log.Add("Could not find movie title.");
            }

            //TODO currently no validation if user supplies year
            if (Year.HasValue)
            {
                Movie.Year = Year.Value;
            }
            else
            {
                _worker?.ReportProgress(0, "No year provided, searching filename...");
                //TODO return something more informative in case the implementer wants to present choices of the correct year to the user.
                var matches = rgx_yearKey.Matches(Movie.SourceFile.Name);
                if (matches.Count == 0)
                    log.Add($"'{Movie.SourceFile.Name}' missing year identifier.");
                else if (matches.Count > 1)
                    log.Add($"Found multiple year identifiers in '{Movie.SourceFile.Name}'.");
                else
                {
                    if (int.TryParse(matches[0].Value, out int i))
                    {
                        Movie.Year = i;
                        Year = i;
                    }
                    _worker?.ReportProgress(0, $"Found '{Movie.Year}'");
                }
            }

            var result = new PlexFormatterResult();
            if (log.Count > 0)
            {
                result.Status = PlexFormatterResult.ResultStatus.Failed;
                result.Log.AddRange(log);
                return result;
            }

            IsValidated = true;
            return result.Finalize(PlexFormatterResult.ResultStatus.Success);
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
            if (string.IsNullOrEmpty(Movie.DestinationPath))
            {
                var removed_chars = new List<char>();
                var title = $"{Movie.Title} ({Movie.Year})";
                var folderName = title;
                if (InvalidPathChars.IsMatch(folderName))
                {
                    var matches = InvalidPathChars.Matches(folderName);
                    for (int i = 0; i < matches.Count; i++)
                        removed_chars.Add(matches[i].Value[0]);
                    folderName = InvalidPathChars.Replace(folderName, string.Empty);
                }
                var fileName = title + Movie.SourceFile.Extension;
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
                Movie.DestinationPath = Path.Combine(PlexRootDirectory, folderName, fileName);
            }
            return result.Finalize(PlexFormatterResult.ResultStatus.Success);
        }

        public override PlexFormatterResult Import()
        {
            if (!IsFormatted)
            {
                var fr = Format();
                if (fr.Status != PlexFormatterResult.ResultStatus.Success)
                    return fr;
            }

            var result = new PlexFormatterResult();
            try
            {
                _worker?.ReportProgress(0, $"Creating directory for {Movie.Title}");
                Directory.CreateDirectory(Movie.DestinationPath.Substring(0, Movie.DestinationPath.LastIndexOf('\\')));
            }
            catch (Exception ex)
            {
                return result.Finalize(PlexFormatterResult.ResultStatus.Failed, $"Unable to create drirectory for file(s). The error was: {ex.Message}");
            }

            try
            {
                _worker?.ReportProgress(0, $"Copying source file for {Movie.Title}");
                var copier = new ProgressReportingFileCopier(Movie.SourceFile.FullName, Movie.DestinationPath);
                copier.OnUpdate += (i) => _worker?.ReportProgress(0, new CopyUpdate(i, i == 0));
                copier.OnComplete += () => _worker?.ReportProgress(0, "Complete!");
                copier.Copy();
            }
            catch (Exception ex)
            {
                return result.Finalize(PlexFormatterResult.ResultStatus.Failed, $"Unable to copy file. The error was: {ex.Message}");
            }

            if (_deleteSourceFiles)
            {
                try
                {
                    _worker?.ReportProgress(0, $"Deleting source file for {Movie.Title}");
                    Movie.SourceFile.Delete();
                }
                catch (Exception ex)
                {
                    _worker?.ReportProgress(0, $"Unable to delete source file. The error was: {ex.Message}");
                }
            }
            return result.Finalize(PlexFormatterResult.ResultStatus.Success);
        }
    }
}

public struct CopyUpdate
{
    public int PercentComplete;
    public bool IsFirstUpdate;
    public CopyUpdate(int percent, bool isFirstUpdate)
    {
        PercentComplete = percent;
        IsFirstUpdate = isFirstUpdate;
    }
}