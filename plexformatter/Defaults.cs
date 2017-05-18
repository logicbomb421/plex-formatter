using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexFormatter
{
    public static class Defaults
    {
        public const string PLEX_ROOT_MOVIE = @"C:\Plex\Movies\";
        public const string PLEX_ROOT_TV    = @"C:\Plex\TV Shows\";

        public static readonly string[] VideoExtensions = { "mp4", "mkv" };
    }
}
