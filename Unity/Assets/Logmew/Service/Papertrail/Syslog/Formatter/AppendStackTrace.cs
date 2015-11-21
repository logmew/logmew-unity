namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// Append stack trace to tail of ILogEntry.Message.
	/// </summary>
	public class AppendStackTrace : ILogEntryMessageFormatter
	{
		private string delimiter;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="delimiter">Delimiter string, used between message and stacktrace. Default is " @@ "</param>
		public AppendStackTrace(string delimiter = " @@ ")
		{
			this.delimiter = delimiter;
		}

		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="message">Message.</param>
		/// <param name="logEntry">Log entry.</param>
		public string FormatMessage(string message, ILogEntry logEntry)
		{
			return string.Format("{0}{1}{2}", message, delimiter, logEntry.StackTrace);
		}
	}
}
