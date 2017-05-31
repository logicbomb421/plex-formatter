using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexFormatter
{
    public static class Defaults
    {
        public const string PLEX_ROOT_MOVIE             = @"C:\Plex\Movies\";
        public const string PLEX_ROOT_TV                = @"C:\Plex\TV Shows\";
        public const string PLEX_ROOT_PHOTO             = @"C:\Plex\Photos\";
        public const string PLEX_ROOT_MUSIC             = @"C:\Plex\Music\";
        public const bool   PLEX_REFRESH_ON_IMPORT      = false;
        public const bool   PLEX_DELETE_SOURCE_FILES    = false;

        public static readonly string[] VideoExtensions = { "mp4", "mkv" };
    }
}
