//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using static PlexFormatter.Defaults;

//namespace PlexFormatter
//{
//    public class TvFormatter //: FormatterBase
//    {
//        private Regex _rgx_seasonKey = new Regex(@"(([Ss])([0-9]))\w");
//        private Regex _rgx_episodeKey = new Regex(@"(([Ee])([0-9]))\w");
//        private int _season;

//        private string _plexRootDirectory = null;
//        public override string PlexRootDirectory
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(_plexRootDirectory))
//                    _plexRootDirectory = PLEX_ROOT_TV;
//                return _plexRootDirectory;
//            }
//            set { _plexRootDirectory = value; }
//        }

//        public TvFormatter(string name, int season, string sourceDirectory, string plexRootDirectory)
//        {
//            if (!Directory.Exists(sourceDirectory))
//                throw new DirectoryNotFoundException($"Could not find source directory: {sourceDirectory}");

//            _plexRootDirectory = plexRootDirectory;
//            Files = new DirectoryInfo(sourceDirectory).EnumerateFiles();
//            Title = name;
//            _season = season;
//        }

//        public override PlexFormatterResult Validate()
//        {
//            var result = new PlexFormatterResult(true);
//            foreach (var file in Files)
//            {
//                //TODO only one match, multiple could indicate we picked up something irrelevant. There are still other ways to mess this up that need to be sorted too.
//                if (!_rgx_seasonKey.IsMatch(file.Name))
//                    result.Log.Add($"'{file.Name}' missing season identifier.");

//                if (!_rgx_episodeKey.IsMatch(file.Name))
//                    result.Log.Add($"'{file.Name}' missing episode identifier.");
//            }

//            if (result.Log.Count > 0)
//                result.IsValid = false;

//            return result;
//        }

//        public override PlexFormatterResult FormatAndImport()
//        {
//            var valid = Validate();
//            if (!valid.IsValid)
//                return valid;

//            string fullBaseDirectory = $@"{PlexRootDirectory}\{Title}\Season {_season}\";
//            if (Directory.Exists(fullBaseDirectory))
//                return new PlexFormatterResult(false, $"Directory {fullBaseDirectory} already exists. Cannot create or copy files.");

//            try
//            {
//                Directory.CreateDirectory(fullBaseDirectory);
//            }
//            catch (Exception e)
//            {
//                return new PlexFormatterResult(false, $"Could not create directory: {fullBaseDirectory}.", e.Message);
//            }

//            var tempLog = new List<string>();
//            foreach (var file in Files)
//            {
//                var season = _rgx_seasonKey.Match(file.Name).Value.ToUpper();
//                var episode = _rgx_episodeKey.Match(file.Name).Value.ToUpper();
//                var newName = $@"{fullBaseDirectory}\{Title} - {season}{episode}{file.Extension}";
//                try
//                {
//                    file.CopyTo(newName);
//                }
//                catch (Exception e)
//                {
//                    tempLog.Add($"Could not move file: {newName}. The error was: {e.Message}");
//                }
//            }

//            if (tempLog.Count > 0)
//                return new PlexFormatterResult(false, tempLog.ToArray());

//            return new PlexFormatterResult(true);
//        }
//    }
//}
