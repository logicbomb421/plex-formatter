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
                    var yearPlusThree = (DateTime.Now.Year + 3).ToString();
                    var x = yearPlusThree[yearPlusThree.Length - 2];
                    var y = yearPlusThree[yearPlusThree.Length - 1];
                    _rgx_yearKey = new Regex($@"(19[0-9]\d|20[0-{x}][0-{y}])");
                }
                return _rgx_yearKey;
            }
        }
        private Regex _rgx_yearKey = null;
        private string _rgx_match = null;

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

        public MovieFormatter(string sourceDirectory, string title = null, string plexRootDirectory = null)
        {
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException($"Could not find source directory: {sourceDirectory}");

            _plexRootDirectory = plexRootDirectory;
            Title = title;
            new DirectoryInfo(sourceDirectory)
                .EnumerateFiles()
                .ToList()
                .ForEach(f => Media.Add(new PlexMedia(f)));
        }

        public override PlexFormatterResult Validate()
        {
            var log = new List<string>();
            foreach (var media in Media)
            {
                //TODO return something more informative in case the implementer wants to prevent choices of the correct year to the user.
                //TODO allow multi year override if user provides correct year.
                var matches = rgx_yearKey.Matches(media.File.Name);
                if (matches.Count == 0)
                    log.Add($"'{media.File.Name}' missing year identifier.");
                else if (matches.Count > 1)
                    log.Add($"Found multiple year identifiers in '{media.File.Name}'.");
                else
                    media.RegexMatch = matches[0].Value;
            }

            var result = new PlexFormatterResult(true);
            if (log.Count > 0)
            {
                result.IsValid = false;
                result.Log.AddRange(log);
            }

            return result;
        }

        public override PlexFormatterResult FormatAndImport()
        {
            var valid = Validate();
            if (!valid.IsValid)
                return valid;

            string fullBaseDirectory = $@"{PlexRootDirectory}\{Title} ({_rgx_match})\";
            if (Directory.Exists(fullBaseDirectory))
                return new PlexFormatterResult(false, $"Directory {fullBaseDirectory} already exists. Cannot create or copy files.");

            try
            {
                Directory.CreateDirectory(fullBaseDirectory);
            }
            catch (Exception e)
            {
                return new PlexFormatterResult(false, $"Could not create directory: {fullBaseDirectory}.", e.Message);
            }

            var tempLog = new List<string>();
            foreach (var file in Files)
            {
                var newName = $@"{fullBaseDirectory}\{Title} ({_rgx_match}){file.Extension}";
                try
                {
                    file.CopyTo(newName);
                }
                catch (Exception e)
                {
                    tempLog.Add($"Could not move file: {newName}. The error was: {e.Message}");
                }
            }

            if (tempLog.Count > 0)
                return new PlexFormatterResult(false, tempLog.ToArray());

            return new PlexFormatterResult(true);
        }

        public override PlexFormatterResult Format()
        {
            var result = Validate();
            if (!result.IsValid)
                return result;

            //TODO class var
            string fullBaseDirectory = $@"{PlexRootDirectory}\{Title} ({_rgx_match})\";
            if (Directory.Exists(fullBaseDirectory))
                return result.Finalzie(false, $"Directory {fullBaseDirectory} already exists. Cannot create or copy files.");

            Media.ForEach(m => { if (string.IsNullOrEmpty(m.Title)) m.Title = $@"{fullBaseDirectory}\{m.Title} ({m.RegexMatch}){m.File.Extension}" });

            return result;
        }

        public override PlexFormatterResult Import()
        {
            throw new NotImplementedException();
        }
    }
}
