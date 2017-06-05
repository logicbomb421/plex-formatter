using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlexFormatter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexFormatter.Formatters;

namespace PlexFormatterTests
{
    [TestClass]
    public class ProgressReportingFileCopierTests
    {
            private string _sourceFile;
            private string _destFile;

            const int TEST_FILE_SIZE = (1024 * 1024) * 50;

            [TestInitialize]
            public void Init()
            {
                var source = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                File.WriteAllBytes(source, new byte[TEST_FILE_SIZE]);
                _sourceFile = source;
                _destFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }

            [TestCleanup]
            public void Cleanup()
            {
                File.Delete(_sourceFile);
                File.Delete(_destFile);
            }

        [TestMethod]
        public void CopyFile()
        {
            var copier = new ProgressReportingFileCopier(_sourceFile, _destFile);
            copier.OnUpdate += (i) => Console.Write($"{i}...");
            copier.OnComplete += () => Console.WriteLine("Complete!");
            copier.Copy();
        }

        [TestMethod]
        public void SpeedTest()
        {
            const int DIM1 = 2;
            const int DIM2 = 50;

            var sw = new Stopwatch();
            int[,] results = new int[DIM1,DIM2]; 
            for (int i = 0; i < DIM2; ++i, File.Delete(_destFile))
            {
                sw.Reset();
                var copier = new ProgressReportingFileCopier(_sourceFile, _destFile);
                sw.Start();
                copier.Copy();
                sw.Stop();
                results[0, i] = (int)sw.ElapsedMilliseconds;

                File.Delete(_destFile);

                sw.Reset();
                sw.Start();
                File.Copy(_sourceFile, _destFile);
                sw.Stop();
                results[1, i] = (int)sw.ElapsedMilliseconds;
            }

            int progRepTotal = 0;
            int fileCopyTotal = 0;
            for (int i = 0; i < DIM1; ++i)
            {
                for (int j = 0; j < DIM2; ++j)
                {
                    if (i == 0)
                        progRepTotal += results[i, j];
                    else
                        fileCopyTotal += results[i, j];
                }
            }

            double progRepAvgMs = (double)progRepTotal / (double)DIM2;
            double fileCopyAvgMs = (double)fileCopyTotal / (double)DIM2;

            //double progRepKbMs = getKbPerMs(TEST_FILE_SIZE, progRepAvgMs);
            //double fileCopyKbMs = getKbPerMs(TEST_FILE_SIZE, fileCopyAvgMs);

            Console.WriteLine($"{DIM2} runs | {TEST_FILE_SIZE} B");
            Console.WriteLine();
            Console.WriteLine($"Average {nameof(ProgressReportingFileCopier)} (PR) speed:");
            Console.WriteLine($"\t{progRepAvgMs} ms/file");
            //Console.WriteLine($"\t{Math.Round(progRepKbMs)} kb/ms");
            Console.WriteLine($"Average {nameof(File)}.{nameof(File.Copy)} (FC) speed:");
            Console.WriteLine($"\t{fileCopyAvgMs} ms/file");
            //Console.WriteLine($"\t{Math.Round(fileCopyKbMs)} kb/ms");

            Console.WriteLine($"100MB would take: PR {progRepAvgMs * 20}ms | FC {fileCopyAvgMs * 20}ms");
            Console.WriteLine($"500MB would take: PR {progRepAvgMs * 100}ms | FC {fileCopyAvgMs * 100}ms");
            Console.WriteLine($"1GB would take: PR {progRepAvgMs * 200}ms | FC {fileCopyAvgMs * 200}ms");
            Console.WriteLine($"2GB would take: PR {progRepAvgMs * 400}ms | FC {fileCopyAvgMs * 400}ms");

            //something below doesnt work.. not getting the numbers im expecting

            //Console.WriteLine();
            //Console.WriteLine("100MB would take:");
            //Console.WriteLine($"\tPR: {getSeconds(getMbToKb(100), progRepKbMs)}s");
            //Console.WriteLine($"\tFC: {getSeconds(getMbToKb(100), fileCopyKbMs)}s");
            //Console.WriteLine("300MB would take:");
            //Console.WriteLine($"\tPR: {(getSeconds(getMbToKb(300), progRepKbMs) * 60)}min");
            //Console.WriteLine($"\tFC: {(getSeconds(getMbToKb(300), fileCopyKbMs) * 60)}min");
            //Console.WriteLine("750MB would take:");
            //Console.WriteLine($"\tPR: {(getSeconds(getMbToKb(750), progRepKbMs) * 60)}min");
            //Console.WriteLine($"\tFC: {(getSeconds(getMbToKb(750), fileCopyKbMs) * 60)}min");
            //Console.WriteLine("1GB would take:");
            //Console.WriteLine($"\tPR: {(getSeconds(getGbToKb(1), progRepKbMs) * 60)}min");
            //Console.WriteLine($"\tFC: {(getSeconds(getGbToKb(1), fileCopyKbMs) * 60)}min");
            //Console.WriteLine("2GB would take:");
            //Console.WriteLine($"\tPR: {(getSeconds(getGbToKb(2), progRepKbMs) * 60)}min");
            //Console.WriteLine($"\tFC: {(getSeconds(getGbToKb(2), fileCopyKbMs) * 60)}min");
            //Console.WriteLine("3GB would take:");
            //Console.WriteLine($"\tPR: {(getSeconds(getGbToKb(3), progRepKbMs) * 60)}min");
            //Console.WriteLine($"\tFC: {(getSeconds(getGbToKb(3), fileCopyKbMs) * 60)}min");
            //Console.WriteLine("5GB would take:");
            //Console.WriteLine($"\tPR: {(getSeconds(getGbToKb(5), progRepKbMs) * 60)}min");
            //Console.WriteLine($"\tFC: {(getSeconds(getGbToKb(5), fileCopyKbMs) * 60)}min");
            //Console.WriteLine("10GB would take:");
            //Console.WriteLine($"\tPR: {(getSeconds(getGbToKb(10), progRepKbMs) * 60)}min");
            //Console.WriteLine($"\tFC: {(getSeconds(getGbToKb(10), fileCopyKbMs) * 60)}min");

            double getKbPerMs(double file_size, double total_ms)
                => (file_size / 1024d) / total_ms;

            double getSeconds(long test_size_kb, double kbms)
                => (test_size_kb / kbms) / 1000d;

            long getMbToKb(int mb)
                => mb * 1024L;

            long getGbToKb(int gb)
                => getMbToKb(gb) * 1024L;
        }
    }
}
