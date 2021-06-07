namespace StackLogParser.Entities
{
    /// <summary>
    /// A simple data structure for tracking our byte averaging data
    /// </summary>
    public class ByteAveragingData
    {
        public ByteAveragingData()
        {
            TotalRequestCount = 0;
            TotalRequestSize = 0;
        }

        /// <summary>
        /// The total size of all request within the reporting boundaries
        /// </summary>
        public long TotalRequestSize { get; set; }

        /// <summary>
        /// The total number of all requests within the reporting boundaries
        /// </summary>
        public long TotalRequestCount { get; set; }
    }
}
