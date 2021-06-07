using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Extensions;
using StackLogParser.Entities;
using StackLogParser.Models;
using StackLogParser.Options;
using StackLogParser.Services;
using Xunit;

namespace StackLogParserTests
{
    public class ReportServiceTests
    {
        private readonly ILogger<ReportService> _logger;
        private readonly ILogger<LogReaderService> _logReaderLogger;
        private readonly IOptions<LogEntryOptions> _logEntryOptions;

        public ReportServiceTests()
        {
            _logger = Substitute.For<ILogger<ReportService>>();
            _logReaderLogger = Substitute.For<ILogger<LogReaderService>>();
            var logEntryOptions = new LogEntryOptions()
            {
                ColumnSplitter = ",",
                ExpectedColumnCount = 6
            };
            _logEntryOptions = Options.Create<LogEntryOptions>(logEntryOptions);
        }

        #region Request Bytes Tests
        [Fact]
        public async void Should_Add_To_Data_When_Dates_Match()
        {
            var reportService = new ReportService(_logger);
            var reportRequest = new ReportRequest()
            {
                ByteAverageWindowStart = DateTime.Parse("2020-01-01 00:00:01Z"),
                ByteAverageWindowEnd = DateTime.Parse("2020-01-01 01:00:01Z")
            };
            await reportService.UpdateRequestByteSizeDataAsync(reportRequest, new LogEntry()
            {
                RequestSize = 10,
                LogTime = DateTime.Parse("2020-01-01 00:00:02Z")
            });

            Assert.Equal(1, reportService.ByteAverageData.TotalRequestCount);
            Assert.Equal(10, reportService.ByteAverageData.TotalRequestSize);
        }

        [Fact]
        public async void Should_Not_Add_To_Data_When_Dates_Dont_Match()
        {
            var reportService = new ReportService(_logger);
            var reportRequest = new ReportRequest()
            {
                ByteAverageWindowStart = DateTime.Parse("2020-01-01 00:00:01Z"),
                ByteAverageWindowEnd = DateTime.Parse("2020-01-01 01:00:01Z")
            };
            await reportService.UpdateRequestByteSizeDataAsync(reportRequest, new LogEntry()
            {
                RequestSize = 10,
                LogTime = DateTime.Parse("2020-01-10 00:00:02Z")
            });

            Assert.Equal(0, reportService.ByteAverageData.TotalRequestCount);
            Assert.Equal(0, reportService.ByteAverageData.TotalRequestSize);
        }
        #endregion

        #region user agent tests
        [Fact]
        public async void Should_Increment_UserAgent_Count_When_Matched()
        {
            var reportService = new ReportService(_logger);
            var reportRequest = new ReportRequest()
            {
                LookupUserAgent = "botson",
                LookupUserAgentMethod = "GET"
            };
            await reportService.AddOrUpdateUserAgentAsync(reportRequest, new LogEntry()
            {
                UserAgent = "botson",
                HttpMethod = new System.Net.Http.HttpMethod("GET")
            });

            Assert.Equal(1, reportService.LookupUserAgentCount);
        }

        [Fact]
        public async void Should_Not_Increment_UserAgent_Count_When_Not_Matched()
        {
            var reportService = new ReportService(_logger);
            var reportRequest = new ReportRequest()
            {
                LookupUserAgent = "botsonny",
                LookupUserAgentMethod = "GET"
            };
            await reportService.AddOrUpdateUserAgentAsync(reportRequest, new LogEntry()
            {
                UserAgent = "botson",
                HttpMethod = new System.Net.Http.HttpMethod("GET")
            });

            Assert.Equal(0, reportService.LookupUserAgentCount);
        }
        #endregion

        #region ip address tests
        [Fact]
        public async void Should_Add_And_Increment_IpAddress_Counters()
        {
            var reportService = new ReportService(_logger);
            await reportService.AddOrUpdateIpEntryAsync(new LogEntry()
            {
                SourceIP = IPAddress.Parse("10.9.8.7")
            });

            Assert.Equal(1, reportService.IpAddressDictionary.Count);
            Assert.Equal(1, reportService.IpAddressDictionary["10.9.8.7"]);

            await reportService.AddOrUpdateIpEntryAsync(new LogEntry()
            {
                SourceIP = IPAddress.Parse("10.9.8.7")
            });

            Assert.Equal(1, reportService.IpAddressDictionary.Count);
            Assert.Equal(2, reportService.IpAddressDictionary["10.9.8.7"]);
        }
        #endregion

        #region report tests
        [Theory]
        [InlineData(@"2020-04-23 20:08:20Z, 10.42.127.77, GET, 200, 53704, useragent: edge
2020 - 04 - 23 20:08:22Z, 192.168.170.54, GET, 200, 24007, useragent: chrome
2020 - 04 - 23 20:08:24Z, 192.168.248.125, GET, 200, 37084, useragent: firefox
2020 - 04 - 23 20:08:25Z, 192.168.56.205, GET, 200, 56887, useragent: edge
2020 - 04 - 23 20:08:27Z, 192.168.74.239, GET, 200, 63843, useragent: firefox
2020 - 04 - 23 20:08:28Z, 10.189.13.110, GET, 200, 3918, useragent: chrome
2020 - 04 - 23 20:08:30Z, 10.97.110.114, GET, 200, 5561, useragent: chrome
2020 - 04 - 23 20:08:32Z, 192.168.135.206, GET, 200, 8657, useragent: chrome
2020 - 04 - 23 20:08:33Z, 10.180.92.145, GET, 500, 26005, useragent: edge
2020 - 04 - 23 20:08:35Z, 10.141.191.34, GET, 200, 2458, useragent: chrome
2020 - 04 - 23 20:08:36Z, 172.16.210.16, GET, 200, 34103, useragent: edge
2020 - 04 - 23 20:08:38Z, 192.168.55.112, GET, 200, 76503, useragent: badbot
2020 - 04 - 23 20:08:39Z, 10.17.37.69, GET, 200, 60466, useragent: chrome
2020 - 04 - 23 20:08:41Z, 172.16.66.42, GET, 200, 46570, useragent: edge
2020 - 04 - 23 20:08:43Z, 192.168.212.81, GET, 200, 57533, useragent: chrome
2020 - 04 - 23 20:08:44Z, 192.168.212.81, GET, 200, 57533, useragent: chrome")]
        public async void Should_Yield_Expected_Report_Data(string inputData)
        {
            // setup the reader so we can just play with the text on the theory rather than needing an actual file
            var logReaderService = Substitute.ForPartsOf<LogReaderService>(_logReaderLogger, _logEntryOptions);
            logReaderService.Configure().TestFilePath("test").Returns(true);
            logReaderService.Configure().GetFileStream("test").Returns(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(inputData))));

            var logEntries = logReaderService.ReadAsync("test");

            // send our stream to the reporting processor
            var reportService = new ReportService(_logger);
            var report = await reportService.GetReportAsync(new ReportRequest()
            {
                ByteAverageWindowStart = DateTime.Parse("2020 - 04 - 23 20:08:36Z"),
                ByteAverageWindowEnd = DateTime.Parse("2020 - 04 - 24 20:08:36Z"),
                LogFile = "test",
                LookupIpAddress = "10.180.92.145",
                LookupUserAgent = "chrome",
                LookupUserAgentMethod = "GET"
            }, logEntries, CancellationToken.None);

            // validate the report data
            Assert.Equal(1, report.LookupIpRequestCount);
            Assert.Equal(8, report.LookupUserAgentRequestCount);
            Assert.Equal(55451, report.LookupAverageRequestBytes);
            Assert.Equal("192.168.212.81", report.MostCommonIp);
        }
        #endregion
    }
}
