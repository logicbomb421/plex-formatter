using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlexFormatter.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlexFormatter.PlexFormatterResult;

namespace PlexFormatterTests.FormatterTests
{
    [TestClass]
    public class MovieTests
    {
        private readonly Random _rand = new Random();
        private const int TEST_FILE_SIZE = (1024 * 1024) * 50;
        private const int MIN_YEAR = 1900;
        private int _maxYear = 0;
        private int _year = 0;
        private string _plexRoot;
        private string _source;
        private string _fileName;
        private string _title;
        private string[] _supportedExts;

        private string getValidExt => _supportedExts[_rand.Next(0, _supportedExts.Length)];

        [TestInitialize]
        public void Init()
        {
            _supportedExts = new[]
            {
                "mp4",
                "mkv"
            };

            _title = generateRandomString();
            _fileName = changeExtension(Path.GetRandomFileName(), getValidExt);
            _plexRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var source = Path.Combine(Path.GetTempPath(), _fileName);
            File.WriteAllBytes(source, new byte[TEST_FILE_SIZE]);
            _source = source;

            //rule is end of current decade
            var year = DateTime.Now.Year;
            while (year % 10 != 0) //not a 0 year
                ++year;
            _maxYear = --year; //shift down to the previous 9
            _year = _rand.Next(MIN_YEAR, _maxYear);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_plexRoot))
                Directory.Delete(_plexRoot, recursive: true);

            if (File.Exists(_source))
                File.Delete(_source);
        }

        private string changeExtension(string file, string ext) => $"{file.Substring(0, file.LastIndexOf('.'))}.{ext}";
        private string generateRandomString(int numChars = 15) 
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            return new string(Enumerable.Repeat(chars, numChars)
              .Select(s => s[_rand.Next(s.Length)]).ToArray());
        }

        [TestMethod]
        public void Formatter_IsValid_WithYear()
        {
            var f = new MovieFormatter(_source, _title, false, _plexRoot, _year.ToString());
            var result = f.Validate();
            Assert.IsTrue(f.IsValidated, "Formatter was not in valid state after running validation method.");
            Assert.IsTrue(result.Status == ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
        }

        [TestMethod]
        public void Formatter_IsValid_YearInFilename()
        {
            File.Delete(_source);
            var fi = new FileInfo(_source);
            string dir = fi.DirectoryName, name = fi.Name, ext = fi.Extension;
            name = $"{name}.{_year}.{generateRandomString(5)}";
            _source = Path.Combine(dir, $"{name}.{ext}");
            File.Create(_source);
            
            var f = new MovieFormatter(_source, _title, false, _plexRoot);
            var result = f.Validate();
            Assert.IsTrue(f.IsValidated, "Formatter was not in valid state after running validation method.");
            Assert.IsTrue(result.Status == ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
        }

        [TestMethod]
        public void Formatter_Invalid_NoYear()
        {
            var f = new MovieFormatter(_source, _title, false, _plexRoot);
            var result = f.Validate();
            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state without supplying year identifier.");
            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success without supplying year identifier.");
        }

        [TestMethod]
        public void Formatter_Invalid_YearOverMax()
        {
            var f = new MovieFormatter(_source, _title, false, _plexRoot, (_maxYear + 1).ToString());
            var result = f.Validate();
            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state with incorrect year.");
            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success with incorrect year.");
        }
    }
}
