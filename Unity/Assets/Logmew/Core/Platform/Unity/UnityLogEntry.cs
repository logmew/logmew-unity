using System;
using SyslogNet.Client;
using UnityEngine;

namespace Logmew
{
	/// <summary>
	/// Unity log entry data.
	/// </summary>
	public class UnityLogEntry : ILogEntry
	{
		public readonly DateTimeOffset timestamp;
		public readonly LogType logType;
		public readonly String tag;
		public readonly string message;
		public readonly string stackTrace;

		/// <summary>
		/// Log origin name.
		/// </summary>
		/// <value>Log origin name.</value>
		public string Origin { get { return "Unity"; } }

		/// <summary>
		/// Log timestamp
		/// </summary>
		public DateTimeOffset TimeStamp { get { return timestamp; } }

		/// <summary>
		/// Log level
		/// </summary>
		/// <value>The log level.</value>
		public LogLevel LogLevel
		{
			get { return LogLevelUtility.FromLogType(logType); }
		}

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
		public string StackTrace
		{
			get { return StackTraceUnity.Normalize(stackTrace); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.LogEntryUnity"/> class.
		/// </summary>
		/// <param name="time">Log generation time.</param>
		/// <param name="logType">Log type.</param>
		/// <param name="tag">Tag</param>
		/// <param name="message">Log message.</param>
		/// <param name="stackTrace">Stack trace.</param>
		public UnityLogEntry(DateTimeOffset time, LogType logType, string tag, object message, string stackTrace)
		{
			this.timestamp = time;
			this.logType = logType;
			this.tag = tag;
			if (message ==  null) {
				this.message = string.Empty;
			} else {
				this.message = message.ToString();
			}
			this.stackTrace = stackTrace;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.LogEntryUnity"/> class.
		/// </summary>
		/// <param name="time">Log generation time.</param>
		/// <param name="logType">Log type.</param>
		/// <param name="message">Log message.</param>
		/// <param name="stackTrace">Stack trace.</param>
		public UnityLogEntry(DateTimeOffset time, LogType logType, object message, string stackTrace)
			: this(time, logType, string.Empty, message, stackTrace) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.LogEntryUnity"/> class.
		/// </summary>
		/// <param name="logType">Log type.</param>
		/// <param name="tag">Tag.</param>
		/// <param name="message">Log message.</param>
		/// <param name="stackTrace">Stack trace.</param>
		public UnityLogEntry(LogType logType, string tag, object message, string stackTrace)
			: this(DateTimeOffset.Now, logType, tag, message, stackTrace)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.LogEntryUnity"/> class.
		/// </summary>
		/// <param name="logType">Log type.</param>
		/// <param name="message">Log message.</param>
		/// <param name="stackTrace">Stack trace.</param>
		public UnityLogEntry(LogType logType, object message, string stackTrace)
			: this(DateTimeOffset.Now, logType, string.Empty, message, stackTrace)
		{
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Logmew.LogEntryUnity"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Logmew.LogEntryUnity"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Logmew] LogEntryUnity={{LogType={0}, Message={1},\n  StackTrace={2}",
				logType, message, stackTrace);
		}
	}
}
