using System.Collections.Generic;
using System.IO;
using StackLogParser.Entities;

namespace StackLogParser.Abstractions
{
    /// <summary>
    /// An interface describing the supported operations of a LogReaderService
    /// The responsibility of services which implement this interface is to be able to parse a log file
    /// and stream out our known types
    /// </summary>
    public interface ILogReaderService
    {
        /// <summary>
        /// A method that given a log yield an async data stream for consumption
        /// </summary>
        /// <param name="path"> the path of the file to process </param>
        /// <returns> An async data stream of ILogEntry objects </returns>
        public IAsyncEnumerable<ILogEntry> ReadAsync(string path);

        /// <summary>
        /// A simple abstraction to enable easy mock of access to the file
        /// </summary>
        /// <param name="path"> the path of the file to process </param>
        /// <returns></returns>
        public bool TestFilePath(string path);

        /// <summary>
        /// A simple abstraction to enable easy mock of access to the file
        /// </summary>
        /// <param name="path"> the path of the file to process </param>
        /// <returns></returns>
        public StreamReader GetFileStream(string path);

        /// <summary>
        /// A method that can split a line into an ILogEntry object
        /// </summary>
        /// <param name="line"> The line to process </param>
        /// <param name="line"> The number of the line in the file </param>
        /// <returns></returns>
        public ILogEntry ParseLine(string line, long lineNumber);
    }
}
