using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StackLogParser.Entities;
using StackLogParser.Models;

namespace StackLogParser.Abstractions
{
    /// <summary>
    /// An interface describing the members of a service that can produce an expected report output
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Used for tracking IP count information
        /// </summary>
        public IDictionary<string, long> IpAddressDictionary { get; set; }

        /// <summary>
        /// The occurances of the targetted user agent within the log
        /// </summary>
        public long LookupUserAgentCount { get; set; }

        /// <summary>
        /// The data we store and update for our byte request length averaging
        /// </summary>
        public ByteAveragingData ByteAverageData { get; set; }

        /// <summary>
        /// A method that can add or update the counter for an IP address
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public Task AddOrUpdateIpEntryAsync(ILogEntry logEntry);

        /// <summary>
        /// A method that can add or update the counter for our user agent report aspect
        /// </summary>
        /// <param name="reportRequest"> The report request options </param>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public ValueTask AddOrUpdateUserAgentAsync(ReportRequest reportRequest, ILogEntry logEntry);

        /// <summary>
        /// A method returning a Task that yields our report object
        /// </summary>
        /// <param name="reportRequest"> The report request options </param>
        /// <param name="logEntries"> The async datastream of log entries to report on </param>
        /// <param name="cancellationToken"> A cancellation token used to signify any cancellation </param>
        /// <returns></returns>
        public Task<IReport> GetReportAsync(ReportRequest reportRequest, IAsyncEnumerable<ILogEntry> logEntries, CancellationToken cancellationToken);

        /// <summary>
        /// A method that can update the request byte size data
        /// </summary>
        /// <param name="reportRequest"> The report request options </param>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public ValueTask UpdateRequestByteSizeDataAsync(ReportRequest reportRequest, ILogEntry logEntry);
    }
}
