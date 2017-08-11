//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using PlexFormatter.Formatters;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static PlexFormatter.Result;

//namespace PlexFormatterTests.FormatterTests
//{
//    [TestClass]
//    public class MovieTests
//    {
//        private readonly Random _rand = new Random();
//        private const int TEST_FILE_SIZE = (1024 * 1024) * 50;
//        private const int MIN_YEAR = 1900;
//        private int _maxYear = 0;
//        private int _year = 0;
//        private string _plexRoot;
//        private string _source;
//        private string _fileName;
//        private string _title;
//        private string[] _supportedExts;
//        private char[] _invalidChars = { '=', '+', '{', '}', ':', '"', '<', '>', '/', '?', '`', '\\', '|', '*', '&', '^', '%', '$', '#', '@', '!' };

//        private List<string> _files = new List<string>();
//        private List<string> _directories = new List<string>();

//        private string getValidExt => _supportedExts[_rand.Next(0, _supportedExts.Length)];

//        [TestInitialize]
//        public void Init()
//        {
//            _supportedExts = new[]
//            {
//                "mp4",
//                "mkv",
//                "avi"
//            };

//            _title = generateRandomString();
//            _fileName = changeExtension(Path.GetRandomFileName(), getValidExt);
//            _plexRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
//            _directories.Add(_plexRoot);
//            var source = Path.Combine(Path.GetTempPath(), _fileName);
//            File.WriteAllBytes(source, new byte[TEST_FILE_SIZE]);
//            _source = source;
//            _files.Add(_source);

//            //TODO rule is end of current decade (this will be an issue in 2020 lol... revisit)
//            var year = DateTime.Now.Year;
//            while (year % 10 != 0) //not a 0 year
//                ++year;
//            _maxYear = --year; //shift down to the previous 9
//            _year = _rand.Next(MIN_YEAR, _maxYear);
//        }

//        [TestCleanup]
//        public void Cleanup()
//        {
//            foreach (var d in _directories)
//            {
//                if (Directory.Exists(d))
//                    Directory.Delete(d, recursive: true);
//            }

//            foreach (var f in _files)
//            {
//                if (File.Exists(f))
//                    File.Delete(f);
//            }
//        }

//        private string changeExtension(string file, string ext) => $"{file.Substring(0, file.LastIndexOf('.'))}.{ext}";
//        private string generateRandomString(int numChars = 15) 
//        {
//            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
//            return new string(Enumerable.Repeat(chars, numChars)
//              .Select(s => s[_rand.Next(s.Length)]).ToArray());
//        }

//        #region Validate()
//        [TestMethod]
//        public void Validate_Valid_ExplicitYear()
//        {
//            var f = new MovieFormatter(_source, _title, false, _plexRoot, _year);
//            var result = f.Validate();
//            Assert.IsTrue(f.IsValidated, "Formatter was not in valid state after running validation method.");
//            Assert.IsTrue(result.Status == ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
//            Assert.IsTrue(f.Year.GetValueOrDefault(-1) == _year,
//                $"Formatter year did not match provided year. Formatter: {f.Year.GetValueOrDefault(-1)} | Provided: {_year}");
//            Assert.IsTrue(f.Movie.Year == _year, $"Movie year did not match provided year. Movie: {f.Movie.Year} | Provided: {_year}");
//        }

//        [TestMethod]
//        public void Validate_Valid_YearInFilename()
//        {
//            var file = $"{generateRandomString()}.{_year}.{generateRandomString()}.{getValidExt}";
//            _source = Path.Combine(Path.GetTempPath(), file);
//            _files.Add(_source);
//            using (File.Create(_source)) ; //noop

//            var f = new MovieFormatter(_source, _title, false, _plexRoot);
//            var result = f.Validate();
//            Assert.IsTrue(f.IsValidated, "Formatter was not in valid state after running validation method.");
//            Assert.IsTrue(result.Status == ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
//            Assert.IsTrue(f.Year.GetValueOrDefault(-1) == _year,
//                $"Formatter year did not match provided year. Formatter: {f.Year.GetValueOrDefault(-1)} | Provided: {_year}");
//            Assert.IsTrue(f.Movie.Year == _year, $"Movie year did not match provided year. Movie: {f.Movie.Year} | Provided: {_year}");
//        }

//        [TestMethod]
//        public void Validate_Invalid_NoYear()
//        {
//            var f = new MovieFormatter(_source, _title, false, _plexRoot);
//            var result = f.Validate();
//            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state without supplying year identifier.");
//            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success without supplying year identifier.");
//            Assert.IsTrue(!f.Year.HasValue, $"Formatter year was not null when no year was found via regex. Current value: {f.Year.GetValueOrDefault(-1)}");
//            Assert.IsTrue(f.Movie.Year == -1, $"Movie year had non-default vaule when no year was found via regex. Current value: {f.Movie.Year}");
//        }

//        [TestMethod]
//        public void Validate_Invalid_YearOverMax()
//        {
//            //currently if you provide a year, it can be whatever you want. we only check the years we find via regex
//            var file = $"{generateRandomString()}.{_maxYear + 1}.{generateRandomString()}.{getValidExt}";
//            _source = Path.Combine(Path.GetTempPath(), file);
//            _files.Add(_source);
//            using (File.Create(_source)) ; //noop

//            var f = new MovieFormatter(_source, _title, false, _plexRoot);
//            var result = f.Validate();
//            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state with incorrect year.");
//            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success with incorrect year.");
//            Assert.IsTrue(!f.Year.HasValue, $"Formatter year was not null when no year was found via regex. Current value: {f.Year.GetValueOrDefault(-1)}");
//            Assert.IsTrue(f.Movie.Year == -1, $"Movie year had non-default vaule when no year was found via regex. Current value: {f.Movie.Year}");
//        }

//        [TestMethod]
//        public void Validate_Invalid_YearUnderMin()
//        {
//            //currently if you provide a year, it can be whatever you want. we only check the years we find via regex
//            var file = $"{generateRandomString()}.{MIN_YEAR - 1}.{generateRandomString()}.{getValidExt}";
//            _source = Path.Combine(Path.GetTempPath(), file);
//            _files.Add(_source);
//            using (File.Create(_source)) ; //noop

//            var f = new MovieFormatter(_source, _title, false, _plexRoot);
//            var result = f.Validate();
//            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state with incorrect year.");
//            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success with incorrect year.");
//            Assert.IsTrue(!f.Year.HasValue, $"Formatter year was not null when no year was found via regex. Current value: {f.Year.GetValueOrDefault(-1)}");
//            Assert.IsTrue(f.Movie.Year == -1, $"Movie year had non-default vaule when no year was found via regex. Current value: {f.Movie.Year}");
//        }

//        [TestMethod]
//        public void Validate_Invalid_MultipleYears()
//        {
//            //currently if you provide a year, it can be whatever you want. we only check the years we find via regex
//            var file = $"{generateRandomString()}.{_year}.{_rand.Next(MIN_YEAR, _maxYear)}.{generateRandomString()}.{getValidExt}";
//            _source = Path.Combine(Path.GetTempPath(), file);
//            _files.Add(_source);
//            using (File.Create(_source)) ; //noop

//            var f = new MovieFormatter(_source, _title, false, _plexRoot);
//            var result = f.Validate();
//            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state with incorrect year.");
//            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success with incorrect year.");
//            Assert.IsTrue(!f.Year.HasValue, $"Formatter year was not null when multiple years were found via regex. Current value: {f.Year.GetValueOrDefault(-1)}");
//            Assert.IsTrue(f.Movie.Year == -1, $"Movie year had non-default vaule when multiple years were found via regex. Current value: {f.Movie.Year}");
//        }

//        [TestMethod]
//        public void Validate_Invalid_NoTitle()
//        {
//            var f = new MovieFormatter(_source, null, false, _plexRoot, _year);
//            var result = f.Validate();
//            Assert.IsTrue(!f.IsValidated, "Formatter was in valid state with incorrect year.");
//            Assert.IsTrue(result.Status != ResultStatus.Success, "Result status was success with incorrect year.");
//            Assert.IsNull(f.Movie.Title, $"Movie title prop was not null even though nothing was supplied. Current value: {f.Movie.Title}");
//        }
//        #endregion

//        #region Format()
//        [TestMethod]
//        public void Format_Success()
//        {
//            var expectedDestinationPath = Path.Combine(_plexRoot, $"{_title} ({_year})", $"{_title} ({_year}){_source.Substring(_source.LastIndexOf('.'))}");
//            var f = new MovieFormatter(_source, _title, false, _plexRoot, _year);

//            var v_result = f.Validate();
//            if (v_result.Status != ResultStatus.Success)
//                Assert.Fail("Formatter didn't pass initial validation."); //not in scope for the test hence explicit assert.fail

//            var result = f.Format();
//            Assert.IsTrue(f.IsFormatted, "Formatter was not in formatted state after running format method.");
//            Assert.AreEqual(result.Status, ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
//            Assert.IsNotNull(f.Movie.DestinationPath, "Destination path was null.");
//            Assert.AreEqual(expectedDestinationPath, f.Movie.DestinationPath, 
//                $"Actual destination path did not match expected. Expected: {expectedDestinationPath} | Actual: {f.Movie.DestinationPath}");
//        }

//        [TestMethod]
//        public void Format_Success_WithInvalidChars()
//        {
//            var expectedDestinationPath = Path.Combine(_plexRoot, $"{_title} ({_year})", $"{_title} ({_year}){_source.Substring(_source.LastIndexOf('.'))}");
//            const int max_invalids = 3;
//            char[] invalidChars = new char[max_invalids];
//            for (int i = 0; i < max_invalids; ++i)
//                invalidChars[i] = _invalidChars[_rand.Next(0, _invalidChars.Length)];
//            _title = $"{_title}{string.Join("", invalidChars)}";

//            var f = new MovieFormatter(_source, _title, false, _plexRoot, _year);

//            var v_result = f.Validate();
//            if (v_result.Status != ResultStatus.Success)
//                Assert.Fail("Formatter didn't pass initial validation."); //not in scope for the test hence explicit assert.fail

//            var result = f.Format();
//            Assert.IsTrue(f.IsFormatted, "Formatter was not in formatted state after running format method.");
//            Assert.AreEqual(result.Status, ResultStatus.Success, $"Result status was not success. Log: \r\n {string.Join("\r\n\t", result.Log)}");
//            Assert.IsNotNull(f.Movie.DestinationPath, "Destination path was null.");
//            Assert.AreEqual(expectedDestinationPath, f.Movie.DestinationPath,
//                $"Actual destination path did not match expected. Expected: {expectedDestinationPath} | Actual: {f.Movie.DestinationPath}");
//            Assert.
//        }
//        #endregion

//        #region Import()

//        #endregion
//    }
//}
