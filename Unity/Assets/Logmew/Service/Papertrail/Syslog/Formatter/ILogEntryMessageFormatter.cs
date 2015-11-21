namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// LogEntry message formatter.
	/// </summary>
	public interface ILogEntryMessageFormatter
	{
		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="message">Message.</param>
		/// <param name="logEntry">Log entry.</param>
		string FormatMessage(string message, ILogEntry logEntry);
	}
}
