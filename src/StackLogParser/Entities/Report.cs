using System.Net;

namespace StackLogParser.Entities
{
    /// <summary>
    /// An interface describing the members of our Report object which we provide as output
    /// </summary>
    public interface IReport
    {
        /// <summary>
        /// The Count of requests by the target IP address for the report
        /// </summary>
        public long LookupIpRequestCount { get; set; }

        /// <summary>
        /// The Count of requests by the target UserAgent
        /// </summary>
        public long LookupUserAgentRequestCount { get; set; }

        /// <summary>
        /// The most common IP in the log that was searched
        /// </summary>
        public string MostCommonIp { get; set; }

        /// <summary>
        /// The average request size in bytes for the target date range for this report.
        /// </summary>
        public long LookupAverageRequestBytes { get; set; }
    }

    public class Report : IReport
    {
        public Report()
        {
        }

        /// <summary>
        /// The Count of requests by the target IP address for the report
        /// </summary>
        public long LookupIpRequestCount { get; set; }

        /// <summary>
        /// The Count of requests by the target UserAgent
        /// </summary>
        public long LookupUserAgentRequestCount { get; set; }

        /// <summary>
        /// The most common IP in the log that was searched
        /// </summary>
        public string MostCommonIp { get; set; }

        /// <summary>
        /// The average request size in bytes for the target date range for this report.
        /// </summary>
        public long LookupAverageRequestBytes { get; set; }
    }
}
