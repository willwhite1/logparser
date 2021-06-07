using System;
namespace StackLogParser.Exceptions
{
    /// <summary>
    /// An exception that represents an unexpected log entry line format.
    /// This could happen if a new column is added or removed for instance.
    /// </summary>
    public class UnexpectedLogEntryFormatException : Exception
    {
        public UnexpectedLogEntryFormatException(string message) : base(message)
        {
        }
    }
}
