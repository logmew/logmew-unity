using System;
using System.Runtime.InteropServices;
using SyslogNet.Client;

#if !UNITY_EDITOR && UNITY_IOS
namespace Logmew
{
	/// <summary>
	/// Receives logs from iOS codebase.
	/// </summary>
	public static class IosLogManager
	{
		public delegate void LogDelegate(double timestamp, LogLevel logLevel, string tag,
			string message, string stackTrace);

		private static bool active;

		/// <summary>
		/// Occurs when new log comes.
		/// </summary>
		public static event Action<IosLogEntry> OnLog = delegate { };

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Logmew.LogmewIosBridge"/> is active.
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
					// Pass native the entry point of sendLogAsync
					LGMSetCallback(sendLogAsync);
				} else {
					LGMSetCallback(null);
				}
			}
		}


		/// <summary>
		/// Sets the minimum log level.
		/// </summary>
		/// <value>The minimum log level.</value>
		public static LogLevel MinLogLevel
		{
			set	{ LGMSetMinLogLevel((int)value); }
		}

		/// <summary>
		/// To include stacktrace in log or not.
		/// </summary>
		/// <value><c>true</c> to provide stack trace; otherwise, <c>false</c>.</value>
		public static bool ProvidesStackTrace
		{
			set { LGMEnableStackTrace(value); }
		}

		/// <summary>
		/// Entry point of log send proc, invoked from iOS codebase.
		/// </summary>
		/// <param name="timestamp">Log generated time in Unix timestamp.</param>
		/// <param name="logLevel">Log level.</param>
		/// <param name="tag">Tag.</param>
		/// <param name="message">Message.</param>
		/// <param name="stackTrace">Stack trace.</param>
		[AOT.MonoPInvokeCallback(typeof(LogDelegate))]
		public static void sendLogAsync(double timestamp, LogLevel logLevel, string tag,
			string message, string stackTrace)
		{
			var logEntry = new IosLogEntry(timestamp, logLevel, tag, message, stackTrace);
			OnLog(logEntry);
		}

		[DllImport("__Internal")]
		public extern static void LGMSetCallback(LogDelegate sendLogAsyncProc);

		[DllImport("__Internal")]
		public extern static void LGMSetMinLogLevel(int logLevel);

		[DllImport("__Internal")]
		public extern static void LGMEnableStackTrace(bool enable);
	}
}
#endif
