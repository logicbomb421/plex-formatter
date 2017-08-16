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
    public class TvFormatter : FormatterBase<PlexTvMedia>
    {
        private static Regex _rgxSeason = new Regex(@"(?<=\b|\w)(s(|e((ason)|(ries)))([0-9]?\d))(?=\b|\w)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _rgxEpisode = new Regex(@"(?<=\b|\w)((e|ep|episode)([0-9]?\d))(?=\b|\w)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly string[] _seasonTokens = { "s", "season", "series" }; //TODO 'se'?
        private static readonly string[] _episodeTokens = { "e", "ep", "episode" };

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
                .Where(f => new[] { ".mkv",".m4v",".mp4" }.Contains(f.Extension)) //TODO globalize + clean this up
                .Select(f => new PlexTvMedia(f))
                .ToList();
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

                //TODO maybe validate against thetvdb.com (what plex uses)

                var rgx_season = _rgxSeason.Matches(m.SourceFile.Name);
                if (rgx_season.Count != 1)
                {
                    r.Log.Add($"Incorrect number of matches found for file {m.SourceFile.Name}. Expecting a single season token. Found {rgx_season.Count}: {string.Join(", ", rgx_season)}");
                }
                else
                {
                    var tr_s = parseSeason(rgx_season[0]);
                    if (tr_s.Status == ResultStatus.Success)
                        m.Season = tr_s.Data;
                    else
                    {
                        r.Log.Add($"Unable to determine season for file {m.SourceFile.Name}");
                        r.Log.AddRange(tr_s.Log);
                    }
                }

                var rgx_episode = _rgxEpisode.Matches(m.SourceFile.Name);
                if (rgx_episode.Count != 1)
                {
                    r.Log.Add($"Incorrect number of matches found for file {m.SourceFile.Name}. Expecting a single episode token. Found {rgx_episode.Count}: {string.Join(", ", rgx_episode)}");
                }
                else
                {
                    var tr_e = parseEpisode(rgx_episode[0]);
                    if (tr_e.Status == ResultStatus.Success)
                        m.Episode = tr_e.Data;
                    else
                    {
                        r.Log.Add($"Unable to determine episode for file {m.SourceFile.Name}");
                        r.Log.AddRange(tr_e.Log);
                    }
                }
            }

            if (r.Log.Count == 0)
                r.Status = ResultStatus.Success;
            return r;
        }
        private Result<int> parseSeason(Match rgxMatch)
        {
            var result = new Result<int>(ResultStatus.Success);
            var match = rgxMatch.Value.ToLower();
            string token;
            if (string.IsNullOrEmpty(token = _seasonTokens.FirstOrDefault(st => match.IndexOf(st, StringComparison.OrdinalIgnoreCase) != -1)))
                result.Log.Add($"Unable to find season token in match: {match}");
            else
            {
                match = match.Replace(token, string.Empty);
                if (int.TryParse(match, out int i))
                    result.Data = i;
                else
                    result.Log.Add($"Could not convert parsed season value {match} into a number.");
            }

            if (result.Log.Count > 0)
                result.Status = ResultStatus.Failed;
            return result;
        }

        private Result<int> parseEpisode(Match rgxMatch)
        {
            var result = new Result<int>(ResultStatus.Success);
            var match = rgxMatch.Value.ToLower();
            string token;
            if (string.IsNullOrEmpty(token = _episodeTokens.FirstOrDefault(et => match.IndexOf(et, StringComparison.OrdinalIgnoreCase) != -1)))
                result.Log.Add($"Unable to find episode token in match: {match}");
            else
            {
                match = match.Replace(token, string.Empty);
                if (int.TryParse(match, out int i))
                    result.Data = i;
                else
                    result.Log.Add($"Could not convert parsed episode value {match} into a number.");
            }

            if (result.Log.Count > 0)
                result.Status = ResultStatus.Failed;
            return result;
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
