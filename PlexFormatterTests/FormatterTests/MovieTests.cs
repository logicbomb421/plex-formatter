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

        private List<string> _files = new List<string>();
        private List<string> _directories = new List<string>();

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
            _directories.Add(_plexRoot);
            var source = Path.Combine(Path.GetTempPath(), _fileName);
            File.WriteAllBytes(source, new byte[TEST_FILE_SIZE]);
            _source = source;
            _files.Add(_source);

            //rule is end of current decade (this will be an issue in 2020 lol... revisit)
            var year = DateTime.Now.Year;
            while (year % 10 != 0) //not a 0 year
                ++year;
            _maxYear = --year; //shift down to the previous 9
            _year = _rand.Next(MIN_YEAR, _maxYear);
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var d in _directories)
            {
                if (Directory.Exists(d))
                    Directory.Delete(d, recursive: true);
            }

            foreach (var f in _files)
            {
                if (File.Exists(f))
                    File.Delete(f);
            }
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
            var f = new MovieFormatter(_source, _title, false, _plexRoot, _year);
            var result = f.Validate();
            Assert.IsTrue(f.IsValidated, "Formatter was not in valid state after running validation method.");
            Assert.IsTrue(result.Status == ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
            Assert.IsTrue(f.Year.GetValueOrDefault(-1) == _year,
                $"Formatter year did not match provided year. Formatter: {f.Year.GetValueOrDefault(-1)} | Provided: {_year}");
            Assert.IsTrue(f.Movie.Year == _year, $"Movie year did not match provided year. Movie: {f.Movie.Year} | Provided: {_year}");
        }

        [TestMethod]
        public void Formatter_IsValid_YearInFilename()
        {
            var file = $"{generateRandomString()}.{_year}.{generateRandomString()}.{getValidExt}";
            _source = Path.Combine(Path.GetTempPath(), file);
            _files.Add(_source);
            using (File.Create(_source)) ; //noop
            
            var f = new MovieFormatter(_source, _title, false, _plexRoot);
            var result = f.Validate();
            Assert.IsTrue(f.IsValidated, "Formatter was not in valid state after running validation method.");
            Assert.IsTrue(result.Status == ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
            Assert.IsTrue(f.Year.GetValueOrDefault(-1) == _year, 
                $"Formatter year did not match provided year. Formatter: {f.Year.GetValueOrDefault(-1)} | Provided: {_year}");
            Assert.IsTrue(f.Movie.Year == _year, $"Movie year did not match provided year. Movie: {f.Movie.Year} | Provided: {_year}");
        }

        [TestMethod]
        public void Formatter_Invalid_NoYear()
        {
            var f = new MovieFormatter(_source, _title, false, _plexRoot);
            var result = f.Validate();
            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state without supplying year identifier.");
            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success without supplying year identifier.");
            Assert.IsTrue(!f.Year.HasValue, $"Formatter year was not null when no year was found via regex. Current value: {f.Year.GetValueOrDefault(-1)}");
            Assert.IsTrue(f.Movie.Year == -1, $"Movie year had non-default vaule when no year was found via regex. Current value: {f.Movie.Year}");
        }

        [TestMethod]
        public void Formatter_Invalid_YearOverMax()
        {
            //currently if you provide a year, it can be whatever you want. we only check the years we find via regex
            var file = $"{generateRandomString()}.{_maxYear + 1}.{generateRandomString()}.{getValidExt}";
            _source = Path.Combine(Path.GetTempPath(), file);
            _files.Add(_source);
            using (File.Create(_source)) ; //noop

            var f = new MovieFormatter(_source, _title, false, _plexRoot);
            var result = f.Validate();
            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state with incorrect year.");
            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success with incorrect year.");
            Assert.IsTrue(!f.Year.HasValue, $"Formatter year was not null when no year was found via regex. Current value: {f.Year.GetValueOrDefault(-1)}");
            Assert.IsTrue(f.Movie.Year == -1, $"Movie year had non-default vaule when no year was found via regex. Current value: {f.Movie.Year}");
        }

        [TestMethod]
        public void Formatter_Invalid_YearUnderMin()
        {
            //currently if you provide a year, it can be whatever you want. we only check the years we find via regex
            var file = $"{generateRandomString()}.{MIN_YEAR - 1}.{generateRandomString()}.{getValidExt}";
            _source = Path.Combine(Path.GetTempPath(), file);
            _files.Add(_source);
            using (File.Create(_source)) ; //noop

            var f = new MovieFormatter(_source, _title, false, _plexRoot);
            var result = f.Validate();
            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state with incorrect year.");
            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success with incorrect year.");
            Assert.IsTrue(!f.Year.HasValue, $"Formatter year was not null when no year was found via regex. Current value: {f.Year.GetValueOrDefault(-1)}");
            Assert.IsTrue(f.Movie.Year == -1, $"Movie year had non-default vaule when no year was found via regex. Current value: {f.Movie.Year}");
        }
    }
}
