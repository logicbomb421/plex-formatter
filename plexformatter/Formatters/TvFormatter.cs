using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PlexFormatter.Defaults;

namespace PlexFormatter.Formatters
{
    public class TvFormatter : FormatterBase
    {
        //https://regex101.com/r/QDts0q/2 made it here if modifications are needed
        private Regex _rgxSeEp = new Regex(@"(\b|(?<=[_]))(((s(|e((ason)|(ries))))|(e|ep|episode))([0-9]?\d))(\b|(?=[_]))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _plexRootDirectory = null;
        public override string PlexRootDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_plexRootDirectory))
                    _plexRootDirectory = PLEX_ROOT_TV;
                return _plexRootDirectory;
            }
            set => _plexRootDirectory = value;
        }

        public IEnumerable<PlexTvMedia> Media { get; set; }

        public string SeriesTitle { get; set; }
        public int SeriesYear { get; set; }

        public TvFormatter(string source, string seriesTitle, int seriesYear)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source), "Must supply source directory.");
            if (!Utilities.IsDirectory(source))
                throw new DirectoryNotFoundException($"Source does not appear to be a directory: {source}");

            Media = new DirectoryInfo(source)
                .GetFiles()
                .Where(f => new[] { ".mkv",".m4v",".mp4" }.Contains(f.Extension))
                .Select(f => new PlexTvMedia(f)); //TODO globalize + clean this up
            if (!Media.Any())
                throw new FileNotFoundException($"No files found in source directory: {source}");

            SeriesTitle = seriesTitle;
            SeriesYear = seriesYear;
        }

        public override Result Validate()
        {
            var r = new Result(ResultStatus.Failed);

            if (string.IsNullOrEmpty(SeriesTitle))
                r.Log.Add("Could not find series title");

            //TODO parse/regex series year

            foreach (var m in Media)
            {
                m.Title = SeriesTitle;
                //TODO add year once parsed

                var matches = _rgxSeEp.Matches(m.SourceFile.Name);
                if (matches.Count != 2)
                {
                    r.Log.Add($"Incorrect number of matches found for file {m.SourceFile.Name}. Expecting season and episode tokens. Found: {string.Join(", ", matches)}");
                    continue;
                }
                var tmpr = parseSeasonAndEpisode(m, matches);
                if (tmpr.Status != ResultStatus.Success)
                {
                    r.Log.AddRange(tmpr.Log);
                    continue; //in case of later validation
                }
            }

            if (r.Log.Count == 0)
                r.Status = ResultStatus.Success;
            return r;
        }

        private Result parseSeasonAndEpisode(PlexTvMedia media, MatchCollection matches)
        {
            var r = new Result(ResultStatus.Success);
            var seasonTokens = new[] { "s", "season", "series" };
            var episodeTokens = new[] { "e", "ep", "episode" };
            foreach (Match m in matches)
            {
                var lval = m.Value.ToLower();
                string token;
                if (!string.IsNullOrEmpty(token = seasonTokens.FirstOrDefault(st => lval.IndexOf(st, StringComparison.OrdinalIgnoreCase) != -1)))
                {
                    lval = lval.Replace(token, string.Empty);
                    if (int.TryParse(lval, out int i))
                        media.Season = i;
                    else
                        r.Log.Add($"Could not convert parsed season value {lval} into a number.");
                }
                else if (!string.IsNullOrEmpty(token = episodeTokens.FirstOrDefault(et => lval.IndexOf(et, StringComparison.OrdinalIgnoreCase) != -1)))
                {
                    lval = lval.Replace(token, string.Empty);
                    if (int.TryParse(lval, out int i))
                        media.Episode = i;
                    else
                        r.Log.Add($"Could not convert parsed episode value {lval} into a number.");
                }
                else
                {
                    r.Log.Add($"Unable to find season or episode token in match: {lval}");
                }
            }

            if (r.Log.Count > 0)
                r.Status = ResultStatus.Failed;
            return r;
        }

        public override Result Format()
        {
            throw new NotImplementedException();
        }

        public override Result Import()
        {
            throw new NotImplementedException();
        }
    }
}
