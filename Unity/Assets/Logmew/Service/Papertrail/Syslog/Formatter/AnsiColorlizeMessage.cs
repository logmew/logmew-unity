namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// Add ANSI color code to message.
	/// </summary>
	public class AnsiColorlizeMessage : ILogEntryMessageFormatter
	{
		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="message">Message.</param>
		/// <param name="logEntry">Log entry.</param>
		public string FormatMessage(string message, ILogEntry logEntry)
		{
			switch (logEntry.LogLevel) {
				case LogLevel.Assert:
					return string.Format("\x1b[35m{0}\x1b[0m", message);  // purple
				case LogLevel.Exception:
				case LogLevel.Error:
					return string.Format("\x1b[31m{0}\x1b[0m", message);  // red
				case LogLevel.Warning:
					return string.Format("\x1b[33m{0}\x1b[0m", message);  // yellow
				case LogLevel.Info:
					return string.Format("\x1b[36m{0}\x1b[0m", message);  // cyan
				case LogLevel.Debug:
				default:
					return message;
			}
		}
	}
}
