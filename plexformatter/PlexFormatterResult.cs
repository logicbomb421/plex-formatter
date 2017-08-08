using System.Collections.Generic;

namespace PlexFormatter
{
    public class PlexFormatterResult
    {
        public enum ResultStatus : byte
        {
            Unknown,
            Failed,
            Success,
        }

        public ResultStatus Status { get; set; } = ResultStatus.Unknown;
        public List<string> Log { get; set; } = new List<string>();

        public PlexFormatterResult() { }

        public PlexFormatterResult(ResultStatus status)
        {
            Status = status;
        }

        public PlexFormatterResult(ResultStatus status, params string[] addToLog)
        {
            Status = status;
            Log.AddRange(addToLog);
        }

        public PlexFormatterResult Finalize(ResultStatus status, string addToLog = null)
        {
            Status = status;
            if (!string.IsNullOrEmpty(addToLog))
                Log.Add(addToLog);
            return this;
        }
    }

    public class PlexFormatterResult<T> : PlexFormatterResult
    {
        public T Data { get; set; }

        public PlexFormatterResult() : base() { }
        public PlexFormatterResult(ResultStatus status) : base(status) { }
        public PlexFormatterResult(ResultStatus status, params string[] addToLog) : base(status, addToLog) { }

        public PlexFormatterResult(T data)
        {
            Data = data;
        }
    }
}
