using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackLogParser.Abstractions;
using StackLogParser.Entities;
using StackLogParser.Exceptions;
using StackLogParser.Options;

namespace StackLogParser.Services
{
    /// <summary>
    /// An implementation of our LogReaderService whose responsbility it is to give us an object from a given line of log text
    /// </summary>
    public class LogReaderService : ILogReaderService
    {
        /// <summary>
        /// Our logger... for logging
        /// </summary>
        private readonly ILogger<LogReaderService> _logger;

        /// <summary>
        /// Our configuration options for parsing
        /// </summary>
        private readonly LogEntryOptions _logEntryOptions;

        /// <summary>
        /// public constructor to be supplied our logger by DI container
        /// </summary>
        /// <param name="logger"></param>
        public LogReaderService(ILogger<LogReaderService> logger, IOptions<LogEntryOptions> logEntryOptions)
        {
            _logger = logger;
            _logEntryOptions = logEntryOptions.Value;
        }

        /// <summary>
        /// A method that given a log yield an async data stream for consumption
        /// </summary>
        /// <param name="path"> the path of the file to process </param>
        /// <returns> An async data stream of ILogEntry objects </returns>
        public async IAsyncEnumerable<ILogEntry> ReadAsync(string path)
        {
            TestFilePath(path);
            var lineCounter = 1;
            using var reader = GetFileStream(path);
            while (!reader.EndOfStream)
                yield return ParseLine(reader.ReadLine(), lineCounter);
                lineCounter++;
        }

        /// <summary>
        /// A simple abstraction to enable easy mock of access to the file
        /// </summary>
        /// <param name="path"> the path of the file to process </param>
        /// <returns></returns>
        public virtual bool TestFilePath(string path)
        {
            if (!File.Exists(path))
            {
                var msg = $"Unable to find log at supplied path {path}.";
                _logger.LogError(msg);
                throw new FileNotFoundException(msg);
            }
            return true;
        }

        /// <summary>
        /// A simple abstraction to enable easy mock of access to the file
        /// </summary>
        /// <param name="path"> the path of the file to process </param>
        /// <returns></returns>
        public virtual StreamReader GetFileStream(string path)
        {
            return File.OpenText(path);
        }

        /// <summary>
        /// A method that can split a line into an ILogEntry object
        /// </summary>
        /// <param name="line"> The line to process </param>
        /// <param name="line"> The number of the line in the file </param>
        /// <returns></returns>
        public ILogEntry ParseLine(string line, long lineNumber)
        {
            var splits = line.Split(_logEntryOptions.ColumnSplitter).Select(p => p.Trim()).ToList();
            if (splits.Count != _logEntryOptions.ExpectedColumnCount)
            {
                var msg = $"Line number {lineNumber} had an unexpected number of columns (Expected:{_logEntryOptions.ExpectedColumnCount} Actual:{splits.Count})";
                _logger.LogError(msg);
                throw new UnexpectedLogEntryFormatException(msg);
            }
            // if we hit here we should be in basically reasonable state
            return new LogEntry(DateTime.Parse(splits[0]),
                IPAddress.Parse(splits[1]),
                new HttpMethod(splits[2]),
                Convert.ToInt32(splits[3]),
                Convert.ToInt32(splits[4]),
                splits[5].Split(":")[1].Trim());
        }
    }
}
