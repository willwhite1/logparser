namespace StackLogParser.Options
{
    /// <summary>
    /// A class that represents all of the configuration levers this application supports that relate
    /// to performance optimization
    /// </summary>
    public class LogEntryOptions
    {
        /// <summary>
        /// The character with which to split our log line. Not likely to change... but who knows.
        /// </summary>
        public string ColumnSplitter { get; set; }

        /// <summary>
        /// The number of columns we expect to see in a given line based on the configured splitter character
        /// </summary>
        public int ExpectedColumnCount { get; set; }
    }
}
