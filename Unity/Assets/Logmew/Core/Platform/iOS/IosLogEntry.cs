using System;
using UnityEngine;

namespace Logmew
{
#if !UNITY_EDITOR && UNITY_IOS
	/// <summary>
	/// iOS log entry data.
	/// </summary>
	public class IosLogEntry : ILogEntry
	{
		/// <summary>
		/// Log timestamp(Unix timestamp in seconds, sub mili seconds precision).
		/// </summary>
		public readonly double timestamp;
		public readonly LogLevel logLevel;
		public readonly String tag;
		public readonly string message;
		public readonly String stackTrace;

		/// <summary>
		/// Log origin name.
		/// </summary>
		/// <value>Log origin name.</value>
		public string Origin { get { return "iOS"; } }

		/// <summary>
		/// Log timestamp
		/// </summary>
		public DateTimeOffset TimeStamp { get { return LogTime.FromUnixTimestampSec(timestamp); } }

		/// <summary>
		/// Log level
		/// </summary>
		/// <value>The log level.</value>
		public LogLevel LogLevel { get { return logLevel; } }

		/// <summary>
		/// Tag
		/// </summary>
		/// <value>The tag.</value>
		public string Tag { get { return tag; } }

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message { get { return message; } }

		/// <summary>
		/// Stack trace
		/// </summary>
		/// <value>The stack trace.</value>
		public string StackTrace { get { return stackTrace; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.LogEntryIos"/> class.
		/// </summary>
		/// <param name="timestamp">Log generated time in Unix timestamp.</param>
		/// <param name="message">Message.</param>
		/// <param name="stackTrace">Stack trace.</param>
		public IosLogEntry(double timestamp, LogLevel logLevel, string tag, string message, string stackTrace)
		{
			this.timestamp = timestamp;
			this.logLevel = logLevel;
			this.tag = tag;
			this.message = message;
			this.stackTrace = stackTrace;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Logmew.LogEntryAndroid"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Logmew.LogEntryAndroid"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Logmew] LogEntryIos={{LogLevel={0}, Message={1},\n  StackTrace={2}",
				LogLevel, Message, StackTrace);
		}
	}
#endif
}
