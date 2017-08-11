using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlexFormatter;
using PlexFormatter.Formatters;

namespace PlexFormatterTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RunTv()
        {
            //FormatterBase tvformat = new TvFormatter("Mr. Robot", 2, @"C:\Temp\MrRb2\");
            //tvformat.FormatAndImport();
        }

        [TestMethod]
        public void RunMovie()
        {
            //var mov = new MovieFormatter(@"C:\Users\Michael\Downloads\Rogue One (2016) [1080p] [YTS.AG]\", "Rogue One");
            //mov.Validate();
            //mov.Format();
            //mov.Import();
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var tv = new TvFormatter(@"C:\Users\Michael\Downloads\Fringe Season 1", "Fringe", 0);
            var r = tv.Validate();
        }
    }
}
