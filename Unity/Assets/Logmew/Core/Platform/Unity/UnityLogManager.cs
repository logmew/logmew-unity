using System;
using UnityEngine;

namespace Logmew
{

	/// <summary>
	/// Receives logs from Unity codebase.
	/// </summary>
	public static class UnityLogManager
	{
		private static bool active;
		private static LogLevel minLogLevel = LogLevel.Debug;
		private static bool providesStackTrace = true;

		/// <summary>
		/// Occurs when new log comes.
		/// </summary>
		public static event Action<UnityLogEntry> OnLog = delegate { };

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Logmew.LogmewUnityBridge"/>
		/// is active.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public static bool Active
		{
			get { return active; }
			set
			{
				if (active == value) {
					return;
				}
				active = value;
				if (active) {
					Application.logMessageReceivedThreaded += unityLogHandler;
				} else {
					Application.logMessageReceivedThreaded -= unityLogHandler;
				}
			}
		}

		/// <summary>
		/// Minimum log type.
		/// </summary>
		/// <value>The minimum log type.</value>
		public static LogType MinLogType
		{
			get { return LogLevelUtility.ToLogType(minLogLevel); }
			set { minLogLevel = LogLevelUtility.FromLogType(value); }
		}

		/// <summary>
		/// Minimum log level.
		/// </summary>
		/// <value>The minimum log level.</value>
		public static LogLevel MinLogLevel
		{
			get { return minLogLevel; }
			set { minLogLevel = value; }
		}

		/// <summary>
		/// To include stacktrace in log or not.
		/// </summary>
		/// <value><c>true</c> to provide stack trace; otherwise, <c>false</c>.</value>
		public static bool ProvidesStackTrace
		{
			get { return providesStackTrace; }
			set { providesStackTrace = value; }
		}

		internal static void Log(UnityLogEntry logEntry)
		{
			if (isBelowMinLogType(logEntry.logType)) {
				return;
			}

			OnLog(logEntry);
		}

		private static void unityLogHandler(string msg, string trace, LogType logType)
		{
#if RUNTIME_UNITY4
			if (!active) {
				return;
			}
#endif
			if (isBelowMinLogType(logType)) {
				return;
			}

			var logEntry = new UnityLogEntry(logType, msg, providesStackTrace ? trace : string.Empty);
			OnLog(logEntry);
		}

		private static bool isBelowMinLogType(LogType logType)
		{
			var logLevel = LogLevelUtility.FromLogType(logType);
			return (int)minLogLevel < (int)logLevel;
		}
	}
}
