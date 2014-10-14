
namespace MSTestAllureAdapter
{
    /// <summary>
    /// Represents an ErrorInfo element in the trx file.
    /// </summary>
    /// <remarks>>This class is immutable.</remarks>
    public class ErrorInfo
    {
        public ErrorInfo(string message) : this(message, null) { }

        public ErrorInfo(string message, string stackTrace) : this(message, stackTrace, null) { }

        public ErrorInfo(string message, string stackTrace, string stdOut)
        {
            Message = message;
            StackTrace = stackTrace;
            StdOut = stdOut;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace { get; private set; }

        /// <summary>
        /// Gets or sets the StdOut.
        /// </summary>
        /// <value>The StdOut.</value>
        public string StdOut { get; private set; }

        public override string ToString()
        {
            return string.Format("[ErrorInfo: Message={0}, StackTrace={1}, StdOut={2}]", Message, StackTrace, StdOut);
        }
    }
}
