using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Extensions;
using StackLogParser.Entities;
using StackLogParser.Options;
using StackLogParser.Services;
using Xunit;

namespace StackLogParserTests
{
    public class LogReaderServiceTests
    {
        private readonly ILogger<LogReaderService> _logger;
        private readonly IOptions<LogEntryOptions> _logEntryOptions;

        public LogReaderServiceTests()
        {
            _logger = Substitute.For<ILogger<LogReaderService>>();
            var logEntryOptions = new LogEntryOptions()
            {
                ColumnSplitter = ",",
                ExpectedColumnCount = 6
            };
            _logEntryOptions = Options.Create<LogEntryOptions>(logEntryOptions);
        }

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
2020 - 04 - 23 20:08:43Z, 192.168.212.81, GET, 200, 57533, useragent: chrome")]
        public async void Should_Parse_Expected_Log_Format(string inputData)
        {
            // setup the reader so we can just play with the text on the theory rather than needing an actual file
            var logReaderService = Substitute.ForPartsOf<LogReaderService>(_logger, _logEntryOptions);
            logReaderService.Configure().TestFilePath("test").Returns(true);
            logReaderService.Configure().GetFileStream("test").Returns(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(inputData))));

            var logEntries = logReaderService.ReadAsync("test");
            // we have to get the materialize task outputs from this data stream
            var list = new List<ILogEntry>();
            await foreach (var item in logEntries)
            {
                list.Add(item);
            }

            Assert.Equal(15, list.Count);

            Assert.Equal(DateTime.Parse("2020-04-23 20:08:20Z"), list[0].LogTime);
            Assert.Equal(IPAddress.Parse("10.42.127.77"), list[0].SourceIP);
            Assert.Equal(new HttpMethod("GET"), list[0].HttpMethod);
            Assert.Equal(200, list[0].HttpResponseCode);
            Assert.Equal(53704, list[0].RequestSize);
            Assert.Equal("edge", list[0].UserAgent);

            Assert.Equal(DateTime.Parse("2020-04-23 20:08:30Z"), list[6].LogTime);
            Assert.Equal(IPAddress.Parse("10.97.110.114"), list[6].SourceIP);
            Assert.Equal(new HttpMethod("GET"), list[6].HttpMethod);
            Assert.Equal(200, list[6].HttpResponseCode);
            Assert.Equal(5561, list[6].RequestSize);
            Assert.Equal("chrome", list[6].UserAgent);
        }
    }
}
