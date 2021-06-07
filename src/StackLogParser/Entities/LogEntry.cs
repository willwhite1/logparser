using System;
using System.Net;
using System.Net.Http;

namespace StackLogParser.Entities
{
    /// <summary>
    /// An interface describing the public members of a LogEntry item.
    /// A LogEntry item represents a single disected log line split into its constituent parts
    /// </summary>
    public interface ILogEntry
    {
        /// <summary>
        /// The UTC DateTime of the log entry
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// The IPv4 Source IP of the request
        /// </summary>
        public IPAddress SourceIP { get; set; }

        /// <summary>
        /// The HTTP method that the request used
        /// </summary>
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// The HTTP Response code of the response to the request
        /// </summary>
        public int HttpResponseCode { get; set; }

        /// <summary>
        /// The size of the HTTP request payload in Bytes
        /// </summary>
        public int RequestSize { get; set; }

        /// <summary>
        /// The user agent of the HTTP request
        /// </summary>
        public string UserAgent { get; set; }
    }

    public class LogEntry : ILogEntry
    {
        public LogEntry() { }

        /// <summary>
        /// Overload to support creation with pre-split values
        /// </summary>
        /// <param name="logTime"></param>
        /// <param name="sourceIP"></param>
        /// <param name="httpMethod"></param>
        /// <param name="httpResponseCode"></param>
        /// <param name="requestSize"></param>
        /// <param name="userAgent"></param>
        public LogEntry(DateTime logTime, IPAddress sourceIP, HttpMethod httpMethod, int httpResponseCode, int requestSize, string userAgent)
        {
            LogTime = logTime;
            SourceIP = sourceIP;
            HttpMethod = httpMethod;
            HttpResponseCode = httpResponseCode;
            RequestSize = requestSize;
            UserAgent = userAgent;
        }

        /// <summary>
        /// The UTC DateTime of the log entry
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// The IPv4 Source IP of the request
        /// </summary>
        public IPAddress SourceIP { get; set; }

        /// <summary>
        /// The HTTP method that the request used
        /// </summary>
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// The HTTP Response code of the response to the request
        /// </summary>
        public int HttpResponseCode { get; set; }

        /// <summary>
        /// The size of the HTTP request payload in Bytes
        /// </summary>
        public int RequestSize { get; set; }

        /// <summary>
        /// The user agent of the HTTP request
        /// </summary>
        public string UserAgent { get; set; }
    }
}
