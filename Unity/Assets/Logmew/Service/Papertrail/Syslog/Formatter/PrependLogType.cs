using UnityEngine;
using System.Collections;

namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// Insert LogType string to head of message.
	/// </summary>
	public class PrependLogType : ILogEntryMessageFormatter
	{
		private string delimiter;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="delimiter">Delimiter.</param>
		public PrependLogType(string delimiter = " ")
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
			switch (logEntry.LogLevel) {
				case LogLevel.Error:
					return string.Format("[ERROR]{0}{1}", delimiter, message);
				case LogLevel.Assert:
					return string.Format("[ASSRT]{0}{1}", delimiter, message);
				case LogLevel.Exception:
					return string.Format("[EXCPT]{0}{1}", delimiter, message);
				case LogLevel.Warning:
					return string.Format("[WARN] {0}{1}", delimiter, message);
				case LogLevel.Info:
					return string.Format("[INFO] {0}{1}", delimiter, message);
				case LogLevel.Debug:
					return string.Format("[DEBUG]{0}{1}", delimiter, message);
				default:
					return string.Format("[{0}]{1}{2}", logEntry.LogLevel.ToString(), delimiter, message);
			}
		}
	}
}
