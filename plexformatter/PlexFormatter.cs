using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlexFormatter
{
    public class PlexFormatter
    {
        static ICollection<string> Plexify(MediaTypes type, string name)
        {
            var regex_sn = new Regex(@"(([Ss])([0-9]))\w");
            var regex_ep = new Regex(@"(([Ee])([0-9]))\w");

            var ret = new List<string>();
            Func<string, ICollection<string>> finalize = (string log) =>
            {
                ret.Add(log);
                return ret;
            };

            var srcdir = @"C:\Temp\MrRobotS1\";
            if (!Directory.Exists(srcdir))
                return finalize($"Source directory: {srcdir} does not exist.");

            var srcfiles = new DirectoryInfo(srcdir).EnumerateFiles();

            var tempLog = new List<string>();
            foreach (var file in srcfiles)
            {
                //TODO only one match, multiple could indicate we picked up something irrelevant. There are still other ways to mess this up that need to be sorted too.
                if (!regex_sn.IsMatch(file.Name))
                    tempLog.Add($"'{file.Name}' missing season identifier.");

                if (!regex_ep.IsMatch(file.Name))
                    tempLog.Add($"'{file.Name}' missing episode identifier.");
            }

            if (tempLog.Any())
            {
                ret.Add("One or more file names were not in the correct format. Please review the files below and ensure every file includes an identifier for the season and episode (such as 'S01E02' or 'S01.E02' or even 'S01.TitleOfShow.E02'). This is case insensitive. ");
                ret.AddRange(tempLog);
                return ret;
            }

            foreach (var file in srcfiles)
            {
                var season = regex_sn.Match(file.Name).Value;
                var episode = regex_ep.Match(file.Name).Value;
                var newName = $@"{file.DirectoryName}\{name} - {season}{episode}{file.Extension}";
                try
                {
                    file.MoveTo(newName);
                }
                catch (Exception e)
                {
                    ret.Add(e.Message);
                    return ret;
                }
            }
            return ret;
        }
    }

    enum MediaTypes : byte
    {
        Movie = 0,
        TV = 1,
        Music = 2,
        Picture = 3,
        Unknown = 255
    }
}
