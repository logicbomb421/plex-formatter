using System.IO;

namespace PlexFormatter
{
    public class ProgressReportingFileCopier
    {
        public delegate void OnCompleteHandler();
        public event OnCompleteHandler OnComplete;
        public delegate void OnUpdateHandler(int percentComplete);
        public event OnUpdateHandler OnUpdate;

        public bool IsComplete { get; set; } = false;
        public string Source { get; set; }
        public string Destination { get; set; }

        public ProgressReportingFileCopier(string source, string dest)
        {
            Source = source;
            Destination = dest;
        }

        public void Copy()
        {
            using (var source = new FileStream(Source, FileMode.Open, FileAccess.Read))
            using (var dest = new FileStream(Destination, FileMode.CreateNew, FileAccess.Write))
            {
                long update_chunk = source.Length / 20;
                long next_update = update_chunk;
                byte percent_complete = 0;
                OnUpdate?.Invoke(percent_complete);
                byte[] buffer = new byte[4096];
                long total_bytes = 0;
                int bytes_read = 0;
                while ((bytes_read = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dest.Write(buffer, 0, bytes_read);
                    total_bytes += bytes_read;
                    if (total_bytes >= next_update)
                    {
                        next_update += update_chunk;
                        percent_complete += 5;
                        OnUpdate?.Invoke(percent_complete);
                    }
                }
            }
            IsComplete = true;
            OnComplete?.Invoke();
        }
    }
}
