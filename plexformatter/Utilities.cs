using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexFormatter
{
    public static class Utilities
    {
        public static bool IsDirectory(string path)
        {
            try
            {
                return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
            }
            catch (Exception)
            {
                return false;
            }
        }
            
    }
}
