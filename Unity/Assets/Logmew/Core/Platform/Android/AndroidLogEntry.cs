using System;
using UnityEngine;

namespace Logmew
{
#if !UNITY_EDITOR && UNITY_ANDROID
	/// <summary>
	/// Android log entry data.
	/// </summary>
	public class AndroidLogEntry : ILogEntry
	{
		/// <summary>
		/// Log timestamp(Unix timestamp in msec).
		/// </summary>
		public readonly long timestamp;
		public readonly LogLevel logLevel;
		public readonly String tag;
		public readonly String message;
		public readonly String stackTrace;

		/// <summary>
		/// Log origin name.
		/// </summary>
		/// <value>Log origin name.</value>
		public string Origin { get { return "Android"; } }

		/// <summary>
		/// Log timestamp
		/// </summary>
		public DateTimeOffset TimeStamp { get { return LogTime.FromUnixTimestampMilliSec(timestamp); } }

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
		/// Initializes a new instance of the <see cref="Logmew.LogEntryAndroid"/> class.
		/// </summary>
		/// <param name="androidLogEntry">Log entry from android</param>
		public AndroidLogEntry(AndroidJavaObject androidLogEntry)
		{
			timestamp = androidLogEntry.Get<long>("timestamp");
			logLevel = (LogLevel)androidLogEntry.Get<int>("logLevel");
			tag = androidLogEntry.Get<string>("tag");
			message = androidLogEntry.Get<string>("message");
			stackTrace = androidLogEntry.Get<string>("stackTrace");
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Logmew.LogEntryAndroid"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Logmew.LogEntryAndroid"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Logmew] LogEntryAndroid={{LogLevel={0}, Tag={1}, Message={2},\n  StackTrace={3}",
				LogLevel, Tag, Message, StackTrace);
		}
	}
#endif
}
