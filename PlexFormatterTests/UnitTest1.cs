using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlexFormatter;

namespace PlexFormatterTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RunTv()
        {
            FormatterBase tvformat = new TvFormatter("Mr. Robot", 2, @"C:\Temp\MrRb2\");
            tvformat.FormatAndImport();
        }

        [TestMethod]
        public void RunMovie()
        {
            FormatterBase movieformat = new MovieFormatter("Oblivion", @"C:\Temp\obl\");
            movieformat.FormatAndImport();
        }
    }
}
