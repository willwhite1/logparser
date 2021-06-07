using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackLogParser.Abstractions;
using StackLogParser.Entities;
using StackLogParser.Models;

namespace StackLogParser.Services
{
    /// <summary>
    /// An implementation of a service that can take in a data stream of log entries and generate our report output
    /// </summary>
    public class ReportService : IReportService
    {
        /// <summary>
        /// Our logger
        /// </summary>
        private readonly ILogger<ReportService> _logger;

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

        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
            IpAddressDictionary = new Dictionary<string, long>();
            LookupUserAgentCount = 0;
            ByteAverageData = new ByteAveragingData();
        }

        /// <summary>
        /// A method that can add or update the counter for an IP address
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public Task AddOrUpdateIpEntryAsync(ILogEntry logEntry)
        {
            if (IpAddressDictionary.ContainsKey(logEntry.SourceIP.ToString()))
            {
                IpAddressDictionary[logEntry.SourceIP.ToString()] = IpAddressDictionary[logEntry.SourceIP.ToString()] + 1;
            }
            else
            {
                IpAddressDictionary.Add(logEntry.SourceIP.ToString(), 1);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// A method that can add or update the counter for our user agent report aspect
        /// </summary>
        /// <param name="reportRequest"> The report request options </param>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public ValueTask AddOrUpdateUserAgentAsync(ReportRequest reportRequest, ILogEntry logEntry)
        {
            if (!string.Equals(logEntry.UserAgent, reportRequest.LookupUserAgent, StringComparison.InvariantCultureIgnoreCase))
            {
                return ValueTask.CompletedTask;
            }
            if (!string.Equals(logEntry.HttpMethod.Method, reportRequest.LookupUserAgentMethod, StringComparison.InvariantCultureIgnoreCase))
            {
                return ValueTask.CompletedTask;
            }

            // if we hit here we deal with the increment
            LookupUserAgentCount++;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// A method returning a Task that yields our report object
        /// </summary>
        /// <param name="reportRequest"> The report request options </param>
        /// <param name="logEntries"> The async datastream of log entries to report on </param>
        /// <param name="cancellationToken"> A cancellation token used to signify any cancellation </param>
        public async Task<IReport> GetReportAsync(ReportRequest reportRequest, IAsyncEnumerable<ILogEntry> logEntries, CancellationToken cancellationToken)
        {
            // process all of the entries
            await foreach (var entry in logEntries)
            {
                // add / increment our ip counter
                await AddOrUpdateIpEntryAsync(entry);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // process the user agent aspect
                await AddOrUpdateUserAgentAsync(reportRequest, entry);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // update the time boundary based request byte averaging data
                await UpdateRequestByteSizeDataAsync(reportRequest, entry);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            // compose and return our report
            var report = new Report()
            {
                MostCommonIp = IpAddressDictionary.OrderByDescending(x => x.Value).First().Key,
                LookupAverageRequestBytes = ByteAverageData.TotalRequestCount == 0 ? 0 : ByteAverageData.TotalRequestSize / ByteAverageData.TotalRequestCount,
                LookupIpRequestCount = IpAddressDictionary.ContainsKey(reportRequest.LookupIpAddress) ? IpAddressDictionary[reportRequest.LookupIpAddress] : 0,
                LookupUserAgentRequestCount = LookupUserAgentCount
            };

            return report;
        }

        /// <summary>
        /// A method that can update the request byte size data
        /// </summary>
        /// <param name="reportRequest"> The report request options </param>
        /// <param name="logEntry"></param>
        /// <returns></returns>
        public ValueTask UpdateRequestByteSizeDataAsync(ReportRequest reportRequest, ILogEntry logEntry)
        {
            if (logEntry.LogTime >= reportRequest.ByteAverageWindowStart && logEntry.LogTime <= reportRequest.ByteAverageWindowEnd)
            {
                ByteAverageData.TotalRequestSize = ByteAverageData.TotalRequestSize + logEntry.RequestSize;
                ByteAverageData.TotalRequestCount = ByteAverageData.TotalRequestCount + 1;
            }
            return ValueTask.CompletedTask;
        }
    }  
}
