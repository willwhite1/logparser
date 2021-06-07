using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StackLogParser.Models
{
    /// <summary>
    /// An API input model allowing a user to specify reporting options
    /// </summary>
    public class ReportRequest
    {
        /// <summary>
        /// The log file (full path) to process
        /// </summary>
        [Required]
        [JsonPropertyName("logFile")]
        public string LogFile { get; set; }

        /// <summary>
        /// The IP Address to lookup
        /// </summary>
        [Required]
        [JsonPropertyName("lookupIpAddress")]
        public string LookupIpAddress { get; set; }

        /// <summary>
        /// The name of the user agent to lookup
        /// </summary>
        [Required]
        [JsonPropertyName("lookupUserAgent")]
        public string LookupUserAgent { get; set; }

        /// <summary>
        /// The user agent method to lookup
        /// </summary>
        [Required]
        [JsonPropertyName("lookupUserAgentMethod")]
        public string LookupUserAgentMethod { get; set; }

        /// <summary>
        /// The datetime window to start the byte size calculation from
        /// </summary>
        [Required]
        [JsonPropertyName("byteAverageWindowStart")]
        public DateTime ByteAverageWindowStart { get; set; }

        /// <summary>
        /// The datetime window to end the byte size calculation
        /// </summary>
        [Required]
        [JsonPropertyName("byteAverageWindowEnd")]
        public DateTime ByteAverageWindowEnd { get; set; }

        /// <summary>
        /// A method to check the validity of the input reporting options
        /// </summary>
        /// <param name="isValid"></param>
        /// <returns></returns>
        public string IsValid(out bool isValid)
        {
            isValid = true;

            if (ByteAverageWindowEnd <= ByteAverageWindowStart)
            {
                isValid = false;
                return $"ByteAverageWindowEnd ({ByteAverageWindowEnd}) is less than or equal to byteAverageWindowStart ({ByteAverageWindowStart})";
            }
            return string.Empty;
        }
    }
}
