using System.Collections.Generic;

namespace PlexFormatter
{
    public enum ResultStatus : byte
    {
        Unknown,
        Failed,
        Success,
    }

    public class Result
    {

        public ResultStatus Status { get; set; } = ResultStatus.Unknown;
        public List<string> Log { get; set; } = new List<string>();

        public Result() { Status = ResultStatus.Unknown; }

        public Result(ResultStatus status)
        {
            Status = status;
        }

        public Result(ResultStatus status, params string[] addToLog)
        {
            Status = status;
            Log.AddRange(addToLog);
        }

        public Result Finalize(ResultStatus status, string addToLog = null)
        {
            Status = status;
            if (!string.IsNullOrEmpty(addToLog))
                Log.Add(addToLog);
            return this;
        }
    }

    public class PlexFormatterResult<T> : Result
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
